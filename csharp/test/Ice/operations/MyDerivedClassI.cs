//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using Ice;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ice.operations
{
    public sealed class MyDerivedClass : Test.IMyDerivedClass, IObject
    {
        private static void test(bool b)
        {
            if (!b)
            {
                throw new System.Exception();
            }
        }

        //
        // Override the Object "pseudo" operations to verify the operation mode.
        //

        public bool IceIsA(string id, Current current)
        {
            test(current.Mode == OperationMode.Nonmutating);
            return typeof(Test.IMyDerivedClass).GetAllIceTypeIds().Contains(id);
        }

        public void IcePing(Current current) => test(current.Mode == OperationMode.Nonmutating);

        public string[] IceIds(Current current)
        {
            test(current.Mode == OperationMode.Nonmutating);
            return typeof(Test.IMyDerivedClass).GetAllIceTypeIds();
        }

        public string IceId(Current current)
        {
            test(current.Mode == OperationMode.Nonmutating);
            return typeof(Test.IMyDerivedClass).GetIceTypeId()!;
        }

        public void shutdown(Current current) => current.Adapter.Communicator.Shutdown();

        public bool supportsCompress(Current current) => IceInternal.BZip2.Supported();

        public void opVoid(Current current) => test(current.Mode == OperationMode.Normal);

        public (bool, bool) opBool(bool p1, bool p2, Current current) => (p2, p1);

        public (bool[], bool[]) opBoolS(bool[] p1, bool[] p2, Current current)
        {
            var p3 = new bool[p1.Length + p2.Length];
            Array.Copy(p1, p3, p1.Length);
            Array.Copy(p2, 0, p3, p1.Length, p2.Length);

            bool[] r = new bool[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (bool[][], bool[][]) opBoolSS(bool[][] p1, bool[][] p2, Current current)
        {
            var p3 = new bool[p1.Length + p2.Length][];
            Array.Copy(p1, p3, p1.Length);
            Array.Copy(p2, 0, p3, p1.Length, p2.Length);

            bool[][] r = new bool[p1.Length][];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (byte, byte) opByte(byte p1, byte p2, Current current) => (p1, (byte)(p1 ^ p2));

        public (Dictionary<byte, bool>, Dictionary<byte, bool>)
        opByteBoolD(Dictionary<byte, bool> p1, Dictionary<byte, bool> p2, Current current)
        {
            var r = new Dictionary<byte, bool>();
            foreach (KeyValuePair<byte, bool> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<byte, bool> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p1);
        }

        public (byte[], byte[]) opByteS(byte[] p1, byte[] p2, Current current)
        {
            var p3 = new byte[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                p3[i] = p1[p1.Length - (i + 1)];
            }

            byte[] r = new byte[p1.Length + p2.Length];
            Array.Copy(p1, r, p1.Length);
            Array.Copy(p2, 0, r, p1.Length, p2.Length);
            return (r, p3);
        }

        public (byte[][], byte[][]) opByteSS(byte[][] p1, byte[][] p2, Current current)
        {
            var p3 = new byte[p1.Length][];
            for (int i = 0; i < p1.Length; i++)
            {
                p3[i] = p1[p1.Length - (i + 1)];
            }

            byte[][] r = new byte[p1.Length + p2.Length][];
            Array.Copy(p1, r, p1.Length);
            Array.Copy(p2, 0, r, p1.Length, p2.Length);
            return (r, p3);
        }

        public (double, float, double) opFloatDouble(float p1, double p2, Current current) => (p2, p1, p2);

        public (double[], float[], double[]) opFloatDoubleS(float[] p1, double[] p2, Current current)
        {
            var p4 = new double[p2.Length];
            for (int i = 0; i < p2.Length; i++)
            {
                p4[i] = p2[p2.Length - (i + 1)];
            }

            double[] r = new double[p2.Length + p1.Length];
            Array.Copy(p2, r, p2.Length);
            for (int i = 0; i < p1.Length; i++)
            {
                r[p2.Length + i] = p1[i];
            }
            return (r, p1, p4);
        }

        public (double[][], float[][], double[][])
        opFloatDoubleSS(float[][] p1, double[][] p2, Current current)
        {
            var p4 = new double[p2.Length][];
            for (int i = 0; i < p2.Length; i++)
            {
                p4[i] = p2[p2.Length - (i + 1)];
            }

            double[][] r = new double[p2.Length + p2.Length][];
            Array.Copy(p2, r, p2.Length);
            for (int i = 0; i < p2.Length; i++)
            {
                r[p2.Length + i] = new double[p2[i].Length];
                for (int j = 0; j < p2[i].Length; j++)
                {
                    r[p2.Length + i][j] = p2[i][j];
                }
            }
            return (r, p1, p4);
        }

        public (Dictionary<long, float>, Dictionary<long, float>)
        opLongFloatD(Dictionary<long, float> p1, Dictionary<long, float> p2, Current current)
        {
            var r = new Dictionary<long, float>();
            foreach (KeyValuePair<long, float> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<long, float> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p1);
        }

        public (Test.IMyClassPrx, Test.IMyClassPrx, Test.IMyClassPrx)
        opMyClass(Test.IMyClassPrx p1, Current current) => (current.Adapter.CreateProxy(current.Id, Test.IMyClassPrx.Factory),
             p1,
             current.Adapter.CreateProxy("noSuchIdentity", Test.IMyClassPrx.Factory));

        public (Test.MyEnum, Test.MyEnum) opMyEnum(Test.MyEnum p1, Current current) => (Test.MyEnum.enum3, p1);

        public (Dictionary<short, int>, Dictionary<short, int>)
        opShortIntD(Dictionary<short, int> p1, Dictionary<short, int> p2, Current current)
        {
            var r = new Dictionary<short, int>();
            foreach (KeyValuePair<short, int> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<short, int> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p1);
        }

        public (long, short, int, long)
        opShortIntLong(short p1, int p2, long p3, Current current) => (p3, p1, p2, p3);

        public (long[], short[], int[], long[]) opShortIntLongS(short[] p1, int[] p2, long[] p3, Current current)
        {
            var p5 = new int[p2.Length];
            for (int i = 0; i < p2.Length; i++)
            {
                p5[i] = p2[p2.Length - (i + 1)];
            }

            var p6 = new long[p3.Length + p3.Length];
            Array.Copy(p3, p6, p3.Length);
            Array.Copy(p3, 0, p6, p3.Length, p3.Length);

            return (p3, p1, p5, p6);
        }

        public (long[][], short[][], int[][], long[][])
        opShortIntLongSS(short[][] p1, int[][] p2, long[][] p3, Current current)
        {
            var p5 = new int[p2.Length][];
            for (int i = 0; i < p2.Length; i++)
            {
                p5[i] = p2[p2.Length - (i + 1)];
            }

            var p6 = new long[p3.Length + p3.Length][];
            Array.Copy(p3, p6, p3.Length);
            Array.Copy(p3, 0, p6, p3.Length, p3.Length);

            return (p3, p1, p5, p6);
        }

        public (string, string) opString(string p1, string p2, Current current) => (p1 + " " + p2, p2 + " " + p1);

        public (Dictionary<string, Test.MyEnum>, Dictionary<string, Test.MyEnum>)
        opStringMyEnumD(Dictionary<string, Test.MyEnum> p1, Dictionary<string, Test.MyEnum> p2, Current current)
        {
            var r = new Dictionary<string, Test.MyEnum>();
            foreach (KeyValuePair<string, Test.MyEnum> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<string, Test.MyEnum> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p1);
        }

        public (Dictionary<Test.MyEnum, string>, Dictionary<Test.MyEnum, string>)
        opMyEnumStringD(Dictionary<Test.MyEnum, string> p1, Dictionary<Test.MyEnum, string> p2, Current current)
        {
            var r = new Dictionary<Test.MyEnum, string>();
            foreach (var e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (var e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p1);
        }

        public (Dictionary<Test.MyStruct, Test.MyEnum>, Dictionary<Test.MyStruct, Test.MyEnum>)
        opMyStructMyEnumD(Dictionary<Test.MyStruct, Test.MyEnum> p1, Dictionary<Test.MyStruct, Test.MyEnum> p2,
            Current current)
        {
            var r = new Dictionary<Test.MyStruct, Test.MyEnum>();
            foreach (var e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (var e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p1);
        }

        public (Dictionary<byte, bool>[], Dictionary<byte, bool>[]) opByteBoolDS(
            Dictionary<byte, bool>[] p1, Dictionary<byte, bool>[] p2, Current current)
        {
            var p3 = new Dictionary<byte, bool>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            Dictionary<byte, bool>[] r = new Dictionary<byte, bool>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<short, int>[], Dictionary<short, int>[])
        opShortIntDS(Dictionary<short, int>[] p1, Dictionary<short, int>[] p2, Current current)
        {
            var p3 = new Dictionary<short, int>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            var r = new Dictionary<short, int>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<long, float>[], Dictionary<long, float>[])
        opLongFloatDS(Dictionary<long, float>[] p1, Dictionary<long, float>[] p2, Current current)
        {
            var p3 = new Dictionary<long, float>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            var r = new Dictionary<long, float>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<string, string>[], Dictionary<string, string>[])
        opStringStringDS(Dictionary<string, string>[] p1, Dictionary<string, string>[] p2, Current current)
        {
            var p3 = new Dictionary<string, string>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            var r = new Dictionary<string, string>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<string, Test.MyEnum>[], Dictionary<string, Test.MyEnum>[])
        opStringMyEnumDS(Dictionary<string, Test.MyEnum>[] p1, Dictionary<string, Test.MyEnum>[] p2, Current current)
        {
            var p3 = new Dictionary<string, Test.MyEnum>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            var r = new Dictionary<string, Test.MyEnum>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<Test.MyEnum, string>[], Dictionary<Test.MyEnum, string>[])
        opMyEnumStringDS(Dictionary<Test.MyEnum, string>[] p1,
                         Dictionary<Test.MyEnum, string>[] p2,
                            Current current)
        {
            var p3 = new Dictionary<Test.MyEnum, string>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            var r = new Dictionary<Test.MyEnum, string>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<Test.MyStruct, Test.MyEnum>[], Dictionary<Test.MyStruct, Test.MyEnum>[])
        opMyStructMyEnumDS(Dictionary<Test.MyStruct, Test.MyEnum>[] p1,
            Dictionary<Test.MyStruct, Test.MyEnum>[] p2,
            Current current)
        {
            var p3 = new Dictionary<Test.MyStruct, Test.MyEnum>[p1.Length + p2.Length];
            Array.Copy(p2, p3, p2.Length);
            Array.Copy(p1, 0, p3, p2.Length, p1.Length);

            var r = new Dictionary<Test.MyStruct, Test.MyEnum>[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<byte, byte[]>, Dictionary<byte, byte[]>)
        opByteByteSD(Dictionary<byte, byte[]> p1,
            Dictionary<byte, byte[]> p2,
            Current current)
        {
            var r = new Dictionary<byte, byte[]>();
            foreach (var e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (var e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<bool, bool[]>, Dictionary<bool, bool[]>)
        opBoolBoolSD(Dictionary<bool, bool[]> p1, Dictionary<bool, bool[]> p2, Current current)
        {
            var r = new Dictionary<bool, bool[]>();
            foreach (KeyValuePair<bool, bool[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<bool, bool[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<short, short[]>, Dictionary<short, short[]>)
        opShortShortSD(Dictionary<short, short[]> p1, Dictionary<short, short[]> p2, Current current)
        {
            var r = new Dictionary<short, short[]>();
            foreach (KeyValuePair<short, short[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<short, short[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<int, int[]>, Dictionary<int, int[]>)
        opIntIntSD(Dictionary<int, int[]> p1,
            Dictionary<int, int[]> p2,
            Current current)
        {
            var r = new Dictionary<int, int[]>();
            foreach (KeyValuePair<int, int[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<int, int[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<long, long[]>, Dictionary<long, long[]>)
        opLongLongSD(Dictionary<long, long[]> p1, Dictionary<long, long[]> p2, Current current)
        {
            var r = new Dictionary<long, long[]>();
            foreach (KeyValuePair<long, long[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<long, long[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<string, float[]>, Dictionary<string, float[]>)
        opStringFloatSD(Dictionary<string, float[]> p1, Dictionary<string, float[]> p2, Current current)
        {
            var r = new Dictionary<string, float[]>();
            foreach (KeyValuePair<string, float[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<string, float[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<string, double[]>, Dictionary<string, double[]>)
        opStringDoubleSD(Dictionary<string, double[]> p1, Dictionary<string, double[]> p2, Current current)
        {
            var r = new Dictionary<string, double[]>();
            foreach (KeyValuePair<string, double[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<string, double[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<string, string[]>, Dictionary<string, string[]>)
        opStringStringSD(Dictionary<string, string[]> p1, Dictionary<string, string[]> p2, Current current)
        {
            var r = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, string[]> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<string, string[]> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public (Dictionary<Test.MyEnum, Test.MyEnum[]>, Dictionary<Test.MyEnum, Test.MyEnum[]>) opMyEnumMyEnumSD(
            Dictionary<Test.MyEnum, Test.MyEnum[]> p1,
            Dictionary<Test.MyEnum, Test.MyEnum[]> p2,
            Current ice)
        {
            var r = new Dictionary<Test.MyEnum, Test.MyEnum[]>();
            foreach (var e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (var e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p2);
        }

        public int[] opIntS(int[] s, Current current)
        {
            int[] r = new int[s.Length];
            for (int i = 0; i < s.Length; ++i)
            {
                r[i] = -s[i];
            }
            return r;
        }

        public void opByteSOneway(byte[] s, Current current)
        {
            lock (this)
            {
                ++_opByteSOnewayCallCount;
            }
        }

        public int opByteSOnewayCallCount(Current current)
        {
            lock (this)
            {
                int count = _opByteSOnewayCallCount;
                _opByteSOnewayCallCount = 0;
                return count;
            }
        }

        public Dictionary<string, string> opContext(Current current) =>
            current.Context == null ? new Dictionary<string, string>() : new Dictionary<string, string>(current.Context);

        public void opDoubleMarshaling(double p1, double[] p2, Current current)
        {
            double d = 1278312346.0 / 13.0;
            test(p1 == d);
            for (int i = 0; i < p2.Length; ++i)
            {
                test(p2[i] == d);
            }
        }

        public (string[], string[])
        opStringS(string[] p1, string[] p2, Current current)
        {
            var p3 = new string[p1.Length + p2.Length];
            Array.Copy(p1, p3, p1.Length);
            Array.Copy(p2, 0, p3, p1.Length, p2.Length);

            string[] r = new string[p1.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                r[i] = p1[p1.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (string[][], string[][])
        opStringSS(string[][] p1, string[][] p2, Current current)
        {
            var p3 = new string[p1.Length + p2.Length][];
            Array.Copy(p1, p3, p1.Length);
            Array.Copy(p2, 0, p3, p1.Length, p2.Length);

            string[][] r = new string[p2.Length][];
            for (int i = 0; i < p2.Length; i++)
            {
                r[i] = p2[p2.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (string[][][], string[][][])
        opStringSSS(string[][][] p1, string[][][] p2, Current current)
        {
            var p3 = new string[p1.Length + p2.Length][][];
            Array.Copy(p1, p3, p1.Length);
            Array.Copy(p2, 0, p3, p1.Length, p2.Length);

            string[][][] r = new string[p2.Length][][];
            for (int i = 0; i < p2.Length; i++)
            {
                r[i] = p2[p2.Length - (i + 1)];
            }
            return (r, p3);
        }

        public (Dictionary<string, string>, Dictionary<string, string>)
        opStringStringD(Dictionary<string, string> p1,
            Dictionary<string, string> p2,
            Current current)
        {
            var p3 = p1;
            Dictionary<string, string> r = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> e in p1)
            {
                r[e.Key] = e.Value;
            }
            foreach (KeyValuePair<string, string> e in p2)
            {
                r[e.Key] = e.Value;
            }
            return (r, p3);
        }

        public (Test.Structure, Test.Structure)
        opStruct(Test.Structure p1, Test.Structure p2, Current current)
        {
            var p3 = p1;
            p3.s.s = "a new string";
            return (p2, p3);
        }

        public void opIdempotent(Current current) => test(current.Mode == OperationMode.Idempotent);

        public void opNonmutating(Current current) => test(current.Mode == OperationMode.Nonmutating);

        public void opDerived(Current current)
        {
        }

        public byte opByte1(byte opByte1, Current current) => opByte1;

        public short opShort1(short opShort1, Current current) => opShort1;

        public int opInt1(int opInt1, Current current) => opInt1;

        public long opLong1(long opLong1, Current current) => opLong1;

        public float opFloat1(float opFloat1, Current current) => opFloat1;

        public double opDouble1(double opDouble1, Current current) => opDouble1;

        public string opString1(string opString1, Current current) => opString1;

        public string[] opStringS1(string[] opStringS1, Current current) => opStringS1;

        public Dictionary<byte, bool> opByteBoolD1(Dictionary<byte, bool> opByteBoolD1, Current current) => opByteBoolD1;

        public string[] opStringS2(string[] opStringS2, Current current) => opStringS2;

        public Dictionary<byte, bool>
        opByteBoolD2(Dictionary<byte, bool> opByteBoolD2, Current current) => opByteBoolD2;

        public Test.MyClass1 opMyClass1(Test.MyClass1 c, Current current) => c;

        public Test.MyStruct1 opMyStruct1(Test.MyStruct1 s, Current current) => s;

        public string[] opStringLiterals(Current current)
        {
            return new string[]
                {
                        Test.s0.value,
                        Test.s1.value,
                        Test.s2.value,
                        Test.s3.value,
                        Test.s4.value,
                        Test.s5.value,
                        Test.s6.value,
                        Test.s7.value,
                        Test.s8.value,
                        Test.s9.value,
                        Test.s10.value,

                        Test.sw0.value,
                        Test.sw1.value,
                        Test.sw2.value,
                        Test.sw3.value,
                        Test.sw4.value,
                        Test.sw5.value,
                        Test.sw6.value,
                        Test.sw7.value,
                        Test.sw8.value,
                        Test.sw9.value,
                        Test.sw10.value,

                        Test.ss0.value,
                        Test.ss1.value,
                        Test.ss2.value,
                        Test.ss3.value,
                        Test.ss4.value,
                        Test.ss5.value,

                        Test.su0.value,
                        Test.su1.value,
                        Test.su2.value
                };
        }

        public string[] opWStringLiterals(Current current) => opStringLiterals(current);

        public Test.IMyClass.OpMStruct1MarshaledReturnValue opMStruct1(Current current) =>
            new Test.IMyClass.OpMStruct1MarshaledReturnValue(
                new Test.Structure(null, Test.MyEnum.enum1, new Test.AnotherStruct("")), current);

        public Test.IMyClass.OpMStruct2MarshaledReturnValue opMStruct2(Test.Structure p1, Current current) =>
            new Test.IMyClass.OpMStruct2MarshaledReturnValue(p1, p1, current);

        public Test.IMyClass.OpMSeq1MarshaledReturnValue opMSeq1(Current current) =>
            new Test.IMyClass.OpMSeq1MarshaledReturnValue(new string[0], current);

        public Test.IMyClass.OpMSeq2MarshaledReturnValue opMSeq2(string[] p1, Current current) =>
            new Test.IMyClass.OpMSeq2MarshaledReturnValue(p1, p1, current);

        public Test.IMyClass.OpMDict1MarshaledReturnValue opMDict1(Current current) =>
            new Test.IMyClass.OpMDict1MarshaledReturnValue(new Dictionary<string, string>(), current);

        public Test.IMyClass.OpMDict2MarshaledReturnValue opMDict2(Dictionary<string, string> p1, Current current) =>
            new Test.IMyClass.OpMDict2MarshaledReturnValue(p1, p1, current);

        private int _opByteSOnewayCallCount = 0;
    }
}
