//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using Ice;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IceLocatorDiscovery
{
    public sealed class PluginFactory : Ice.IPluginFactory
    {
        public Ice.IPlugin
        Create(Communicator communicator, string name, string[] args) => new PluginI(name, communicator);
    }

    public interface IPlugin : Ice.IPlugin
    {
        List<ILocatorPrx> GetLocators(string instanceName, int waitTime);
    }

    internal class Request : TaskCompletionSource<Object_Ice_invokeResult>
    {
        public Request(LocatorI locator,
                       string operation,
                       OperationMode mode,
                       byte[] inParams,
                       Dictionary<string, string>? context)
        {
            _locator = locator;
            _operation = operation;
            _mode = mode;
            _inParams = inParams;
            _context = context;
        }

        public void
        Invoke(ILocatorPrx l)
        {
            if (_locatorPrx == null || !_locatorPrx.Equals(l))
            {
                _locatorPrx = l;
                l.InvokeAsync(_operation, _mode, _inParams, _context).ContinueWith(
                    (task) =>
                    {
                        try
                        {
                            SetResult(task.Result);
                        }
                        catch (AggregateException ae)
                        {
                            Exception(ae.InnerException);
                        }
                    },
                    TaskScheduler.Current);
            }
            else
            {
                Debug.Assert(_exception != null);
                throw _exception;
            }
        }

        private void
        Exception(System.Exception ex)
        {
            try
            {
                throw ex;
            }
            catch (Ice.RequestFailedException exc)
            {
                SetException(exc);
            }
            catch (Ice.UnknownException exc)
            {
                SetException(exc);
            }
            catch (Ice.NoEndpointException)
            {
                SetException(new Ice.ObjectNotExistException());
            }
            catch (Ice.ObjectAdapterDeactivatedException)
            {
                SetException(new Ice.ObjectNotExistException());
            }
            catch (Ice.CommunicatorDestroyedException)
            {
                SetException(new Ice.ObjectNotExistException());
            }
            catch (System.Exception exc)
            {
                _exception = exc;
                _locator.Invoke(_locatorPrx, this); // Retry with new locator proxy
            }
        }

        private readonly LocatorI _locator;
        private readonly string _operation;
        private readonly OperationMode _mode;
        private readonly Dictionary<string, string>? _context;
        private readonly byte[] _inParams;

        private ILocatorPrx? _locatorPrx;
        private System.Exception? _exception;
    }

    internal class VoidLocatorI : ILocator
    {
        public ValueTask<IObjectPrx?> FindObjectByIdAsync(Identity id, Current current)
        {
            IObjectPrx? prx = null;
            return new ValueTask<IObjectPrx?>(prx);
        }

        public ValueTask<IObjectPrx?> FindAdapterByIdAsync(string id, Current current)
        {
            IObjectPrx? prx = null;
            return new ValueTask<IObjectPrx?>(prx);
        }

        public ILocatorRegistryPrx? GetRegistry(Current current) => null;
    }

    internal class LocatorI : BlobjectAsync, IceInternal.ITimerTask
    {
        public
        LocatorI(string name, ILookupPrx lookup, Communicator communicator, string instanceName, ILocatorPrx voidLocator)
        {
            _lookup = lookup;
            _timeout = communicator.GetPropertyAsInt($"{name}.Timeout") ?? 300;
            if (_timeout < 0)
            {
                _timeout = 300;
            }
            _retryCount = communicator.GetPropertyAsInt("${name}.RetryCount") ?? 3;
            if (_retryCount < 0)
            {
                _retryCount = 0;
            }
            _retryDelay = communicator.GetPropertyAsInt($"{name}.RetryDelay") ?? 2000;
            if (_retryDelay < 0)
            {
                _retryDelay = 0;
            }
            _timer = lookup.Communicator.Timer();
            _traceLevel = communicator.GetPropertyAsInt($"{name}.Trace.Lookup") ?? 0;
            _instanceName = instanceName;
            _warned = false;
            _locator = lookup.Communicator.GetDefaultLocator();
            _voidLocator = voidLocator;
            _pending = false;
            _pendingRetryCount = 0;
            _failureCount = 0;
            _warnOnce = true;

            //
            // Create one lookup proxy per endpoint from the given proxy. We want to send a multicast
            // datagram on each endpoint.
            //
            var single = new IEndpoint[1];
            foreach (IEndpoint endpt in lookup.Endpoints)
            {
                single[0] = endpt;
                _lookups[lookup.Clone(endpoints: single)] = null;
            }
            Debug.Assert(_lookups.Count > 0);
        }

        public void
        SetLookupReply(ILookupReplyPrx lookupReply)
        {
            //
            // Use a lookup reply proxy whose adress matches the interface used to send multicast datagrams.
            //
            var single = new IEndpoint[1];
            foreach (ILookupPrx key in new List<ILookupPrx>(_lookups.Keys))
            {
                var info = (UDPEndpointInfo)key.Endpoints[0].GetInfo();
                if (info.McastInterface.Length > 0)
                {
                    foreach (IEndpoint q in lookupReply.Endpoints)
                    {
                        EndpointInfo r = q.GetInfo();
                        if (r is IPEndpointInfo && ((IPEndpointInfo)r).Host.Equals(info.McastInterface))
                        {
                            single[0] = q;
                            _lookups[key] = lookupReply.Clone(endpoints: single);
                        }
                    }
                }

                if (_lookups[key] == null)
                {
                    // Fallback: just use the given lookup reply proxy if no matching endpoint found.
                    _lookups[key] = lookupReply;
                }
            }
        }

        public override Task<Ice.Object_Ice_invokeResult>
        IceInvokeAsync(byte[] inParams, Current current)
        {
            lock (this)
            {
                var request = new Request(this, current.Operation, current.Mode, inParams, current.Context);
                Invoke(null, request);
                return request.Task;
            }
        }

        public List<Ice.ILocatorPrx>
        GetLocators(string instanceName, int waitTime)
        {
            //
            // Clear locators from previous search.
            //
            lock (this)
            {
                _locators.Clear();
            }

            //
            // Find a locator
            //
            Invoke(null, null);

            //
            // Wait for responses
            //
            if (instanceName.Length == 0)
            {
                Thread.Sleep(waitTime);
            }
            else
            {
                lock (this)
                {
                    while (!_locators.ContainsKey(instanceName) && _pending)
                    {
                        Monitor.Wait(this, waitTime);
                    }
                }
            }

            //
            // Return found locators
            //
            lock (this)
            {
                return new List<Ice.ILocatorPrx>(_locators.Values);
            }
        }

        public void
        FoundLocator(ILocatorPrx? locator)
        {
            lock (this)
            {
                if (locator == null ||
                   (_instanceName.Length > 0 && !locator.Identity.Category.Equals(_instanceName)))
                {
                    if (_traceLevel > 2)
                    {
                        var s = new StringBuilder("ignoring locator reply: instance name doesn't match\n");
                        s.Append("expected = ").Append(_instanceName);
                        s.Append("received = ").Append(locator.Identity.Category);
                        _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                    }
                    return;
                }

                //
                // If we already have a locator assigned, ensure the given locator
                // has the same identity, otherwise ignore it.
                //
                if (_pendingRequests.Count > 0 &&
                    _locator != null && !locator.Identity.Category.Equals(_locator.Identity.Category))
                {
                    if (!_warned)
                    {
                        _warned = true; // Only warn once

                        locator.Communicator.Logger.Warning(
                            "received Ice locator with different instance name:\n" +
                            "using = `" + _locator.Identity.Category + "'\n" +
                            "received = `" + locator.Identity.Category + "'\n" +
                            "This is typically the case if multiple Ice locators with different " +
                            "instance names are deployed and the property `IceLocatorDiscovery.InstanceName'" +
                            "is not set.");

                    }
                    return;
                }

                if (_pending) // No need to continue, we found a locator
                {
                    _timer.Cancel(this);
                    _pendingRetryCount = 0;
                    _pending = false;
                }

                if (_traceLevel > 0)
                {
                    var s = new StringBuilder("locator lookup succeeded:\nlocator = ");
                    s.Append(locator);
                    if (_instanceName.Length == 0)
                    {
                        s.Append("\ninstance name = ").Append(_instanceName);
                    }
                    _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                }

                ILocatorPrx? l = null;
                if (_pendingRequests.Count == 0)
                {
                    _locators.TryGetValue(locator.Identity.Category, out _locator);
                }
                else
                {
                    l = _locator;
                }

                if (l != null)
                {
                    //
                    // We found another locator replica, append its endpoints to the
                    // current locator proxy endpoints.
                    //
                    var newEndpoints = new List<IEndpoint>(l.Endpoints);
                    foreach (IEndpoint p in locator.Endpoints)
                    {
                        //
                        // Only add endpoints if not already in the locator proxy endpoints
                        //
                        bool found = false;
                        foreach (IEndpoint q in newEndpoints)
                        {
                            if (p.Equals(q))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            newEndpoints.Add(p);
                        }
                    }
                    l = l.Clone(endpoints: newEndpoints.ToArray());
                }
                else
                {
                    l = locator;
                }

                if (_pendingRequests.Count == 0)
                {
                    _locators[locator.Identity.Category] = l;
                    Monitor.Pulse(this);
                }
                else
                {
                    _locator = l;
                    if (_instanceName.Length == 0)
                    {
                        _instanceName = _locator.Identity.Category; // Stick to the first locator
                    }

                    //
                    // Send pending requests if any.
                    //
                    foreach (Request req in _pendingRequests)
                    {
                        req.Invoke(_locator);
                    }
                    _pendingRequests.Clear();
                }
            }
        }

        public void
        Invoke(ILocatorPrx? locator, Request? request)
        {
            lock (this)
            {
                if (request != null && _locator != null && _locator != locator)
                {
                    request.Invoke(_locator);
                }
                else if (request != null && IceInternal.Time.CurrentMonotonicTimeMillis() < _nextRetry)
                {
                    request.Invoke(_voidLocator); // Don't retry to find a locator before the retry delay expires
                }
                else
                {
                    _locator = null;

                    if (request != null)
                    {
                        _pendingRequests.Add(request);
                    }

                    if (!_pending) // No request in progress
                    {
                        _pending = true;
                        _pendingRetryCount = _retryCount;
                        _failureCount = 0;
                        try
                        {
                            if (_traceLevel > 1)
                            {
                                var s = new StringBuilder("looking up locator:\nlookup = ");
                                s.Append(_lookup);
                                if (_instanceName.Length == 0)
                                {
                                    s.Append("\ninstance name = ").Append(_instanceName);
                                }
                                _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                            }

                            foreach (KeyValuePair<ILookupPrx, ILookupReplyPrx?> l in _lookups)
                            {
                                l.Key.FindLocatorAsync(_instanceName, l.Value).ContinueWith(t =>
                                {
                                    try
                                    {
                                        t.Wait();
                                    }
                                    catch (AggregateException ex)
                                    {
                                        Exception(ex.InnerException);
                                    }
                                }, l.Key.Scheduler); // Send multicast request.
                            }
                            _timer.Schedule(this, _timeout);
                        }
                        catch (LocalException ex)
                        {
                            if (_traceLevel > 0)
                            {
                                var s = new StringBuilder("locator lookup failed:\nlookup = ");
                                s.Append(_lookup);
                                if (_instanceName.Length == 0)
                                {
                                    s.Append("\ninstance name = ").Append(_instanceName);
                                }
                                s.Append("\n").Append(ex);
                                _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                            }

                            foreach (Request req in _pendingRequests)
                            {
                                req.Invoke(_voidLocator);
                            }
                            _pendingRequests.Clear();
                            _pendingRetryCount = 0;
                            _pending = false;
                        }
                    }
                }
            }
        }

        private void Exception(System.Exception ex)
        {
            lock (this)
            {
                if (++_failureCount == _lookups.Count && _pending)
                {
                    //
                    // All the lookup calls failed, cancel the timer and propagate the error to the requests.
                    //
                    _timer.Cancel(this);
                    _pendingRetryCount = 0;
                    _pending = false;

                    if (_warnOnce)
                    {
                        var builder = new StringBuilder();
                        builder.Append("failed to lookup locator with lookup proxy `");
                        builder.Append(_lookup);
                        builder.Append("':\n");
                        builder.Append(ex);
                        _lookup.Communicator.Logger.Warning(builder.ToString());
                        _warnOnce = false;
                    }

                    if (_traceLevel > 0)
                    {
                        var s = new StringBuilder("locator lookup failed:\nlookup = ");
                        s.Append(_lookup);
                        if (_instanceName.Length == 0)
                        {
                            s.Append("\ninstance name = ").Append(_instanceName);
                        }
                        s.Append("\n").Append(ex);
                        _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                    }

                    if (_pendingRequests.Count == 0)
                    {
                        Monitor.Pulse(this);
                    }
                    else
                    {
                        foreach (Request req in _pendingRequests)
                        {
                            req.Invoke(_voidLocator);
                        }
                        _pendingRequests.Clear();
                    }
                }
            }
        }

        public void RunTimerTask()
        {
            lock (this)
            {
                if (!_pending)
                {
                    Debug.Assert(_pendingRequests.Count == 0);
                    return; // Request failed
                }

                if (_pendingRetryCount > 0)
                {
                    --_pendingRetryCount;
                    try
                    {
                        if (_traceLevel > 1)
                        {
                            var s = new StringBuilder("retrying locator lookup:\nlookup = ");
                            s.Append(_lookup);
                            s.Append("\nretry count = ").Append(_retryCount);
                            if (_instanceName.Length == 0)
                            {
                                s.Append("\ninstance name = ").Append(_instanceName);
                            }
                            _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                        }

                        foreach (KeyValuePair<ILookupPrx, ILookupReplyPrx?> l in _lookups)
                        {
                            l.Key.FindLocatorAsync(_instanceName, l.Value).ContinueWith(t =>
                            {
                                try
                                {
                                    t.Wait();
                                }
                                catch (AggregateException ex)
                                {
                                    Exception(ex.InnerException);
                                }
                            }, l.Key.Scheduler); // Send multicast request.
                        }
                        _timer.Schedule(this, _timeout);
                        return;
                    }
                    catch (Ice.LocalException)
                    {
                    }
                    _pendingRetryCount = 0;
                }

                Debug.Assert(_pendingRetryCount == 0);
                _pending = false;

                if (_traceLevel > 0)
                {
                    var s = new StringBuilder("locator lookup timed out:\nlookup = ");
                    s.Append(_lookup);
                    if (_instanceName.Length == 0)
                    {
                        s.Append("\ninstance name = ").Append(_instanceName);
                    }
                    _lookup.Communicator.Logger.Trace("Lookup", s.ToString());
                }

                if (_pendingRequests.Count == 0)
                {
                    Monitor.Pulse(this);
                }
                else
                {
                    foreach (Request req in _pendingRequests)
                    {
                        req.Invoke(_voidLocator);
                    }
                    _pendingRequests.Clear();
                }
                _nextRetry = IceInternal.Time.CurrentMonotonicTimeMillis() + _retryDelay;
            }
        }

        private readonly ILookupPrx _lookup;
        private readonly Dictionary<ILookupPrx, ILookupReplyPrx?> _lookups = new Dictionary<ILookupPrx, ILookupReplyPrx?>();
        private readonly int _timeout;
        private readonly IceInternal.Timer _timer;
        private readonly int _traceLevel;
        private readonly int _retryCount;
        private readonly int _retryDelay;

        private string _instanceName;
        private bool _warned;
        private ILocatorPrx? _locator;
        private readonly ILocatorPrx _voidLocator;
        private readonly Dictionary<string, ILocatorPrx> _locators = new Dictionary<string, ILocatorPrx>();

        private bool _pending;
        private int _pendingRetryCount;
        private int _failureCount;
        private bool _warnOnce = true;
        private readonly List<Request> _pendingRequests = new List<Request>();
        private long _nextRetry;
    };

    internal class LookupReplyI : ILookupReply
    {
        public LookupReplyI(LocatorI locator) => _locator = locator;

        public void
        FoundLocator(ILocatorPrx? locator, Current current) => _locator.FoundLocator(locator);

        private readonly LocatorI _locator;
    }

    internal class PluginI : Ice.IPlugin
    {
        public
        PluginI(string name, Ice.Communicator communicator)
        {
            _name = name;
            _communicator = communicator;
        }

        public void
        Initialize()
        {
            bool ipv4 = (_communicator.GetPropertyAsInt("Ice.IPv4") ?? 1) > 0;
            bool preferIPv6 = _communicator.GetPropertyAsInt("Ice.PreferIPv6Address") > 0;

            string address;
            if (ipv4 && !preferIPv6)
            {
                address = _communicator.GetProperty($"{_name}.Address") ?? "239.255.0.1";
            }
            else
            {
                address = _communicator.GetProperty($"{_name}.Address") ?? "ff15::1";
            }
            int port = _communicator.GetPropertyAsInt($"{_name}.Port") ?? 4061;
            string intf = _communicator.GetProperty($"{_name}.Interface") ?? "";

            string lookupEndpoints = _communicator.GetProperty($"{_name}.Lookup") ?? "";
            if (lookupEndpoints.Length == 0)
            {
                int protocol = ipv4 && !preferIPv6 ? IceInternal.Network.EnableIPv4 : IceInternal.Network.EnableIPv6;
                List<string> interfaces = IceInternal.Network.GetInterfacesForMulticast(intf, protocol);
                foreach (string p in interfaces)
                {
                    if (p != interfaces[0])
                    {
                        lookupEndpoints += ":";
                    }
                    lookupEndpoints += "udp -h \"" + address + "\" -p " + port + " --interface \"" + p + "\"";
                }
            }

            if (_communicator.GetProperty($"{_name}.Reply.Endpoints") == null)
            {
                _communicator.SetProperty($"{_name}.Reply.Endpoints",
                    intf.Length == 0 ? "udp -h *" : $"udp -h \"{intf}\"");
            }

            if (_communicator.GetProperty($"{_name}.Locator.Endpoints") == null)
            {
                _communicator.SetProperty($"{_name}.Locator.AdapterId", Guid.NewGuid().ToString());
            }

            _replyAdapter = _communicator.CreateObjectAdapter(_name + ".Reply");
            _locatorAdapter = _communicator.CreateObjectAdapter(_name + ".Locator");

            // We don't want those adapters to be registered with the locator so clear their locator.
            _replyAdapter.Locator = null;
            _locatorAdapter.Locator = null;

            var lookupPrx = ILookupPrx.Parse($"IceLocatorDiscovery/Lookup -d:{lookupEndpoints}", _communicator);
            // No colloc optimization or router for the multicast proxy!
            lookupPrx = lookupPrx.Clone(clearRouter: false, collocationOptimized: false);

            ILocatorPrx voidLo = _locatorAdapter.AddWithUUID(new VoidLocatorI(), ILocatorPrx.Factory);

            string instanceName = _communicator.GetProperty($"{_name}.InstanceName") ?? "";
            var id = new Identity("Locator", instanceName.Length > 0 ? instanceName : Guid.NewGuid().ToString());

            _defaultLocator = _communicator.GetDefaultLocator();
            _locator = new LocatorI(_name, lookupPrx, _communicator, instanceName, voidLo);
            _locatorPrx = _locatorAdapter.AddWithUUID(_locator, ILocatorPrx.Factory);
            _communicator.SetDefaultLocator(_locatorPrx);

            ILookupReply lookupReplyI = new LookupReplyI(_locator);
            _locator.SetLookupReply(_replyAdapter.AddWithUUID(lookupReplyI, ILookupReplyPrx.Factory)
                .Clone(invocationMode: InvocationMode.Datagram));

            _replyAdapter.Activate();
            _locatorAdapter.Activate();
        }

        public void Destroy()
        {
            if (_replyAdapter != null)
            {
                _replyAdapter.Destroy();
            }
            if (_locatorAdapter != null)
            {
                _locatorAdapter.Destroy();
            }

            if (IObjectPrx.Equals(_communicator.GetDefaultLocator(), _locatorPrx))
            {
                // Restore original default locator proxy, if the user didn't change it in the meantime
                _communicator.SetDefaultLocator(_defaultLocator);
            }
        }

        private readonly string _name;
        private readonly Communicator _communicator;
        private ObjectAdapter? _locatorAdapter;
        private ObjectAdapter? _replyAdapter;
        private LocatorI? _locator;
        private ILocatorPrx? _locatorPrx;
        private ILocatorPrx? _defaultLocator;
    }

    public class Util
    {
        public static void
        RegisterIceLocatorDiscovery(bool loadOnInitialize) =>
            Communicator.RegisterPluginFactory("IceLocatorDiscovery", new PluginFactory(), loadOnInitialize);
    }
}
