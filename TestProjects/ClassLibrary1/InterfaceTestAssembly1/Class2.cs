using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceTestAssembly1
{
    public class Class2
    {
        public void Hello()
        {
            var toPrint = "Hello world hello";
            Console.WriteLine(toPrint);
        }

        public int Boo()
        {
            var lowerBound = 1;
            var rand = new Random();
            return rand.Next(lowerBound, 100) * 100;
        }
    }
}
