using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Arebis.Finance.Tests
{
    [TestClass]
    public class CreditCardTests
    {
        [TestMethod]
        public void IdentificationTests()
        {
            Assert.AreEqual(CreditCardType.AmericanExpress, CreditCardHelper.IdentifyType("371449635398431"), "Failed to identify AmericanExpress");
            Assert.AreEqual(CreditCardType.AmericanExpress, CreditCardHelper.IdentifyType("371449635XXxX31"), "Failed to identify AmericanExpress X");
            Assert.AreEqual(CreditCardType.DinersClub, CreditCardHelper.IdentifyType("30569309025904"), "Failed to identify DinersClub");
            Assert.AreEqual(CreditCardType.DinersClub, CreditCardHelper.IdentifyType("305693XXXX59xX"), "Failed to identify DinersClub X");
            Assert.AreEqual(CreditCardType.Mastercard, CreditCardHelper.IdentifyType("5555555555554444"), "Failed to identify Mastercard");
            Assert.AreEqual(CreditCardType.Mastercard, CreditCardHelper.IdentifyType("5555555555XxX444"), "Failed to identify Mastercard X");
            Assert.AreEqual(CreditCardType.VISA, CreditCardHelper.IdentifyType("4111111111111111"), "Failed to identify VISA");
            Assert.AreEqual(CreditCardType.VISA, CreditCardHelper.IdentifyType("41111111111XxXXX"), "Failed to identify VISA X16");
            Assert.AreEqual(CreditCardType.VISA, CreditCardHelper.IdentifyType("41111111XXxxX"), "Failed to identify VISA X13");
            Assert.AreEqual(CreditCardType.VISA, CreditCardHelper.IdentifyType("4111 1111 1111 1111"), "Failed to identify VISA spaced");

            Assert.AreEqual(CreditCardType.Unknown, CreditCardHelper.IdentifyType("000000000000XXxx"), "Failed to identify Unknown X");
        }
    }
}
