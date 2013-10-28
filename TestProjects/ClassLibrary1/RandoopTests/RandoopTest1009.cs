using Arktos.WinBert.Instrumentation;
using InterfaceTestAssembly2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoopTests
{
    class RandoopTest1009
    {
        // RandoopTest1009
        public static int Main()
        {
            TestUtil.StartTest();
            int num;
            try
            {
                Class1 @class = new Class1();
                TestUtil.RecordVoidInstanceMethodCall(@class, ".ctor");
                DateTime target = @class.I1Baz();
                TestUtil.RecordVoidInstanceMethodCall(target, "InterfaceTestAssembly2.Class1.I1Baz");
                DateTime target2 = @class.I1Baz();
                TestUtil.RecordVoidInstanceMethodCall(target2, "InterfaceTestAssembly2.Class1.I1Baz");
                DateTime target3 = @class.I1Baz();
                TestUtil.RecordVoidInstanceMethodCall(target3, "InterfaceTestAssembly2.Class1.I1Baz");
                int target4 = @class.I1Bar();
                TestUtil.RecordVoidInstanceMethodCall(target4, "InterfaceTestAssembly2.Class1.I1Bar");
                int target5 = @class.I1Bar();
                TestUtil.RecordVoidInstanceMethodCall(target5, "InterfaceTestAssembly2.Class1.I1Bar");
                DateTime target6 = @class.I1Baz();
                TestUtil.RecordVoidInstanceMethodCall(target6, "InterfaceTestAssembly2.Class1.I1Baz");
                DateTime target7 = @class.I1Baz();
                TestUtil.RecordVoidInstanceMethodCall(target7, "InterfaceTestAssembly2.Class1.I1Baz");
                DateTime target8 = @class.I1Baz();
                TestUtil.RecordVoidInstanceMethodCall(target8, "InterfaceTestAssembly2.Class1.I1Baz");
                Console.WriteLine("This was expected behavior. Will exit with code 100.");
                num = 100;
            }
            catch (Exception ex)
            {
                Console.WriteLine("//EXCEPTION:" + ex.GetType().FullName);
                Console.WriteLine("//STACK TRACE:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                Console.WriteLine("This was unexpected behavior. Will exit with code 99.");
                num = 99;
            }
            int arg_16D_0 = num;
            TestUtil.EndTest();
            return arg_16D_0;
        }

    }
}
