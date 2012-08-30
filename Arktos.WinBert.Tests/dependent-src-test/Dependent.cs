namespace WinBert.RandoopIntegration.Tests.TestSrc
{
    using System;
    using Depends;

    public class Dependent
    {
        private Dependency dependency;

        public Dependent()
        {
            this.dependency = new Dependency();
        }
    }
}
