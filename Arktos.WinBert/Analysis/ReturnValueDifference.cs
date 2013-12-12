namespace Arktos.WinBert.Differencing
{
    public class ReturnValueDifference : ValueDifference
    {
        public Xml.MethodCallReturnValue OldReturnValue { get; private set; }

        public Xml.MethodCallReturnValue NewReturnValue { get; private set; }
    }
}
