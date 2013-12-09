using System.Collections.Generic;

namespace Arktos.WinBert.Analysis
{
    public class BehavioralDifference
    {
        public int Distance { get; private set; }

        public string TestName { get; private set; }

        public IEnumerable<FieldDifference> FieldDifferences { get; private set; }

        public IEnumerable<PropertyDifference> PropertyDifferences { get; private set; }

        public ReturnValueDifference ReturnValueDifference { get; private set; }
    }
}
