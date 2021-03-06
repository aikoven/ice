//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

namespace Ice.facets
{
    public sealed class H : Test.IH
    {
        public H(Communicator communicator) => _communicator = communicator;

        public string callG(Ice.Current current) => "G";

        public string callH(Current current) => "H";

        public void shutdown(Current current) => _communicator.Shutdown();

        private Communicator _communicator;
    }
}
