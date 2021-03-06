//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

namespace IceInternal
{
    public interface IProtocolPluginFacade
    {
        //
        // Get the Communicator instance with which this facade is
        // associated.
        //
        Ice.Communicator Communicator { get; }

        //
        // Register an EndpointFactory.
        //
        void AddEndpointFactory(IEndpointFactory factory);

        //
        // Get an EndpointFactory.
        //
        IEndpointFactory? GetEndpointFactory(short type);

        //
        // Obtain the type for a name.
        //
        System.Type? FindType(string name);
    }

    public sealed class ProtocolPluginFacade : IProtocolPluginFacade
    {
        public ProtocolPluginFacade(Ice.Communicator communicator) => Communicator = communicator;

        //
        // Get the Communicator instance with which this facade is
        // associated.
        //
        public Ice.Communicator Communicator { get; private set; }

        //
        // Register an EndpointFactory.
        //
        public void AddEndpointFactory(IEndpointFactory factory) => Communicator.AddEndpointFactory(factory);

        //
        // Get an EndpointFactory.
        //
        public IEndpointFactory? GetEndpointFactory(short type) => Communicator.GetEndpointFactory(type);

        //
        // Obtain the type for a name.
        //
        public System.Type? FindType(string name) => AssemblyUtil.FindType(name);
    }
}
