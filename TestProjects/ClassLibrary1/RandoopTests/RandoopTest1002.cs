using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arktos.WinBert.Instrumentation;

namespace RandoopTests
{
    public class RandoopTest1002
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
                var hello = 1;
                ((InterfaceTestAssembly2.Class1)v0).simpleTest(hello);
                TestUtil.RecordVoidInstanceMethodCall(v0, "simpleTest");

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
