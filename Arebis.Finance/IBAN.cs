using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Arebis.Finance
{
    /// <summary>
    /// Formats and validates IBAN account numbers.
    /// </summary>
    public static class IBAN
    {
        /// <summary>
        /// Cleans, capitalizes and formats the given IBAN account number.
        /// </summary>
        /// <param name="ibanNumber">Input IBAN number.</param>
        /// <param name="includeSpaceSeparators">Whether to include space separators.</param>
        /// <returns>Formatted IBAN number.</returns>
        public static string Formatted(string ibanNumber, bool includeSpaceSeparators = true)
        {
            if (String.IsNullOrWhiteSpace(ibanNumber)) return null;

            var sb = new StringBuilder(ibanNumber.Length);
            foreach (var c in ibanNumber)
            {
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z')
                {
                    sb.Append(c);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    sb.Append((char)(c - 32));
                }
                if (includeSpaceSeparators && (sb.Length == 4 || sb.Length == 9 || sb.Length == 14 || sb.Length == 19 || sb.Length == 24 || sb.Length == 29 || sb.Length == 34))
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Asserts the given IBAN number is valid.
        /// </summary>
        /// <param name="ibanNumber">The given IBAN number.</param>
        /// <param name="allowNullOrEmpty">Whether null or empty values are considered valid.</param>
        /// <param name="useAdvancedValidation">Whether to use advanced (country specific) validation rules.</param>
        public static void AssertValid(string ibanNumber, bool allowNullOrEmpty, bool useAdvancedValidation = false)
        {
            var info = Validate(ibanNumber, useAdvancedValidation);
            if (info.IBAN == null)
            {
                if (!allowNullOrEmpty)
                {
                    throw new ArgumentNullException("IBAN number null or empty.");
                }
            }
            else
            {
                if (!info.IsValid)
                {
                    var ex = new ArgumentException(info.ErrorMessage);
                    ex.Data["IBANNumber"] = ibanNumber;
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Validates the given IBAN number.
        /// </summary>
        /// <param name="ibanNumber">The given IBAN number.</param>
        /// <param name="useAdvancedValidation">Whether to use advanced (country specific) validation rules.</param>
        /// <returns>Information about, and about the validity, of the given IBAN number.</returns>
        public static IbanInfo Validate(string ibanNumber, bool useAdvancedValidation = false)
        {
            ibanNumber = Formatted(ibanNumber, false);

            if (String.IsNullOrWhiteSpace(ibanNumber)) return new IbanInfo(false, IbanErrorCause.NoError, "IBAN is null.", null, null);

            if (ibanNumber.Length < 15 || ibanNumber.Length > 34)
            {
                return new IbanInfo(false, IbanErrorCause.InvalidLength, "IBAN has invalid length.", null, ibanNumber);
            }

            var countryCode = ibanNumber.Substring(0, 2);
            var baseNumber = ibanNumber.Substring(4);
            var str = ReplaceLettersByNumbers(baseNumber + ibanNumber.Substring(0, 4));

            if (!BigInteger.Remainder(BigInteger.Parse(str), 97).IsOne)
            {
                return new IbanInfo(false, IbanErrorCause.ChecksumFailed, "IBAN checksum failed.", countryCode, ibanNumber);
            }

            if (useAdvancedValidation)
            {
                var data = IbanDataList().SingleOrDefault(d => d.CountryCode == countryCode);
                if (data == null)
                {
                    return new IbanInfo(false, IbanErrorCause.UnknownCountryCode, "Unknown IBAN country code.", countryCode, ibanNumber);
                }

                if (!data.Regex.IsMatch(baseNumber))
                {
                    return new IbanInfo(false, IbanErrorCause.CountryCheckFailed, "IBAN does not match format of country.", countryCode, ibanNumber);
                }
            }

            return new IbanInfo(true, IbanErrorCause.NoError, "IBAN is valid.", countryCode, ibanNumber);
        }

        #region class IbanInfo, enum IbanErrorCause

        public class IbanInfo
        {
            public IbanInfo(bool isValid, IbanErrorCause errorCause, string errorMessage, string countryCode, string iban)
            {
                this.IsValid = isValid;
                this.ErrorCause = errorCause;
                this.ErrorMessage = errorMessage;
                this.CountryCode = countryCode;
                this.IBAN = iban;
            }

            /// <summary>
            /// Whether the related IBAN number is valid.
            /// </summary>
            public bool IsValid { get; private set; }

            /// <summary>
            /// Cause of error or NoError.
            /// </summary>
            public IbanErrorCause ErrorCause { get; private set; }

            /// <summary>
            /// Error or success message.
            /// </summary>
            public string ErrorMessage { get; private set; }

            /// <summary>
            /// Alpha-2 ISO 3166 country code.
            /// </summary>
            public string CountryCode { get; private set; }

            /// <summary>
            /// Fully formatted IBAN number.
            /// </summary>
            public string IBAN { get; private set; }
        }

        public enum IbanErrorCause
        {
            NoError = 0,
            InvalidLength = 1,
            ChecksumFailed = 2,
            UnknownCountryCode = 3,
            CountryCheckFailed = 4,
        }

        #endregion

        #region Private implementation

        /// <summary>
        /// Replaces "A" by "10", "B" by "11", etc.
        /// </summary>
        /// <param name="str">Input string, assumed to contain only 0-9 and A-Z characters.</param>
        private static string ReplaceLettersByNumbers(string str)
        {
            var result = new StringBuilder(str.Length + 12);
            foreach (var c in str)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    result.Append((((int)c) - 55).ToString());
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        #endregion

        #region Country specific validation rules

        private static List<IbanData> ibanDataList = null;

        /// <summary>
        /// List of country specific IBAN validation rules.
        /// Source: https://www.codeproject.com/Articles/55667/IBAN-Verification-in-C-Excel-Automation-Add-in-Wor, Sedar Altug, 2010
        /// Check updates @ http://www.tbg5-finance.org/checkiban.js
        /// </summary>
        private static List<IbanData> IbanDataList()
        {
            if (ibanDataList == null)
            {
                List<IbanData> newList = new List<IbanData>();
                newList.Add(new IbanData("AD", 24, @"^\d{8}[a-zA-Z0-9]{12}$", false, "AD1200012030200359100100"));
                newList.Add(new IbanData("AL", 28, @"^\d{8}[a-zA-Z0-9]{16}$", false, "AL47212110090000000235698741"));
                newList.Add(new IbanData("AT", 20, @"^\d{16}$", true, "AT611904300234573201"));
                newList.Add(new IbanData("BA", 20, @"^\d{16}$", false, "BA391290079401028494"));
                newList.Add(new IbanData("BE", 16, @"^\d{12}$", true, "BE68539007547034"));
                newList.Add(new IbanData("BG", 22, @"^[A-Z]{4}\d{6}[a-zA-Z0-9]{8}$", true, "BG80BNBG96611020345678"));
                newList.Add(new IbanData("CH", 21, @"^\d{5}[a-zA-Z0-9]{12}$", false, "CH9300762011623852957"));
                newList.Add(new IbanData("CY", 28, @"^\d{8}[a-zA-Z0-9]{16}$", true, "CY17002001280000001200527600"));
                newList.Add(new IbanData("CZ", 24, @"^\d{20}$", true, "CZ6508000000192000145399"));
                newList.Add(new IbanData("DE", 22, @"^\d{18}$", true, "DE89370400440532013000"));
                newList.Add(new IbanData("DK", 18, @"^\d{14}$", true, "DK5000400440116243"));
                newList.Add(new IbanData("EE", 20, @"^\d{16}$", true, "EE382200221020145685"));
                newList.Add(new IbanData("ES", 24, @"^\d{20}$", true, "ES9121000418450200051332"));
                newList.Add(new IbanData("FI", 18, @"^\d{14}$", true, "FI2112345600000785"));
                newList.Add(new IbanData("FO", 18, @"^\d{14}$", false, "FO6264600001631634"));
                newList.Add(new IbanData("FR", 27, @"^\d{10}[a-zA-Z0-9]{11}\d\d$", true, "FR1420041010050500013M02606"));
                newList.Add(new IbanData("GB", 22, @"^[A-Z]{4}\d{14}$", true, "GB29NWBK60161331926819"));
                newList.Add(new IbanData("GI", 23, @"^[A-Z]{4}[a-zA-Z0-9]{15}$", true, "GI75NWBK000000007099453"));
                newList.Add(new IbanData("GL", 18, @"^\d{14}$", false, "GL8964710001000206"));
                newList.Add(new IbanData("GR", 27, @"^\d{7}[a-zA-Z0-9]{16}$", true, "GR1601101250000000012300695"));
                newList.Add(new IbanData("HR", 21, @"^\d{17}$", false, "HR1210010051863000160"));
                newList.Add(new IbanData("HU", 28, @"^\d{24}$", true, "HU42117730161111101800000000"));
                newList.Add(new IbanData("IE", 22, @"^[A-Z]{4}\d{14}$", true, "IE29AIBK93115212345678"));
                newList.Add(new IbanData("IL", 23, @"^\d{19}$", false, "IL620108000000099999999"));
                newList.Add(new IbanData("IS", 26, @"^\d{22}$", true, "IS140159260076545510730339"));
                newList.Add(new IbanData("IT", 27, @"^[A-Z]\d{10}[a-zA-Z0-9]{12}$", true, "IT60X0542811101000000123456"));
                newList.Add(new IbanData("LB", 28, @"^\d{4}[a-zA-Z0-9]{20}$", false, "LB62099900000001001901229114"));
                newList.Add(new IbanData("LI", 21, @"^\d{5}[a-zA-Z0-9]{12}$", true, "LI21088100002324013AA"));
                newList.Add(new IbanData("LT", 20, @"^\d{16}$", true, "LT121000011101001000"));
                newList.Add(new IbanData("LU", 20, @"^\d{3}[a-zA-Z0-9]{13}$", true, "LU280019400644750000"));
                newList.Add(new IbanData("LV", 21, @"^[A-Z]{4}[a-zA-Z0-9]{13}$", true, "LV80BANK0000435195001"));
                newList.Add(new IbanData("MC", 27, @"^\d{10}[a-zA-Z0-9]{11}\d\d$", true, "MC1112739000700011111000h79"));
                newList.Add(new IbanData("ME", 22, @"^\d{18}$", false, "ME25505000012345678951"));
                newList.Add(new IbanData("MK", 19, @"^\d{3}[a-zA-Z0-9]{10}\d\d$", false, "MK07300000000042425"));
                newList.Add(new IbanData("MT", 31, @"^[A-Z]{4}\d{5}[a-zA-Z0-9]{18}$", true, "MT84MALT011000012345MTLCAST001S"));
                newList.Add(new IbanData("MU", 30, @"^[A-Z]{4}\d{19}[A-Z]{3}$", false, "MU17BOMM0101101030300200000MUR"));
                newList.Add(new IbanData("NL", 18, @"^[A-Z]{4}\d{10}$", true, "NL91ABNA0417164300"));
                newList.Add(new IbanData("NO", 15, @"^\d{11}$", true, "NO9386011117947"));
                newList.Add(new IbanData("PL", 28, @"^\d{8}[a-zA-Z0-9]{16}$", true, "PL27114020040000300201355387"));
                newList.Add(new IbanData("PT", 25, @"^\d{21}$", true, "PT50000201231234567890154"));
                newList.Add(new IbanData("RO", 24, @"^[A-Z]{4}[a-zA-Z0-9]{16}$", true, "RO49AAAA1B31007593840000"));
                newList.Add(new IbanData("RS", 22, @"^\d{18}$", false, "RS35260005601001611379"));
                newList.Add(new IbanData("SA", 24, @"^\d{2}[a-zA-Z0-9]{18}$", false, "SA0380000000608010167519"));
                newList.Add(new IbanData("SE", 24, @"^\d{20}$", true, "SE4550000000058398257466"));
                newList.Add(new IbanData("SI", 19, @"^\d{15}$", true, "SI56191000000123438"));
                newList.Add(new IbanData("SK", 24, @"^\d{20}$", true, "SK3112000000198742637541"));
                newList.Add(new IbanData("SM", 27, @"^[A-Z]\d{10}[a-zA-Z0-9]{12}$", false, "SM86U0322509800000000270100"));
                newList.Add(new IbanData("TN", 24, @"^\d{20}$", false, "TN5914207207100707129648"));
                newList.Add(new IbanData("TR", 26, @"^\d{5}[a-zA-Z0-9]{17}$", false, "TR330006100519786457841326"));
                ibanDataList = newList;
            }

            return ibanDataList;
        }

        private class IbanData
        {
            public string CountryCode;
            public int Lenght;
            public Regex Regex;
            public bool IsEU924;
            public string Sample;

            public IbanData()
            {
            }
            public IbanData(string countryCode, int lenght, string regexStructure, bool isEU924, string sample)
                : this()
            {
                CountryCode = countryCode;
                Lenght = lenght;
                Regex = new Regex(regexStructure, RegexOptions.Compiled);
                IsEU924 = isEU924;
                Sample = sample;
            }
        }

        #endregion
    }
}
