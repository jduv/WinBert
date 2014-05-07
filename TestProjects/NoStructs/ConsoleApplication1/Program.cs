using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Program
    {
        private int twiddle = 345;
        private Class1 instance = new Class1("default");

        public void Test1()
        {
            instance = new Class1("Test1");
        }

        public void Test2(Class1 toProcess)
        {
            toProcess.Number = 15466;
            this.instance = toProcess;
        }

        public void Test3(Class1 toProcess)
        {
            toProcess.Number = 8679;
            this.instance = toProcess;
        }

        public void Test4SkipTo2(Class1 toProcess)
        {
            Test2(toProcess);
        }

        public Class1 Test5SkipTo4(Class1 toProcess)
        {
            Test4SkipTo2(toProcess);
            return toProcess;
        }

        public Class1 TestModInt(int number)
        {
            var c = new Class1("TestModInt") { Number = number };
            this.instance = c;
            return c;
        }

        public void Twiddle()
        {
            this.twiddle = 111090;
        }

        public static void Main(string[] args)
        {
        }
    }
}
