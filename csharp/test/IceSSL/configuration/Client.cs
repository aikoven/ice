//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using System;
using System.Reflection;

[assembly: AssemblyTitle("IceTest")]
[assembly: AssemblyDescription("Ice test")]
[assembly: AssemblyCompany("ZeroC, Inc.")]

public class Client : Test.TestHelper
{
    public override void run(string[] args)
    {
        using var communicator = initialize(ref args);
        if (args.Length < 1)
        {
            throw new ArgumentException("Usage: client testdir");
        }

        Test.IServerFactoryPrx factory;
        factory = AllTests.allTests(this, args[0]);
        factory.shutdown();
    }

    public static int Main(string[] args) => Test.TestDriver.runTest<Client>(args);
}
