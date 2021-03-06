//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using System.Linq;
using System.Collections.Generic;
using Ice.optional.Test;

namespace Ice.optional
{
    public class AllTests : global::Test.AllTests
    {
        public static Test.IInitialPrx allTests(global::Test.TestHelper helper)
        {
            var communicator = helper.communicator();
            // FactoryI factory = new FactoryI();
            // communicator.getValueFactoryManager().add(factory.create, "");

            var output = helper.getWriter();
            var initial = Test.IInitialPrx.Parse($"initial:{helper.getTestEndpoint(0)}", communicator);

            output.Write("testing optional data members... ");
            output.Flush();

            var oo1 = new Test.OneOptional();
            test(!oo1.a.HasValue);
            oo1.a = 15;
            test(oo1.a.HasValue && oo1.a == 15);

            Test.OneOptional oo2 = new Test.OneOptional(16);
            test(oo2.a.HasValue && oo2.a == 16);

            Test.MultiOptional mo1 = new Test.MultiOptional();
            mo1.a = 15;
            mo1.b = true;
            mo1.c = 19;
            mo1.d = 78;
            mo1.e = 99;
            mo1.f = (float)5.5;
            mo1.g = 1.0;
            mo1.h = "test";
            mo1.i = Test.MyEnum.MyEnumMember;
            mo1.k = mo1;
            mo1.bs = new byte[] { 5 };
            mo1.ss = new string[] { "test", "test2" };
            mo1.iid = new Dictionary<int, int>();
            mo1.iid.Add(4, 3);
            mo1.sid = new Dictionary<string, int>();
            mo1.sid.Add("test", 10);
            Test.FixedStruct fs = new Test.FixedStruct();
            fs.m = 78;
            mo1.fs = fs;
            Test.VarStruct vs = new Test.VarStruct();
            vs.m = "hello";
            mo1.vs = vs;

            mo1.shs = new short[] { 1 };
            mo1.es = new Test.MyEnum[] { Test.MyEnum.MyEnumMember, Test.MyEnum.MyEnumMember };
            mo1.fss = new Test.FixedStruct[] { fs };
            mo1.vss = new Test.VarStruct[] { vs };
            mo1.oos = new Test.OneOptional[] { oo1 };

            mo1.ied = new Dictionary<int, Test.MyEnum>();
            mo1.ied.Add(4, Test.MyEnum.MyEnumMember);
            mo1.ifsd = new Dictionary<int, Test.FixedStruct>();
            mo1.ifsd.Add(4, fs);
            mo1.ivsd = new Dictionary<int, Test.VarStruct>();
            mo1.ivsd.Add(5, vs);
            mo1.iood = new Dictionary<int, Test.OneOptional>();
            mo1.iood.Add(5, new Test.OneOptional(15));

            mo1.bos = new bool[] { false, true, false };
            mo1.ser = new Test.SerializableClass(56);

            test(mo1.a == 15);
            test(mo1.b == true);
            test(mo1.c == 19);
            test(mo1.d == 78);
            test(mo1.e == 99);
            test(mo1.f == (float)5.5);
            test(mo1.g == 1.0);
            test(mo1.h.Equals("test"));
            test(mo1.i == Test.MyEnum.MyEnumMember);
            test(mo1.k == mo1);
            test(global::Test.Collections.Equals(mo1.bs, new byte[] { 5 }));
            test(global::Test.Collections.Equals(mo1.ss, new string[] { "test", "test2" }));
            test(mo1.iid[4] == 3);
            test(mo1.sid["test"] == 10);
            test(mo1.fs.Equals(new Test.FixedStruct(78)));
            test(mo1.vs.Equals(new Test.VarStruct("hello")));

            test(mo1.shs[0] == 1);
            test(mo1.es[0] == Test.MyEnum.MyEnumMember && mo1.es[1] == Test.MyEnum.MyEnumMember);
            test(mo1.fss[0].Equals(new Test.FixedStruct(78)));
            test(mo1.vss[0].Equals(new Test.VarStruct("hello")));
            test(mo1.oos[0] == oo1);

            test(mo1.ied[4] == Test.MyEnum.MyEnumMember);
            test(mo1.ifsd[4].Equals(new Test.FixedStruct(78)));
            test(mo1.ivsd[5].Equals(new Test.VarStruct("hello")));
            test(mo1.iood[5].a == 15);

            test(global::Test.Collections.Equals(mo1.bos, new bool[] { false, true, false }));
            test(mo1.ser.Equals(new Test.SerializableClass(56)));

            output.WriteLine("ok");

            output.Write("testing marshaling... ");
            output.Flush();

            Test.OneOptional oo4 = (Test.OneOptional)initial.pingPong(new Test.OneOptional());
            test(!oo4.a.HasValue);

            Test.OneOptional oo5 = (Test.OneOptional)initial.pingPong(oo1);
            test(oo1.a == oo5.a);

            Test.MultiOptional mo4 = (Test.MultiOptional)initial.pingPong(new Test.MultiOptional());
            test(mo4.a == null);
            test(mo4.b == null);
            test(mo4.c == null);
            test(mo4.d == null);
            test(mo4.e == null);
            test(mo4.f == null);
            test(mo4.g == null);
            test(mo4.h == null);
            test(mo4.i == null);
            test(mo4.k == null);
            test(mo4.bs == null);
            test(mo4.ss == null);
            test(mo4.iid == null);
            test(mo4.sid == null);
            test(mo4.fs == null);
            test(mo4.vs == null);

            test(mo4.shs == null);
            test(mo4.es == null);
            test(mo4.fss == null);
            test(mo4.vss == null);
            test(mo4.oos == null);

            test(mo4.ied == null);
            test(mo4.ifsd == null);
            test(mo4.ivsd == null);
            test(mo4.iood == null);

            test(mo4.bos == null);

            test(mo4.ser == null);

            bool supportsCsharpSerializable = initial.supportsCsharpSerializable();
            if (!supportsCsharpSerializable)
            {
                mo1.ser = null;
            }

            Test.MultiOptional mo5 = (Test.MultiOptional)initial.pingPong(mo1);
            test(mo5.a == mo1.a);
            test(mo5.b == mo1.b);
            test(mo5.c == mo1.c);
            test(mo5.d == mo1.d);
            test(mo5.e == mo1.e);
            test(mo5.f == mo1.f);
            test(mo5.g == mo1.g);
            test(mo5.h.Equals(mo1.h));
            test(mo5.i == mo1.i);
            test(mo5.k == mo5);
            test(global::Test.Collections.Equals(mo5.bs, mo1.bs));
            test(global::Test.Collections.Equals(mo5.ss, mo1.ss));
            test(mo5.iid[4] == 3);
            test(mo5.sid["test"] == 10);
            test(mo5.fs.Equals(mo1.fs));
            test(mo5.vs.Equals(mo1.vs));
            test(global::Test.Collections.Equals(mo5.shs, mo1.shs));
            test(mo5.es[0] == Test.MyEnum.MyEnumMember && mo1.es[1] == Test.MyEnum.MyEnumMember);
            test(mo5.fss[0].Equals(new Test.FixedStruct(78)));
            test(mo5.vss[0].Equals(new Test.VarStruct("hello")));
            test(mo5.oos[0].a == 15);

            test(mo5.ied[4] == Test.MyEnum.MyEnumMember);
            test(mo5.ifsd[4].Equals(new Test.FixedStruct(78)));
            test(mo5.ivsd[5].Equals(new Test.VarStruct("hello")));
            test(mo5.iood[5].a == 15);

            test(global::Test.Collections.Equals(mo5.bos, new bool[] { false, true, false }));
            if (supportsCsharpSerializable)
            {
                test(mo5.ser.Equals(new Test.SerializableClass(56)));
            }

            // Clear the first half of the optional members
            Test.MultiOptional mo6 = new Test.MultiOptional();
            mo6.b = mo5.b;
            mo6.d = mo5.d;
            mo6.f = mo5.f;
            mo6.h = mo5.h;
            mo6.bs = mo5.bs;
            mo6.iid = mo5.iid;
            mo6.fs = mo5.fs;
            mo6.shs = mo5.shs;
            mo6.fss = mo5.fss;
            mo6.oos = mo5.oos;
            mo6.ifsd = mo5.ifsd;
            mo6.iood = mo5.iood;
            mo6.bos = mo5.bos;

            Test.MultiOptional mo7 = (Test.MultiOptional)initial.pingPong(mo6);
            test(mo7.a == null);
            test(mo7.b.Equals(mo1.b));
            test(mo7.c == null);
            test(mo7.d.Equals(mo1.d));
            test(mo7.e == null);
            test(mo7.f.Equals(mo1.f));
            test(mo7.g == null);
            test(mo7.h.Equals(mo1.h));
            test(mo7.i == null);
            test(mo7.k == null);
            test(global::Test.Collections.Equals(mo7.bs, mo1.bs));
            test(mo7.ss == null);
            test(mo7.iid[4] == 3);
            test(mo7.sid == null);
            test(mo7.fs.Equals(mo1.fs));
            test(mo7.vs == null);

            test(global::Test.Collections.Equals(mo7.shs, mo1.shs));
            test(mo7.es == null);
            test(mo7.fss[0].Equals(new Test.FixedStruct(78)));
            test(mo7.vss == null);
            test(mo7.oos[0].a == 15);

            test(mo7.ied == null);
            test(mo7.ifsd[4].Equals(new Test.FixedStruct(78)));
            test(mo7.ivsd == null);
            test(mo7.iood[5].a == 15);

            test(global::Test.Collections.Equals(mo7.bos, new bool[] { false, true, false }));
            test(mo7.ser == null);

            // Clear the second half of the optional members
            Test.MultiOptional mo8 = new Test.MultiOptional();
            mo8.a = mo5.a;
            mo8.c = mo5.c;
            mo8.e = mo5.e;
            mo8.g = mo5.g;
            mo8.i = mo5.i;
            mo8.k = mo8;
            mo8.ss = mo5.ss;
            mo8.sid = mo5.sid;
            mo8.vs = mo5.vs;

            mo8.es = mo5.es;
            mo8.vss = mo5.vss;

            mo8.ied = mo5.ied;
            mo8.ivsd = mo5.ivsd;
            if (supportsCsharpSerializable)
            {
                mo8.ser = new Test.SerializableClass(56);
            }

            Test.MultiOptional mo9 = (Test.MultiOptional)initial.pingPong(mo8);
            test(mo9.a.Equals(mo1.a));
            test(!mo9.b.HasValue);
            test(mo9.c.Equals(mo1.c));
            test(!mo9.d.HasValue);
            test(mo9.e.Equals(mo1.e));
            test(!mo9.f.HasValue);
            test(mo9.g.Equals(mo1.g));
            test(mo9.h == null);
            test(mo9.i.Equals(mo1.i));
            test(mo9.k == mo9);
            test(mo9.bs == null);
            test(global::Test.Collections.Equals(mo9.ss, mo1.ss));
            test(mo9.iid == null);
            test(mo9.sid["test"] == 10);
            test(mo9.fs == null);
            test(mo9.vs.Equals(mo1.vs));

            test(mo9.shs == null);
            test(mo9.es[0] == Test.MyEnum.MyEnumMember && mo9.es[1] == Test.MyEnum.MyEnumMember);
            test(mo9.fss == null);
            test(mo9.vss[0].Equals(new Test.VarStruct("hello")));
            test(mo9.oos == null);

            test(mo9.ied[4] == Test.MyEnum.MyEnumMember);
            test(mo9.ifsd == null);
            test(mo9.ivsd[5].Equals(new Test.VarStruct("hello")));
            test(mo9.iood == null);

            test(mo9.bos == null);
            if (supportsCsharpSerializable)
            {
                test(mo9.ser.Equals(new Test.SerializableClass(56)));
            }

            {
                Test.OptionalWithCustom owc1 = new Test.OptionalWithCustom();
                owc1.l = new List<Test.SmallStruct>();
                owc1.l.Add(new Test.SmallStruct(5));
                owc1.l.Add(new Test.SmallStruct(6));
                owc1.l.Add(new Test.SmallStruct(7));
                owc1.s = new Test.ClassVarStruct(5);
                Test.OptionalWithCustom owc2 = (Test.OptionalWithCustom)initial.pingPong(owc1);
                test(owc2.l != null);
                test(global::Test.Collections.Equals(owc1.l, owc2.l));
                test(owc2.s != null && owc2.s.Value.a == 5);
            }

            /* TODO: rewrite test without factories

            //
            // Send a request using blobjects. Upon receival, we don't read
            // any of the optional members. This ensures the optional members
            // are skipped even if the receiver knows nothing about them.
            //
            factory.setEnabled(true);
            OutputStream os = new OutputStream(communicator);
            os.StartEncapsulation();
            os.WriteClass(oo1);
            os.EndEncapsulation();
            byte[] inEncaps = os.Finished();
            byte[] outEncaps;
            test(initial.Invoke("pingPong", OperationMode.Normal, inEncaps, out outEncaps));
            InputStream istr = new InputStream(communicator, outEncaps);
            istr.StartEncapsulation();
            ReadClassCallbackI cb = new ReadClassCallbackI();
            istr.ReadClass(cb.invoke);
            istr.EndEncapsulation();
            test(cb.obj != null && cb.obj is TestClassReader);

            os = new OutputStream(communicator);
            os.StartEncapsulation();
            os.WriteClass(mo1);
            os.EndEncapsulation();
            inEncaps = os.Finished();
            test(initial.Invoke("pingPong", OperationMode.Normal, inEncaps, out outEncaps));
            istr = new InputStream(communicator, outEncaps);
            istr.StartEncapsulation();
            istr.ReadClass(cb.invoke);
            istr.EndEncapsulation();
            test(cb.obj != null && cb.obj is TestClassReader);
            factory.setEnabled(false);
            */

            byte[] outEncaps;

            //
            // TODO: simplify test. It was using the 1.0 encoding with operations whose
            // only class parameters were optional.
            //
            Test.OneOptional? oo = new Test.OneOptional(53);
            initial.sendOptionalClass(true, oo);

            oo = initial.returnOptionalClass(true);
            test(oo != null);

            Test.Recursive[] recursive1 = new Test.Recursive[1];
            recursive1[0] = new Test.Recursive();
            Test.Recursive[] recursive2 = new Test.Recursive[1];
            recursive2[0] = new Test.Recursive();
            recursive1[0].value = recursive2;
            Test.Recursive outer = new Test.Recursive();
            outer.value = recursive1;
            initial.pingPong(outer);

            Test.G g = new Test.G();
            g.gg1Opt = new Test.G1("gg1Opt");
            g.gg2 = new Test.G2(10);
            g.gg2Opt = new Test.G2(20);
            g.gg1 = new Test.G1("gg1");
            g = initial.opG(g);
            test("gg1Opt".Equals(g.gg1Opt.a));
            test(10 == g.gg2.a);
            test(20 == g.gg2Opt.a);
            test("gg1".Equals(g.gg1.a));

            initial.opVoid();

            var ostr = new OutputStream(communicator);
            ostr.StartEncapsulation();
            ostr.WriteOptional(1, OptionalFormat.F4);
            ostr.WriteInt(15);
            ostr.WriteOptional(1, OptionalFormat.VSize);
            ostr.WriteString("test");
            ostr.EndEncapsulation();
            var inEncaps = ostr.Finished();
            test(initial.Invoke("opVoid", OperationMode.Normal, inEncaps, out outEncaps));

            output.WriteLine("ok");

            output.Write("testing marshaling of large containers with fixed size elements... ");
            output.Flush();
            Test.MultiOptional mc = new Test.MultiOptional();

            mc.bs = new byte[1000];
            mc.shs = new short[300];

            mc.fss = new Test.FixedStruct[300];
            for (int i = 0; i < 300; ++i)
            {
                mc.fss[i] = new Test.FixedStruct();
            }

            mc.ifsd = new Dictionary<int, Test.FixedStruct>();
            for (int i = 0; i < 300; ++i)
            {
                mc.ifsd.Add(i, new Test.FixedStruct());
            }

            mc = (Test.MultiOptional)initial.pingPong(mc);
            test(mc.bs.Length == 1000);
            test(mc.shs.Length == 300);
            test(mc.fss.Length == 300);
            test(mc.ifsd.Count == 300);

            /*
            factory.setEnabled(true);
            os = new OutputStream(communicator);
            os.StartEncapsulation();
            os.WriteClass(mc);
            os.EndEncapsulation();
            inEncaps = os.Finished();
            test(initial.Invoke("pingPong", OperationMode.Normal, inEncaps, out outEncaps));
            istr = new InputStream(communicator, outEncaps);
            istr.StartEncapsulation();
            istr.ReadClass(cb.invoke);
            istr.EndEncapsulation();
            test(cb.obj != null && cb.obj is TestClassReader);
            factory.setEnabled(false);
            */

            output.WriteLine("ok");

            output.Write("testing tag marshaling... ");
            output.Flush();
            {
                Test.B b = new Test.B();
                Test.B b2 = (Test.B)initial.pingPong(b);
                test(!b2.ma.HasValue);
                test(!b2.mb.HasValue);
                test(!b2.mc.HasValue);

                b.ma = 10;
                b.mb = 11;
                b.mc = 12;
                b.md = 13;

                b2 = (Test.B)initial.pingPong(b);
                test(b2.ma == 10);
                test(b2.mb == 11);
                test(b2.mc == 12);
                test(b2.md == 13);

                /*
                factory.setEnabled(true);
                os = new OutputStream(communicator);
                os.StartEncapsulation();
                os.WriteClass(b);
                os.EndEncapsulation();
                inEncaps = os.Finished();
                test(initial.Invoke("pingPong", OperationMode.Normal, inEncaps, out outEncaps));
                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.ReadClass(cb.invoke);
                istr.EndEncapsulation();
                test(cb.obj != null);
                factory.setEnabled(false);
                */

            }
            output.WriteLine("ok");

            output.Write("testing marshalling of objects with optional objects...");
            output.Flush();
            {
                Test.F f = new Test.F();

                f.af = new Test.A();
                f.ae = f.af;

                Test.F rf = (Test.F)initial.pingPong(f);
                test(rf.ae == rf.af);

                /*
                factory.setEnabled(true);
                os = new OutputStream(communicator);
                os.StartEncapsulation();
                os.WriteClass(f);
                os.EndEncapsulation();
                inEncaps = os.Finished();
                istr = new InputStream(communicator, inEncaps);
                istr.StartEncapsulation();
                ReadClassCallbackI rocb = new ReadClassCallbackI();
                istr.ReadClass(rocb.invoke);
                istr.EndEncapsulation();
                factory.setEnabled(false);
                rf = ((FClassReader)rocb.obj).getF();
                test(rf.ae != null && !rf.af.HasValue);
                */
            }
            output.WriteLine("ok");

            output.Write("testing optional with default values... ");
            output.Flush();
            {
                Test.WD wd = (Test.WD)initial.pingPong(new Test.WD());
                test(wd.a == 5);
                test(wd.s.Equals("test"));
                wd.a = null;
                wd.s = null;
                wd = (Test.WD)initial.pingPong(wd);
                test(wd.a == null);
                test(wd.s == null);
            }
            output.WriteLine("ok");

            if (communicator.GetPropertyAsInt("Default.SlicedFormat") > 0)
            {
                output.Write("testing marshaling with unknown class slices... ");
                output.Flush();
                {
                    Test.C c = new Test.C();
                    c.ss = "test";
                    c.ms = "testms";
                    ostr = new OutputStream(communicator);
                    ostr.StartEncapsulation();
                    ostr.WriteClass(c);
                    ostr.EndEncapsulation();
                    inEncaps = ostr.Finished();

                    /*
                    factory.setEnabled(true);
                    test(initial.Invoke("pingPong", OperationMode.Normal, inEncaps, out outEncaps));
                    istr = new InputStream(communicator, outEncaps);
                    istr.StartEncapsulation();
                    istr.ReadClass(cb.invoke);
                    istr.EndEncapsulation();
                    test(cb.obj is CClassReader);
                    factory.setEnabled(false);

                    factory.setEnabled(true);
                    os = new OutputStream(communicator);
                    os.StartEncapsulation();
                    AnyClass d = new DClassWriter();
                    os.WriteClass(d);
                    os.EndEncapsulation();
                    inEncaps = os.Finished();
                    test(initial.Invoke("pingPong", OperationMode.Normal, inEncaps, out outEncaps));
                    istr = new InputStream(communicator, outEncaps);
                    istr.StartEncapsulation();
                    istr.ReadClass(cb.invoke);
                    istr.EndEncapsulation();
                    test(cb.obj != null && cb.obj is DClassReader);
                    ((DClassReader)cb.obj).check();
                    factory.setEnabled(false);
                    */
                }
                output.WriteLine("ok");

                /*
                output.Write("testing optionals with unknown classes...");
                output.Flush();
                {
                    Test.A a = new Test.A();

                    os = new OutputStream(communicator);
                    os.StartEncapsulation();
                    os.WriteClass(a);
                    os.WriteOptional(1, OptionalFormat.Class);
                    os.WriteClass(new DClassWriter());
                    os.EndEncapsulation();
                    inEncaps = os.Finished();
                    test(initial.Invoke("opClassAndUnknownOptional", OperationMode.Normal, inEncaps,
                                            out outEncaps));

                    var istr = new InputStream(communicator, outEncaps);
                    istr.StartEncapsulation();
                    istr.EndEncapsulation();
                }
                output.WriteLine("ok");
                */
            }

            output.Write("testing optional parameters... ");
            output.Flush();
            {
                byte? p1 = null;
                var (p2, p3) = initial.opByte(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opByte(null);
                test(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.opByte(p1);
                test(p2 == 56 && p3 == 56);
                var r = initial.opByteAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.opByte(p1);
                test(p2 == 56 && p3 == 56);
                r = initial.opByteAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.opByte(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteByte(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opByte", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadByte(1) == 56);
                test(istr.ReadByte(3) == 56);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                bool? p1 = null;
                var (p2, p3) = initial.opBool(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opBool(null);
                test(p2 == null && p3 == null);

                p1 = true;
                (p2, p3) = initial.opBool(p1);
                test(p2 == true && p3 == true);
                var r = initial.opBoolAsync(p1).Result;
                test(r.ReturnValue == true && r.p3 == true);
                (p2, p3) = initial.opBool(true);
                test(p2 == true && p3 == true);
                r = initial.opBoolAsync(true).Result;
                test(r.ReturnValue == true && r.p3 == true);

                (p2, p3) = initial.opBool(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteBool(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opBool", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadBool(1) == true);
                test(istr.ReadBool(3) == true);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                short? p1 = null;
                var (p2, p3) = initial.opShort(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opShort(null);
                test(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.opShort(p1);
                test(p2 == 56 && p3 == 56);
                var r = initial.opShortAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.opShort(p1);
                test(p2 == 56 && p3 == 56);
                r = initial.opShortAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.opShort(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteShort(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opShort", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadShort(1) == 56);
                test(istr.ReadShort(3) == 56);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                int? p1 = null;
                var (p2, p3) = initial.opInt(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opInt(null);
                test(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.opInt(p1);
                test(p2 == 56 && p3 == 56);
                var r = initial.opIntAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.opInt(p1);
                test(p2 == 56 && p3 == 56);
                r = initial.opIntAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.opInt(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteInt(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opInt", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadInt(1) == 56);
                test(istr.ReadInt(3) == 56);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                long? p1 = null;
                var (p2, p3) = initial.opLong(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opLong(null);
                test(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.opLong(p1);
                test(p2 == 56 && p3 == 56);
                var r = initial.opLongAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.opLong(p1);
                test(p2 == 56 && p3 == 56);
                r = initial.opLongAsync(p1).Result;
                test(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.opLong(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteLong(1, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opLong", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadLong(2) == 56);
                test(istr.ReadLong(3) == 56);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                float? p1 = null;
                var (p2, p3) = initial.opFloat(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opFloat(null);
                test(p2 == null && p3 == null);

                p1 = 1.0f;
                (p2, p3) = initial.opFloat(p1);
                test(p2 == 1.0f && p3 == 1.0f);
                var r = initial.opFloatAsync(p1).Result;
                test(r.ReturnValue == 1.0f && r.p3 == 1.0f);
                (p2, p3) = initial.opFloat(p1);
                test(p2 == 1.0f && p3 == 1.0f);
                r = initial.opFloatAsync(p1).Result;
                test(r.ReturnValue == 1.0f && r.p3 == 1.0f);

                (p2, p3) = initial.opFloat(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteFloat(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opFloat", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadFloat(1) == 1.0f);
                test(istr.ReadFloat(3) == 1.0f);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                double? p1 = null;
                var (p2, p3) = initial.opDouble(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opDouble(null);
                test(p2 == null && p3 == null);

                p1 = 1.0;
                (p2, p3) = initial.opDouble(p1);
                test(p2 == 1.0 && p3 == 1.0);
                var r = initial.opDoubleAsync(p1).Result;
                test(r.ReturnValue == 1.0 && r.p3 == 1.0);
                (p2, p3) = initial.opDouble(p1);
                test(p2 == 1.0 && p3 == 1.0);
                r = initial.opDoubleAsync(p1).Result;
                test(r.ReturnValue == 1.0 && r.p3 == 1.0);

                (p2, p3) = initial.opDouble(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteDouble(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opDouble", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadDouble(1) == 1.0);
                test(istr.ReadDouble(3) == 1.0);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                string? p1 = null;
                var (p2, p3) = initial.opString(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opString(null);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opString(null); // Implicitly converts to string>(null)
                test(p2 == null && p3 == null);

                p1 = "test";
                (p2, p3) = initial.opString(p1);
                test(p2 == "test" && p3 == "test");
                var r = initial.opStringAsync(p1).Result;
                test(r.ReturnValue == "test" && r.p3 == "test");
                (p2, p3) = initial.opString(p1);
                test(p2 == "test" && p3 == "test");
                r = initial.opStringAsync(p1).Result;
                test(r.ReturnValue == "test" && r.p3 == "test");

                (p2, p3) = initial.opString(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteString(2, p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opString", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadString(1) == "test");
                test(istr.ReadString(3) == "test");
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.MyEnum? p1 = null;
                var (p2, p3) = initial.opMyEnum(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opMyEnum(null);
                test(p2 == null && p3 == null);

                p1 = Test.MyEnum.MyEnumMember;
                (p2, p3) = initial.opMyEnum(p1);
                test(p2 == Test.MyEnum.MyEnumMember && p3 == Test.MyEnum.MyEnumMember);
                var r = initial.opMyEnumAsync(p1).Result;
                test(r.ReturnValue == Test.MyEnum.MyEnumMember && r.p3 == Test.MyEnum.MyEnumMember);
                (p2, p3) = initial.opMyEnum(p1);
                test(p2 == Test.MyEnum.MyEnumMember && p3 == Test.MyEnum.MyEnumMember);
                r = initial.opMyEnumAsync(p1).Result;
                test(r.ReturnValue == Test.MyEnum.MyEnumMember && r.p3 == Test.MyEnum.MyEnumMember);

                (p2, p3) = initial.opMyEnum(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteEnum(2, (int?)p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opMyEnum", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.Size));
                test((Test.MyEnum)istr.ReadEnum(1) == Test.MyEnum.MyEnumMember);
                test(istr.ReadOptional(3, OptionalFormat.Size));
                test((Test.MyEnum)istr.ReadEnum(1) == Test.MyEnum.MyEnumMember);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.SmallStruct? p1 = null;
                var (p2, p3) = initial.opSmallStruct(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opSmallStruct(null);
                test(p2 == null && p3 == null);

                p1 = new Test.SmallStruct(56);
                (p2, p3) = initial.opSmallStruct(p1);
                test(p2!.Value.m == 56 && p3!.Value.m == 56);
                var r = initial.opSmallStructAsync(p1).Result;
                test(p2!.Value.m == 56 && p3!.Value.m == 56);
                (p2, p3) = initial.opSmallStruct(p1);
                test(p2!.Value.m == 56 && p3!.Value.m == 56);
                r = initial.opSmallStructAsync(p1).Result;
                test(r.ReturnValue!.Value.m == 56 && r.p3!.Value.m == 56);

                (p2, p3) = initial.opSmallStruct(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(1);
                p1.Value.IceWrite(ostr);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opSmallStruct", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                Test.SmallStruct f = new Test.SmallStruct(istr);
                test(f.m == 56);
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                f = new Test.SmallStruct(istr);
                test(f.m == 56);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.FixedStruct? p1 = null;
                var (p2, p3) = initial.opFixedStruct(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opFixedStruct(null);
                test(p2 == null && p3 == null);

                p1 = new Test.FixedStruct(56);
                (p2, p3) = initial.opFixedStruct(p1);
                test(p2!.Value.m == 56 && p3!.Value.m == 56);
                var r = initial.opFixedStructAsync(p1).Result;
                test(r.ReturnValue!.Value.m == 56 && r.p3!.Value.m == 56);
                (p2, p3) = initial.opFixedStruct(p1);
                test(p2!.Value.m == 56 && p3!.Value.m == 56);
                r = initial.opFixedStructAsync(p1).Result;
                test(r.ReturnValue!.Value.m == 56 && r.p3!.Value.m == 56);

                (p2, p3) = initial.opFixedStruct(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(4);
                p1.Value.IceWrite(ostr);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opFixedStruct", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                Test.FixedStruct f = new Test.FixedStruct(istr);
                test(f.m == 56);
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                f = new Test.FixedStruct(istr);
                test(f.m == 56);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.VarStruct? p1 = null;
                var (p2, p3) = initial.opVarStruct(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opVarStruct(null);
                test(p2 == null && p3 == null);

                p1 = new Test.VarStruct("test");
                (p2, p3) = initial.opVarStruct(p1);
                test(p2!.Value.m.Equals("test") && p3!.Value.m.Equals("test"));

                // Test null struct
                (p2, p3) = initial.opVarStruct(null);
                test(p2 == null && p3 == null);

                var r = initial.opVarStructAsync(p1).Result;
                test(r.ReturnValue!.Value.m.Equals("test") && r.p3!.Value.m.Equals("test"));
                (p2, p3) = initial.opVarStruct(p1);
                test(p2!.Value.m.Equals("test") && p3!.Value.m.Equals("test"));
                r = initial.opVarStructAsync(p1).Result;
                test(r.ReturnValue!.Value.m.Equals("test") && r.p3!.Value.m.Equals("test"));

                (p2, p3) = initial.opVarStruct(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.FSize);
                int pos = ostr.StartSize();
                p1.Value.IceWrite(ostr);
                ostr.EndSize(pos);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opVarStruct", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.FSize));
                istr.Skip(4);
                Test.VarStruct v = new Test.VarStruct(istr);
                test(v.m.Equals("test"));
                test(istr.ReadOptional(3, OptionalFormat.FSize));
                istr.Skip(4);
                v = new Test.VarStruct(istr);
                test(v.m.Equals("test"));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.OneOptional? p1 = null;
                var (p2, p3) = initial.opOneOptional(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opOneOptional(null);
                test(p2 == null && p3 == null);

                p1 = new Test.OneOptional(58);
                (p2, p3) = initial.opOneOptional(p1);
                test(p2!.a == 58 && p3!.a == 58);
                var r = initial.opOneOptionalAsync(p1).Result;
                test(r.ReturnValue!.a == 58 && r.p3!.a == 58);
                (p2, p3) = initial.opOneOptional(p1);
                test(p2!.a == 58 && p3!.a == 58);
                r = initial.opOneOptionalAsync(p1).Result;
                test(r.ReturnValue!.a == 58 && r.p3!.a == 58);

                (p2, p3) = initial.opOneOptional(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.Class);
                ostr.WriteClass(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opOneOptional", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.Class));
                var p2c = istr.ReadClass<Test.OneOptional>();
                test(istr.ReadOptional(3, OptionalFormat.Class));
                var p3c = istr.ReadClass<Test.OneOptional>();
                istr.EndEncapsulation();
                test(p2c.a == 58 && p3c.a == 58);

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                IObjectPrx? p1 = null;
                var (p2, p3) = initial.opOneOptionalProxy(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opOneOptionalProxy(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opOneOptionalProxy(null);
                test(p2 == null && p3 == null);

                p1 = IObjectPrx.Parse("test", communicator);
                (p2, p3) = initial.opOneOptionalProxy(p1);
                test(IObjectPrx.Equals(p1, p2) && IObjectPrx.Equals(p1, p3));

                (p2, p3) = initial.opOneOptionalProxyAsync(p1).Result;
                test(IObjectPrx.Equals(p1, p2) && IObjectPrx.Equals(p1, p3));

                (p2, p3) = initial.opOneOptionalProxy(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.FSize);
                int pos = ostr.StartSize();
                ostr.WriteProxy(p1);
                ostr.EndSize(pos);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opOneOptionalProxy", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(IObjectPrx.Equals(istr.ReadProxy(1, IObjectPrx.Factory), p1));
                test(IObjectPrx.Equals(istr.ReadProxy(3, IObjectPrx.Factory), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                byte[]? p1 = null;
                var (p2, p3) = initial.opByteSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opByteSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(x => (byte)56).ToArray();
                (p2, p3) = initial.opByteSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opByteSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opByteSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opByteSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opByteSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteByteSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opByteSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                test(global::Test.Collections.Equals(istr.ReadByteArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                test(global::Test.Collections.Equals(istr.ReadByteArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                bool[]? p1 = null;
                var (p2, p3) = initial.opBoolSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opBoolSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => true).ToArray();
                (p2, p3) = initial.opBoolSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opBoolSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opBoolSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opBoolSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opBoolSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteBoolSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opBoolSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                test(global::Test.Collections.Equals(istr.ReadBoolArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                test(global::Test.Collections.Equals(istr.ReadBoolArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                short[]? p1 = null;
                var (p2, p3) = initial.opShortSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opShortSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => (short)56).ToArray();
                (p2, p3) = initial.opShortSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opShortSeqAsync(p1).Result;

                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opShortSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opShortSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opShortSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length * 2 + (p1.Length > 254 ? 5 : 1));
                ostr.WriteShortSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opShortSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadShortArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadShortArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                int[]? p1 = null;
                var (p2, p3) = initial.opIntSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opIntSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 56).ToArray();
                (p2, p3) = initial.opIntSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opIntSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opIntSeq(p1);
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                r = initial.opIntSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opIntSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length * 4 + (p1.Length > 254 ? 5 : 1));
                ostr.WriteIntSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opIntSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadIntArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadIntArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                long[]? p1 = null;
                var (p2, p3) = initial.opLongSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opLongSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 56L).ToArray();
                (p2, p3) = initial.opLongSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opLongSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opLongSeq(p1);
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                r = initial.opLongSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opLongSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length * 8 + (p1.Length > 254 ? 5 : 1));
                ostr.WriteLongSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opLongSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadLongArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadLongArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                float[]? p1 = null;
                var (p2, p3) = initial.opFloatSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opFloatSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 1.0f).ToArray();
                (p2, p3) = initial.opFloatSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opFloatSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opFloatSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opFloatSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opFloatSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length * 4 + (p1.Length > 254 ? 5 : 1));
                ostr.WriteFloatSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opFloatSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadFloatArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadFloatArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                double[]? p1 = null;
                var (p2, p3) = initial.opDoubleSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opDoubleSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 1.0).ToArray();
                (p2, p3) = initial.opDoubleSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opDoubleSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opDoubleSeq(p1);
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                r = initial.opDoubleSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opDoubleSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length * 8 + (p1.Length > 254 ? 5 : 1));
                ostr.WriteDoubleSeq(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opDoubleSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadDoubleArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                test(global::Test.Collections.Equals(istr.ReadDoubleArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                string[]? p1 = null;
                var (p2, p3) = initial.opStringSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opStringSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => "test1").ToArray();
                (p2, p3) = initial.opStringSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opStringSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opStringSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opStringSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opStringSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.FSize);
                int pos = ostr.StartSize();
                ostr.WriteStringSeq(p1);
                ostr.EndSize(pos);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opStringSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.FSize));
                istr.Skip(4);
                test(global::Test.Collections.Equals(istr.ReadStringArray(), p1));
                test(istr.ReadOptional(3, OptionalFormat.FSize));
                istr.Skip(4);
                test(global::Test.Collections.Equals(istr.ReadStringArray(), p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.SmallStruct[]? p1 = null;
                var (p2, p3) = initial.opSmallStructSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opSmallStructSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => new Test.SmallStruct()).ToArray();
                (p2, p3) = initial.opSmallStructSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opSmallStructSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opSmallStructSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opSmallStructSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opSmallStructSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length + (p1.Length > 254 ? 5 : 1));
                ostr.Write(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opSmallStructSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                SmallStruct[] arr = istr.ReadSmallStructSeq();
                test(global::Test.Collections.Equals(arr, p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                arr = istr.ReadSmallStructSeq();
                test(global::Test.Collections.Equals(arr, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                List<SmallStruct>? p1 = null;
                var (p2, p3) = initial.opSmallStructList(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opSmallStructList(null);
                test(p2 == null && p3 == null);

                p1 = new List<SmallStruct>();
                for (int i = 0; i < 10; ++i)
                {
                    p1.Add(new SmallStruct());
                }
                (p2, p3) = initial.opSmallStructList(p1);
                test(global::Test.Collections.Equals(p2, p1));
                var r = initial.opSmallStructListAsync(p1).Result;
                test(global::Test.Collections.Equals(p2, p1));
                (p2, p3) = initial.opSmallStructList(p1);
                test(global::Test.Collections.Equals(p2, p1));
                r = initial.opSmallStructListAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1));

                (p2, p3) = initial.opSmallStructList(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Count + (p1.Count > 254 ? 5 : 1));
                ostr.Write(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opSmallStructList", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                List<SmallStruct> arr = istr.ReadSmallStructList();
                test(global::Test.Collections.Equals(arr, p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                arr = istr.ReadSmallStructList();
                test(global::Test.Collections.Equals(arr, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.FixedStruct[]? p1 = null;
                var (p2, p3) = initial.opFixedStructSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opFixedStructSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => new Test.FixedStruct()).ToArray();
                (p2, p3) = initial.opFixedStructSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opFixedStructSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opFixedStructSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opFixedStructSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opFixedStructSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Length * 4 + (p1.Length > 254 ? 5 : 1));
                ostr.Write(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opFixedStructSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                Test.FixedStruct[] arr = istr.ReadFixedStructSeq();
                test(global::Test.Collections.Equals(arr, p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                arr = istr.ReadFixedStructSeq();
                test(global::Test.Collections.Equals(arr, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                LinkedList<Test.FixedStruct>? p1 = null;
                var (p2, p3) = initial.opFixedStructList(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opFixedStructList(null);
                test(p2 == null && p3 == null);

                p1 = new LinkedList<Test.FixedStruct>();
                for (int i = 0; i < 10; ++i)
                {
                    p1.AddLast(new Test.FixedStruct());
                }
                (p2, p3) = initial.opFixedStructList(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opFixedStructListAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opFixedStructList(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opFixedStructListAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opFixedStructList(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Count * 4 + (p1.Count > 254 ? 5 : 1));
                ostr.Write(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opFixedStructList", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                var arr = istr.ReadFixedStructList();
                test(global::Test.Collections.Equals(arr, p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                arr = istr.ReadFixedStructList();
                test(global::Test.Collections.Equals(arr, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Test.VarStruct[]? p1 = null;
                var (p2, p3) = initial.opVarStructSeq(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opVarStructSeq(null);
                test(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => new Test.VarStruct("")).ToArray();
                (p2, p3) = initial.opVarStructSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opVarStructSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opVarStructSeq(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opVarStructSeqAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opVarStructSeq(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.FSize);
                int pos = ostr.StartSize();
                ostr.Write(p1);
                ostr.EndSize(pos);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opVarStructSeq", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.FSize));
                istr.Skip(4);
                VarStruct[] arr = istr.ReadVarStructSeq();
                test(global::Test.Collections.Equals(arr, p1));
                test(istr.ReadOptional(3, OptionalFormat.FSize));
                istr.Skip(4);
                arr = istr.ReadVarStructSeq();
                test(global::Test.Collections.Equals(arr, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            if (supportsCsharpSerializable)
            {
                Test.SerializableClass? p1 = null;
                var (p2, p3) = initial.opSerializable(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opSerializable(null);
                test(p2 == null && p3 == null);

                p1 = new Test.SerializableClass(58);
                (p2, p3) = initial.opSerializable(p1);
                test(p2.Equals(p1) && p3.Equals(p1));
                var r = initial.opSerializableAsync(p1).Result;
                test(r.ReturnValue.Equals(p1) && r.p3.Equals(p1));
                (p2, p3) = initial.opSerializable(p1);
                test(p2.Equals(p1) && p3.Equals(p1));
                r = initial.opSerializableAsync(p1).Result;
                test(r.ReturnValue.Equals(p1) && r.p3.Equals(p1));

                (p2, p3) = initial.opSerializable(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSerializable(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opSerializable", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                var sc = (Test.SerializableClass) istr.ReadSerializable();
                test(sc.Equals(p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                sc = (Test.SerializableClass)istr.ReadSerializable();
                test(sc.Equals(p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Dictionary<int, int>? p1 = null;
                var (p2, p3) = initial.opIntIntDict(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opIntIntDict(null);
                test(p2 == null && p3 == null);

                p1 = new Dictionary<int, int>();
                p1.Add(1, 2);
                p1.Add(2, 3);
                (p2, p3) = initial.opIntIntDict(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opIntIntDictAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opIntIntDict(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opIntIntDictAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opIntIntDict(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.VSize);
                ostr.WriteSize(p1.Count * 8 + (p1.Count > 254 ? 5 : 1));
                ostr.Write(p1);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opIntIntDict", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.VSize));
                istr.SkipSize();
                Dictionary<int, int> m = istr.ReadIntIntDict();
                test(global::Test.Collections.Equals(m, p1));
                test(istr.ReadOptional(3, OptionalFormat.VSize));
                istr.SkipSize();
                m = istr.ReadIntIntDict();
                test(global::Test.Collections.Equals(m, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }

            {
                Dictionary<string, int>? p1 = null;
                var (p2, p3) = initial.opStringIntDict(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opStringIntDict(null);
                test(p2 == null && p3 == null);

                p1 = new Dictionary<string, int>();
                p1.Add("1", 1);
                p1.Add("2", 2);
                (p2, p3) = initial.opStringIntDict(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                var r = initial.opStringIntDictAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));
                (p2, p3) = initial.opStringIntDict(p1);
                test(global::Test.Collections.Equals(p2, p1) && global::Test.Collections.Equals(p3, p1));
                r = initial.opStringIntDictAsync(p1).Result;
                test(global::Test.Collections.Equals(r.ReturnValue, p1) && global::Test.Collections.Equals(r.p3, p1));

                (p2, p3) = initial.opStringIntDict(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.FSize);
                int pos = ostr.StartSize();
                ostr.Write(p1);
                ostr.EndSize(pos);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opStringIntDict", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.FSize));
                istr.Skip(4);
                Dictionary<string, int> m = istr.ReadStringIntDict();
                test(global::Test.Collections.Equals(m, p1));
                test(istr.ReadOptional(3, OptionalFormat.FSize));
                istr.Skip(4);
                m = istr.ReadStringIntDict();
                test(global::Test.Collections.Equals(m, p1));
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();

                Test.F f = new Test.F();
                f.af = new Test.A();
                f.af.requiredA = 56;
                f.ae = f.af;

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(1, OptionalFormat.Class);
                ostr.WriteClass(f);
                ostr.WriteOptional(2, OptionalFormat.Class);
                ostr.WriteClass(f.ae);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();

                istr = new InputStream(communicator, inEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(2, OptionalFormat.Class));
                var a = istr.ReadClass<Test.A>();
                istr.EndEncapsulation();
                test(a != null && a.requiredA == 56);
            }

            {
                Dictionary<int, Test.OneOptional>? p1 = null;
                var (p2, p3) = initial.opIntOneOptionalDict(p1);
                test(p2 == null && p3 == null);
                (p2, p3) = initial.opIntOneOptionalDict(null);
                test(p2 == null && p3 == null);

                p1 = new Dictionary<int, Test.OneOptional>();
                p1.Add(1, new Test.OneOptional(58));
                p1.Add(2, new Test.OneOptional(59));
                (p2, p3) = initial.opIntOneOptionalDict(p1);
                test(p2[1].a == 58 && p3[1].a == 58);
                var r = initial.opIntOneOptionalDictAsync(p1).Result;
                test(r.ReturnValue[1].a == 58 && r.p3[1].a == 58);
                (p2, p3) = initial.opIntOneOptionalDict(p1);
                test(p2[1].a == 58 && p3[1].a == 58);
                r = initial.opIntOneOptionalDictAsync(p1).Result;
                test(r.ReturnValue[1].a == 58 && r.p3[1].a == 58);

                (p2, p3) = initial.opIntOneOptionalDict(null);
                test(p2 == null && p3 == null); // Ensure out parameter is cleared.

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteOptional(2, OptionalFormat.FSize);
                int pos = ostr.StartSize();
                ostr.Write(p1);
                ostr.EndSize(pos);
                ostr.EndEncapsulation();
                inEncaps = ostr.Finished();
                initial.Invoke("opIntOneOptionalDict", OperationMode.Normal, inEncaps, out outEncaps);
                var istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                test(istr.ReadOptional(1, OptionalFormat.FSize));
                istr.Skip(4);
                Dictionary<int, OneOptional> m = istr.ReadIntOneOptionalDict();
                test(m[1].a == 58);
                test(istr.ReadOptional(3, OptionalFormat.FSize));
                istr.Skip(4);
                m = istr.ReadIntOneOptionalDict();
                test(m[1].a == 58);
                istr.EndEncapsulation();

                istr = new InputStream(communicator, outEncaps);
                istr.StartEncapsulation();
                istr.EndEncapsulation();
            }
            output.WriteLine("ok");

            output.Write("testing exception optionals... ");
            output.Flush();
            {
                try
                {
                    int? a = null;
                    string? b = null;
                    Test.OneOptional? o = null;
                    initial.opOptionalException(a, b, o);
                }
                catch (Test.OptionalException ex)
                {
                    test(ex.a == null);
                    test(ex.b == null);
                    test(ex.o == null);
                }

                try
                {
                    int? a = 30;
                    string? b = "test";
                    Test.OneOptional? o = new Test.OneOptional(53);
                    initial.opOptionalException(a, b, o);
                }
                catch (Test.OptionalException ex)
                {
                    test(ex.a == 30);
                    test(ex.b == "test");
                    test(ex.o!.a == 53);
                }

                try
                {
                    int? a = null;
                    string? b = null;
                    Test.OneOptional? o = null;
                    initial.opDerivedException(a, b, o);
                }
                catch (Test.DerivedException ex)
                {
                    test(ex.a == null);
                    test(ex.b == null);
                    test(ex.o == null);
                    test(ex.ss == null);
                    test(ex.o2 == null);
                }

                try
                {
                    int? a = 30;
                    string? b = "test2";
                    Test.OneOptional? o = new Test.OneOptional(53);
                    initial.opDerivedException(a, b, o);
                }
                catch (Test.DerivedException ex)
                {
                    test(ex.a == 30);
                    test(ex.b == "test2");
                    test(ex.o!.a == 53);
                    test(ex.ss == "test2");
                    test(ex.o2!.a == 53);
                }

                try
                {
                    int? a = null;
                    string? b = null;
                    Test.OneOptional? o = null;
                    initial.opRequiredException(a, b, o);
                }
                catch (Test.RequiredException ex)
                {
                    test(ex.a == null);
                    test(ex.b == null);
                    test(ex.o == null);
                    test(ex.ss == "test");
                    test(ex.o2 == null);
                }

                try
                {
                    int? a = 30;
                    string? b = "test2";
                    Test.OneOptional? o = new Test.OneOptional(53);
                    initial.opRequiredException(a, b, o);
                }
                catch (Test.RequiredException ex)
                {
                    test(ex.a == 30);
                    test(ex.b == "test2");
                    test(ex.o!.a == 53);
                    test(ex.ss == "test2");
                    test(ex.o2!.a == 53);
                }
            }
            output.WriteLine("ok");

            output.Write("testing optionals with marshaled results... ");
            output.Flush();
            {
                test(initial.opMStruct1() != null);
                test(initial.opMDict1() != null);
                test(initial.opMSeq1() != null);
                test(initial.opMG1() != null);

                {
                    Test.SmallStruct? p1, p2, p3;
                    (p3, p2) = initial.opMStruct2(null);
                    test(p2 == null && p3 == null);

                    p1 = new Test.SmallStruct();
                    (p3, p2) = initial.opMStruct2(p1);
                    test(p2.Equals(p1) && p3.Equals(p1));
                }
                {
                    string[]? p1, p2, p3;
                    (p3, p2) = initial.opMSeq2(null);
                    test(p2 == null && p3 == null);

                    p1 = new string[1] { "hello" };
                    (p3, p2) = initial.opMSeq2(p1);
                    test(global::Test.Collections.Equals(p2, p1) &&
                            global::Test.Collections.Equals(p3, p1));
                }
                {
                    Dictionary<string, int>? p1, p2, p3;
                    (p3, p2) = initial.opMDict2(null);
                    test(p2 == null && p3 == null);

                    p1 = new Dictionary<string, int>();
                    p1["test"] = 54;
                    (p3, p2) = initial.opMDict2(p1);
                    test(global::Test.Collections.Equals(p2, p1) &&
                            global::Test.Collections.Equals(p3, p1));
                }
                {
                    Test.G? p1, p2, p3;
                    (p3, p2) = initial.opMG2(null);
                    test(p2 == null && p3 == null);

                    p1 = new Test.G();
                    (p3, p2) = initial.opMG2(p1);
                    test(p2 != null && p3 != null && p3 == p2);
                }
            }
            output.WriteLine("ok");

            return initial;
        }

        /*
        private class DClassWriter : ClassWriter
        {
            public override string ice_id()
            {
                return "";
            }

            public override void write(OutputStream @out)
            {
                @out.StartClass(null);
                // ::Test::D
                @out.StartSlice("::Test::D", true);
                string s = "test";
                @out.WriteString(s);
                @out.WriteOptional(1, OptionalFormat.FSize);
                string[] o = { "test1", "test2", "test3", "test4" };
                int pos = @out.StartSize();
                @out.WriteStringSeq(o);
                @out.EndSize(pos);
                Test.A a = new Test.A();
                a.mc = 18;
                @out.WriteOptional(1000, OptionalFormat.Class);
                @out.WriteClass(a);
                @out.EndSlice(false);
                // ::Test::B
                @out.StartSlice(Test.B.ice_staticId(), false);
                int v = 14;
                @out.WriteInt(v);
                @out.EndSlice(false);
                // ::Test::A
                @out.StartSlice(Test.A.ice_staticId(), false);
                @out.WriteInt(v);
                @out.EndSlice(true);
                @out.EndClass();
            }
        }

        private class DClassReader : ClassReader
        {
            public override void read(InputStream istr)
            {
                istr.startClass();
                // ::Test::D
                istr.startSlice();
                string s = istr.ReadString();
                test(s.Equals("test"));
                test(istr.ReadOptional(1, OptionalFormat.FSize));
                istr.skip(4);
                string[] o = istr.ReadStringSeq();
                test(o.Length == 4 &&
                        o[0].Equals("test1") && o[1].Equals("test2") && o[2].Equals("test3") && o[3].Equals("test4"));
                test(istr.ReadOptional(1000, OptionalFormat.Class));
                istr.ReadClass(a.invoke);
                istr.endSlice();
                // ::Test::B
                istr.startSlice();
                istr.ReadInt();
                istr.endSlice();
                // ::Test::A
                istr.startSlice();
                istr.ReadInt();
                istr.endSlice();
                istr.endClass(false);
            }

            internal void check()
            {
                test(((Test.A)a.obj).mc == 18);
            }

            private ReadClassCallbackI a = new ReadClassCallbackI();
        }

        private class FClassReader : ClassReader
        {
            public override void read(InputStream istr)
            {
                _f = new Test.F();
                istr.startClass();
                istr.startSlice();
                // Don't read af on purpose
                //in.Read(1, _f.af);
                istr.endSlice();
                istr.startSlice();
                ReadClassCallbackI rocb = new ReadClassCallbackI();
                istr.ReadClass(rocb.invoke);
                istr.endSlice();
                istr.endClass(false);
                _f.ae = (Test.A)rocb.obj;
            }

            public Test.F getF()
            {
                return _f;
            }

            private Test.F _f;
        }

        private class FactoryI
        {
            public AnyClass create(string typeId)
            {
                if (!_enabled)
                {
                    return null;
                }

                if (typeId.Equals(Test.OneOptional.ice_staticId()))
                {
                    return new TestClassReader();
                }
                else if (typeId.Equals(Test.MultiOptional.ice_staticId()))
                {
                    return new TestClassReader();
                }
                else if (typeId.Equals(Test.B.ice_staticId()))
                {
                    return new BClassReader();
                }
                else if (typeId.Equals(Test.C.ice_staticId()))
                {
                    return new CClassReader();
                }
                else if (typeId.Equals("::Test::D"))
                {
                    return new DClassReader();
                }
                else if (typeId.Equals("::Test::F"))
                {
                    return new FClassReader();
                }

                return null;
            }

            internal void setEnabled(bool enabled)
            {
                _enabled = enabled;
            }

            private bool _enabled;
        }
        */
    }
}
