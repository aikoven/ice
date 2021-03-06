//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using System.Diagnostics;
using System.Collections.Generic;
using Ice.admin.Test;

namespace Ice.admin
{
    public class TestFacet : ITestFacet
    {
        public void op(Ice.Current current)
        {
        }
    }

    public class RemoteCommunicator : IRemoteCommunicator
    {
        public RemoteCommunicator(Communicator communicator) => _communicator = communicator;

        public IObjectPrx getAdmin(Current current) => _communicator.GetAdmin();

        public Dictionary<string, string> getChanges(Ice.Current current)
        {
            lock (this)
            {
                return _changes;
            }
        }

        public void print(string message, Current current) => _communicator.Logger.Print(message);

        public void trace(string category, string message, Current current) => _communicator.Logger.Trace(category, message);

        public void warning(string message, Current current) => _communicator.Logger.Warning(message);

        public void error(string message, Current current) => _communicator.Logger.Error(message);

        public void shutdown(Current current) => _communicator.Shutdown();

        // Note that we are executing in a thread of the *main* communicator,
        // not the one that is being shut down.
        public void waitForShutdown(Current current) => _communicator.WaitForShutdown();

        public void destroy(Current current) => _communicator.Destroy();

        public void updated(Dictionary<string, string> changes)
        {
            lock (this)
            {
                _changes = changes;
            }
        }

        private Communicator _communicator;
        private Dictionary<string, string> _changes;
    }

    public class RemoteCommunicatorFactoryI : IRemoteCommunicatorFactory
    {
        public IRemoteCommunicatorPrx createCommunicator(Dictionary<string, string> props, Current current)
        {
            //
            // Prepare the property set using the given properties.
            //
            ILogger? logger = null;
            string? value;
            int nullLogger;
            if (props.TryGetValue("NullLogger", out value) && int.TryParse(value, out nullLogger) && nullLogger > 0)
            {
                logger = new NullLogger();
            }

            //
            // Initialize a new communicator.
            //
            var communicator = new Communicator(props, logger: logger);

            //
            // Install a custom admin facet.
            //
            try
            {
                var testFacet = new TestFacet();
                communicator.AddAdminFacet("TestFacet", testFacet);
            }
            catch (System.ArgumentException)
            {
            }

            //
            // The RemoteCommunicator servant also implements PropertiesAdminUpdateCallback.
            // Set the callback on the admin facet.
            //
            var servant = new RemoteCommunicator(communicator);
            var propFacet = communicator.FindAdminFacet("Properties");

            if (propFacet != null)
            {
                var admin = (INativePropertiesAdmin)propFacet;
                Debug.Assert(admin != null);
                admin.AddUpdateCallback(servant.updated);
            }

            return current.Adapter.AddWithUUID(servant, IRemoteCommunicatorPrx.Factory);
        }

        public void shutdown(Current current) => current.Adapter.Communicator.Shutdown();

        private class NullLogger : ILogger
        {
            public void Print(string message)
            {
            }

            public void Trace(string category, string message)
            {
            }

            public void Warning(string message)
            {
            }

            public void Error(string message)
            {
            }

            public string GetPrefix() => "NullLogger";

            public ILogger CloneWithPrefix(string prefix) => this;
        }
    }
}
