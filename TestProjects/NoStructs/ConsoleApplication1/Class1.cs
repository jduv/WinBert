using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Class1
    {
        public string Message { get; private set; }
        public int Number { get; set; }
        public Class1(string value)
        {
            this.Message = value;
        }
    }
}
