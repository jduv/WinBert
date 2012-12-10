namespace Arktos.WinBert.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Arktos.WinBert.Instrumentation;
    using System;

    [TestClass]
    public class MethodCallDumperUnitTests
    {
        #region Test Methods 
        
        #region DumpVoidInstanceMethod

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpVoidInstanceMethod_NullSignature()
        {
            var target = new MethodCallDumper();
            target.DumpVoidInstanceMethod(1, "test", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpVoidInstanceMethod_EmptySignature()
        {
            var target = new MethodCallDumper();
            target.DumpVoidInstanceMethod(1, "test", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DumpVoidInstanceMethod_NullTarget()
        {
            var target = new MethodCallDumper();
            target.DumpVoidInstanceMethod(1, null, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpVoidInstanceMethod_PrimitiveTarget()
        {
            var target = new MethodCallDumper();
            target.DumpVoidInstanceMethod(1, 1, "Test");
        }

        [TestMethod]
        public void DumpVoidInstanceMethod_ReferenceTypeTarget()
        {
            var refType = new { X = 1, Y = 2, Z = 3 };
            var target = new MethodCallDumper();
            var actual = target.DumpVoidInstanceMethod(1, refType, "Test");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Test", actual.Signature);
            Assert.AreEqual(1U, actual.Id);
            Assert.AreEqual(Xml.MethodCallType.Instance,actual.Type);

            Assert.IsNull(actual.ReturnValue);
            Assert.IsNotNull(actual.PostCallInstance);

            var targetInstance = actual.PostCallInstance as Xml.Object;
            Assert.IsNotNull(targetInstance);
            Assert.AreEqual(refType.GetType().FullName, targetInstance.Type);
            Assert.AreEqual(3, targetInstance.Fields.Length);
            Assert.AreEqual(3, targetInstance.Properties.Length);
        }

        [TestMethod]
        public void DumpVoidInstanceMethod_ValueTypeTarget()
        {
            var valueType = new TestPoint();
            var target = new MethodCallDumper();
            var actual = target.DumpVoidInstanceMethod(1, valueType, "Discombobulate");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Discombobulate", actual.Signature);
            Assert.AreEqual(1U, actual.Id);
            Assert.AreEqual(Xml.MethodCallType.Instance, actual.Type);

            Assert.IsNull(actual.ReturnValue);
            Assert.IsNotNull(actual.PostCallInstance);

            var targetInstance = actual.PostCallInstance as Xml.Object;
            Assert.IsNotNull(targetInstance);
            Assert.AreEqual(valueType.GetType().FullName, targetInstance.Type);

            Assert.AreEqual(valueType.GetType().FullName, targetInstance.Type);
            Assert.AreEqual(2, targetInstance.Fields.Length);
            Assert.AreEqual(2, targetInstance.Properties.Length);
        }

        #endregion

        #region DumpInstanceMethod

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpInstanceMethod_NullSignature()
        {
            var target = new MethodCallDumper();
            target.DumpInstanceMethod(1, "test", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DumpInstanceMethod_EmptySignature()
        {
            var target = new MethodCallDumper();
            target.DumpInstanceMethod(1, "test", null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DumpInstanceMethod_NullTarget()
        {
            var target = new MethodCallDumper();
            target.DumpInstanceMethod(1, null, null, "Test");
        }

        [TestMethod]
        public void DumpInstanceMethod_ReferenceTypeTargetPrimitiveResult()
        {
            var refType = new { X = 1, Y = 2, Z = 3 };
            var returnValue = 1;
            var target = new MethodCallDumper();
            var actual = target.DumpInstanceMethod(1, refType, returnValue,  "Test");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Test", actual.Signature);
            Assert.AreEqual(1U, actual.Id);
            Assert.AreEqual(Xml.MethodCallType.Instance, actual.Type);

            Assert.IsNotNull(actual.ReturnValue);

            var result = actual.ReturnValue.Item as Xml.Primitive;
            Assert.IsNotNull(result);
            Assert.AreEqual(returnValue.GetType().FullName, result.Type);
            Assert.AreEqual(returnValue.ToString(), result.Value);

            Assert.IsNotNull(actual.PostCallInstance);

            var targetInstance = actual.PostCallInstance as Xml.Object;
            Assert.IsNotNull(targetInstance);
            Assert.AreEqual(refType.GetType().FullName, targetInstance.Type);
            Assert.AreEqual(3, targetInstance.Fields.Length);
            Assert.AreEqual(3, targetInstance.Properties.Length);
        }

        [TestMethod]
        public void DumpInstanceMethod_ReferenceTypeTargetObjectResult()
        {
            var refType = new { X = 1, Y = 2, Z = 3 };
            var returnValue = new TestPoint();
            var target = new MethodCallDumper();
            var actual = target.DumpInstanceMethod(1, refType, returnValue, "Test");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Test", actual.Signature);
            Assert.AreEqual(1U, actual.Id);
            Assert.AreEqual(Xml.MethodCallType.Instance, actual.Type);

            Assert.IsNotNull(actual.ReturnValue);

            var result = actual.ReturnValue.Item as Xml.Object;
            Assert.IsNotNull(result);
            Assert.AreEqual(returnValue.GetType().FullName, result.Type);

            Assert.IsNotNull(actual.PostCallInstance);

            var targetInstance = actual.PostCallInstance as Xml.Object;
            Assert.IsNotNull(targetInstance);
            Assert.AreEqual(refType.GetType().FullName, targetInstance.Type);
            Assert.AreEqual(3, targetInstance.Fields.Length);
            Assert.AreEqual(3, targetInstance.Properties.Length);
        }

        [TestMethod]
        public void DumpInstanceMethod_ValueTypeTarget()
        {
            var valueType = new TestPoint();
            var target = new MethodCallDumper();
            var actual = target.DumpVoidInstanceMethod(1, valueType, "Discombobulate");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Discombobulate", actual.Signature);
            Assert.AreEqual(1U, actual.Id);
            Assert.AreEqual(Xml.MethodCallType.Instance, actual.Type);

            Assert.IsNull(actual.ReturnValue);
            Assert.IsNotNull(actual.PostCallInstance);

            var targetInstance = actual.PostCallInstance as Xml.Object;
            Assert.IsNotNull(targetInstance);
            Assert.AreEqual(valueType.GetType().FullName, targetInstance.Type);

            Assert.AreEqual(valueType.GetType().FullName, targetInstance.Type);
            Assert.AreEqual(2, targetInstance.Fields.Length);
            Assert.AreEqual(2, targetInstance.Properties.Length);
        }

        #endregion

        #endregion

        #region Test Classes

        /// <summary>
        /// A test struct with properties and fields.
        /// </summary>
        private struct TestPoint
        {
            private int x;
            private int y;

            public TestPoint(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int X
            {
                get
                {
                    return this.x;
                }
                set
                {
                    this.x = value;
                }
            }

            public int Y
            {
                get
                {
                    return this.y;
                }
                set
                {
                    this.y = value;
                }
            }

            public void Discombobulate()
            {
                var tmp = this.X + this.Y;
                this.X = tmp;
                this.Y = tmp;
            }

            public int Sum()
            {
                return this.X + this.Y;
            }
        }

        #endregion
    }
}