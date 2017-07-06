using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MAP200;
using TestSetLib;

namespace MAP200Tests
{
    [TestClass]
    public class DefaultTests
    {
        [TestClass]
        public class ValidateSerialNumberMethod
        {
            [TestMethod]
            public void AllNumericSerialNumberShouldReturnOpSuccessTrue()
            {
                //Arrange
                var defaultClass = new Default();
                string serialNumber = "222888777001";

                //Act
                var actualOp = defaultClass.ValidateSerialNumber(serialNumber);

                //Assert
                Assert.IsTrue(actualOp.Success);
            }

            [TestMethod]
            public void AllLetterSerialNumberShouldReturnOpSuccessFalse()
            {
                var defaultClass = new Default();
                string serialNumber = "ajdfioabioasnfda";
                string message = "Serial number must only contain numbers";

                var actualOp = defaultClass.ValidateSerialNumber(serialNumber);

                Assert.IsFalse(actualOp.Success);
                Assert.AreEqual(actualOp.ErrorMessages[0], message);
            }

            [TestMethod]
            public void AlphaNumericSerialNumberShouldReturnOpSuccessFalse()
            {
                var defaultClass = new Default();
                string serialNumber = "22222AAABBB3434";
                string message = "Serial number must only contain numbers";

                var actualOp = defaultClass.ValidateSerialNumber(serialNumber);

                Assert.IsFalse(actualOp.Success);
                Assert.AreEqual(actualOp.ErrorMessages[0], message);
            }

            [TestMethod]
            public void NothingInSerialNumberShouldReturnOpSuccessFalseWithMessage()
            {
                var defaultClass = new Default();
                string serialNumber = "";
                string message = "Serial ID length cannot be zero";

                var actualOp = defaultClass.ValidateSerialNumber(serialNumber);

                Assert.IsFalse(actualOp.Success);
                Assert.AreEqual(actualOp.ErrorMessages[0], message);
            }
        }
    }
}
