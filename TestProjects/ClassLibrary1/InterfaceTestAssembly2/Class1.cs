using System;
using System.Diagnostics;

namespace InterfaceTestAssembly2
{
    public class Class1 : InterfaceTestAssembly1.Interface1
    {
        public void I1Foo()
        {
            bippity();
        }

        public int I1Bar()
        {
            return 12999;
        }

        public DateTime I1Baz()
        {
            return DateTime.Now;
        }

        public string I1Boo()
        {
            I1Foo();
            return "Boo";
        }

        public void simpleTest(int value) {
            value += 10;
            Debug.Write(value);
        }

        private void bippity()
        {
            boppity();
        }

        private void boppity()
        {
            boo();
        }

        private void boo()
        {
            Debug.Write("Ahh");
        }
    }
}
