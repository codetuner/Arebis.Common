using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Arebis.Text.Tests
{
    [TestClass]
    public class UnidecodeTests
    {
        [TestMethod]
        public void ToAsciiTest()
        {
            Assert.AreEqual("Fóòbàáréès", "Fóòbàáréès".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Evgeniya", "Евгения".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Zhigulina", "Жигулина".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Yuliya", "Юлия".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Niscáková Ceresnáková", "Niščáková Čerešňáková".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Åhlén Duric", "Åhlén Đuric".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Bak Maciag Walachowska Lopuszynski", "Bąk Maciąg Wałachowska Łopuszyński".Unidecode(UnidecoderLevel.Ascii));
            Assert.AreEqual("Øgreid Byrløkken", "Øgreid Byrløkken".Unidecode(UnidecoderLevel.Ascii));
        }

        [TestMethod]
        public void ToAnsiPlusTest()
        {
            Assert.AreEqual("U", "𝒰".Unidecode(UnidecoderLevel.AnsiPlus));

            // https://apps.timwhitlock.info/unicode/inspect?s=%F0%9D%90%80%F0%9D%90%83%F0%9D%90%84%F0%9D%90%85%F0%9D%90%86%F0%9D%90%88%F0%9D%90%8B%F0%9D%90%8D%F0%9D%90%8E%F0%9D%90%91%F0%9D%90%93%F0%9D%90%95%F0%9D%90%96
            Assert.AreEqual("ADEFGILNORTVW", "𝐀𝐃𝐄𝐅𝐆𝐈𝐋𝐍𝐎𝐑𝐓𝐕𝐖".Unidecode(UnidecoderLevel.AnsiPlus));
        }
    }
}
