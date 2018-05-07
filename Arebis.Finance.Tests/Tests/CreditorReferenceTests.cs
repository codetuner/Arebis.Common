using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Arebis.Finance.Tests
{
    [TestClass]
    public class CreditorReferenceTests
    {
        [TestMethod]
        public void CreditorReferenceValidationTest()
        {
            Assert.IsTrue(CreditorReference.Validate("RF 35 0013 3690").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 74 0013 3667").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 36 0013 3672").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 90 0013 3670").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 34 0013 3611").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 68 0013 3581").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 50 0013 3420 ").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 58 0013 3567 ").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF9000133573").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 83 0013 3505").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 78 0013 3498").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 95 0013 3483").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 68 0013 3484").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 52 0013 3481").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 13 0013 3407").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 64 0013 3362").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 43 0013 3255").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 05 0013 3260").IsValid);
            Assert.IsTrue(CreditorReference.Validate("RF 29 0013 3216").IsValid);
            Assert.IsTrue(CreditorReference.Validate("+++000/0133/28507+++").IsValid);
        }
    }
}
