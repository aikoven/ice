//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

namespace Ice.stream
{
    public class Client : global::Test.TestHelper
    {
        public override void run(string[] args)
        {
            using var communicator = initialize(createTestProperties(ref args),
                typeIdNamespaces: new string[] { "Ice.stream.TypeId" });
            AllTests.allTests(this);
        }

        public static int Main(string[] args) => global::Test.TestDriver.runTest<Client>(args);
    }
}
