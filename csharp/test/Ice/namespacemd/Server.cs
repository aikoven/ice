//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using Test;
using Ice.namespacemd.Test;

namespace Ice
{
    namespace namespacemd
    {
        public class Server : TestHelper
        {
            public override void run(string[] args)
            {
                using var communicator = initialize(ref args);
                communicator.SetProperty("TestAdapter.Endpoints", getTestEndpoint(0));
                var adapter = communicator.CreateObjectAdapter("TestAdapter");
                adapter.Add("initial", new Initial());
                adapter.Activate();
                serverReady();
                communicator.WaitForShutdown();
            }

            public static int Main(string[] args) => TestDriver.runTest<Server>(args);
        }
    }
}
