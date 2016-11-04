using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TargetInfomation;

namespace Seecool.TestTool.Test
{
    [TestClass]
    public class MovableTargetVerifyUnit
    {
        MovableTargetVerify _verify;

        [TestMethod]
        public void TestNormalMovement()
        {
            _verify = new MovableTargetVerify();
            Assert.IsTrue(_verify.Update(121.31, 31.24, new DateTime(2015, 1, 29, 9, 0, 0)));
            Assert.IsTrue(_verify.Update(121.31, 31.240003, new DateTime(2015, 1, 29, 9, 0, 3)));
        }

        [TestMethod]
        public void TestAbNormalMovement()
        {
            _verify = new MovableTargetVerify();
            Assert.IsTrue(_verify.Update(121.31, 31.24, new DateTime(2015, 1, 29, 9, 0, 0)));
            Assert.IsFalse(_verify.Update(121.31, 31.243, new DateTime(2015, 1, 29, 9, 0, 3)));
        }
    }
}