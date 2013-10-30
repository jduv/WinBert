using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arktos.WinBert.Instrumentation;

namespace RandoopTests
{
    public class RandoopTest1001
    {
        public static int Main()
        {
            try
            {
                //BEGIN TEST
                InterfaceTestAssembly2.Class1 v0 = new InterfaceTestAssembly2.Class1();
                TestUtil.RecordVoidInstanceMethodCall(v0, ".ctor");
                System.String v1 = ((InterfaceTestAssembly2.Class1)v0).I1Boo();
                TestUtil.RecordInstanceMethodCall(v0, v1, "I1Boo");
                System.String v2 = ((InterfaceTestAssembly2.Class1)v0).I1Boo();
                TestUtil.RecordInstanceMethodCall(v0, v2, "I1Boo");
                ((InterfaceTestAssembly2.Class1)v0).I1Foo();
                TestUtil.RecordVoidInstanceMethodCall(v0, "I1Foo");
                System.String v4 = ((InterfaceTestAssembly2.Class1)v0).I1Boo();
                TestUtil.RecordInstanceMethodCall(v0, v4, "I1Boo");
                System.String v5 = ((InterfaceTestAssembly2.Class1)v0).I1Boo();
                TestUtil.RecordInstanceMethodCall(v0, v5, "I1Boo");
                System.DateTime v6 = ((InterfaceTestAssembly2.Class1)v0).I1Baz();
                TestUtil.RecordInstanceMethodCall(v0, v6, "I1Baz");
                int v7 = ((InterfaceTestAssembly2.Class1)v0).I1Bar();
                TestUtil.RecordInstanceMethodCall(v0, v7, "I1Baz");
                //END TEST
                System.Console.WriteLine("This was expected behavior. Will exit with code 100.");
                return 100;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("//EXCEPTION:" + e.GetType().FullName);
                System.Console.WriteLine("//STACK TRACE:");
                System.Console.WriteLine(e.StackTrace);
                System.Console.WriteLine();
                System.Console.WriteLine("This was unexpected behavior. Will exit with code 99.");
                return 99;
            }
        }
    }
}
