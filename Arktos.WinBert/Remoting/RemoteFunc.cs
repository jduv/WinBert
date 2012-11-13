using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arktos.WinBert.Remoting
{
    public class RemoteFunc<TIn, TOut> : MarshalByRefObject
    {
        public TOut Execute(TIn input, Func<TIn, TOut> method)
        {
            return method(input);
        }
    }
}
