using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OTAPI.Patcher.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var classInstance = new OTAPI.Core.Class1();
            classInstance = null;
        }
    }
}
