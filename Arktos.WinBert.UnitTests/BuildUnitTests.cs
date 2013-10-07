namespace Arktos.WinBert.UnitTests
{
    using Arktos.WinBert.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BuildUnitTests
    {
        #region Test Methods

        #region Equals

        [TestMethod]
        public void Equals_SameObject_ReferenceEquals()
        {
            var target = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory" };
            Assert.AreEqual(target, target);
        }

        [TestMethod]
        public void Equals_DifferentObject_Equals()
        {
            uint seqNumber = 0;
            string path = @"C:\my\path\to\victory";
            var target = new Build() { SequenceNumber = seqNumber, AssemblyPath = path };
            var clone = new Build() { SequenceNumber = seqNumber, AssemblyPath = path };
            Assert.AreEqual(target, clone);
        }

        [TestMethod]
        public void Equals_PathDifferent_DoesNotEqual()
        {
            var target = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory" };
            var other = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory\diff" };
            Assert.AreNotEqual(target, other);
        }

        [TestMethod]
        public void Equals_SequenceDifferent_DoesNotEqual()
        {
            var target = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory" };
            var other = new Build() { SequenceNumber = 1, AssemblyPath = @"C:\my\path\to\victory" };
            Assert.AreNotEqual(target, other);
        }

        [TestMethod]
        public void Equals_ObjectOverload_Equals()
        {
            object target = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory" };
            object clone = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory" };

            Assert.IsTrue(target.Equals(clone));
        }

        [TestMethod]
        public void Equals_ObjectOverload_DoesNotEqual()
        {
            object target = new Build() { SequenceNumber = 0, AssemblyPath = @"C:\my\path\to\victory" };
            Assert.IsFalse(target.Equals(2));
        }

        #endregion

        #region GetHashCode

        [TestMethod]
        public void GetHashCode_ComputesCorrectly()
        {
            uint seqNumber = 0;
            string path = @"C:\my\path\to\victory";
            var target = new Build() { SequenceNumber = seqNumber, AssemblyPath = path };

            int hashCode = seqNumber.GetHashCode() ^ path.GetHashCode();

            Assert.AreEqual(hashCode, target.GetHashCode());
        }

        #endregion

        #endregion
    }
}
