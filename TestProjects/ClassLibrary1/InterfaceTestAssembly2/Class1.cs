using System;
using System.Diagnostics;

namespace InterfaceTestAssembly2
{
    public class Class1 : InterfaceTestAssembly1.Interface1
    {
        private int value = 0;

        public void I1Foo()
        {
            this.value = I1Bar();
        }

        public int I1Bar()
        {
            return 15895567;
        }

        public DateTime I1Baz()
        {
            return DateTime.Now;
        }

        public string I1Boo()
        {
            I1Foo();
            return "Bo0op";
        }

        public void simpleTest(int value) {
            value += 10;
            Debug.Write(value);
        }

        public void takeTwo(object one, object two)
        {
        }

        public void callTakeTwo()
        {
            string v = "hello";
            int one = 1;

            takeTwo(v, one);
            takeTwo(one, DateTime.Now);
            takeTwo(null, v);
            takeTwo(one, one);
            takeTwo(new Class1(), new Class1());
            takeTwo(this, this);
            var i = new Class1();
            takeTwo(i, i);
        }

        public static void staticCallTakeTwo() {
            string v = "hello";
            int one = 1;

            staticTakeTwo(v, one);
            staticTakeTwo(one, DateTime.Now);
            staticTakeTwo(null, v);
            staticTakeTwo(one, one);
            staticTakeTwo(new Class1(), new Class1());
            var i = new Class1();
            staticTakeTwo(i, i);
        }

        public static void staticTakeTwo(object one, object two) {

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
