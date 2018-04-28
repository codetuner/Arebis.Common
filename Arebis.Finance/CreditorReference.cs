using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Arebis.Finance
{
    // Documentation:
    // https://en.wikipedia.org/wiki/Creditor_Reference
    // https://www.factuursturen.nl/help/111/hoe-werkt-een-betalingskenmerk/ (NL)
    // http://www.finanssiala.fi/maksujenvalitys/dokumentit/Introduction_of_the_global_Structured_Creditor_Reference.pdf
    // https://nl.wikipedia.org/wiki/Gestructureerde_mededeling
    // https://www.acceptgiro.nl/wp-uploads/Cu_RR_Ac_Bijlage_G.A_Formspecs.pdf
    // https://nl.wikipedia.org/wiki/Elfproef

    // Tools:
    // - RF Credit References:
    //   http://www.jknc.eu/RFcalculator
    // - Belgian OGM:
    //   http://www.gestructureerdemededeling.be/
    // - Dutch ElfProef and reference:

    /// <summary>
    /// Generates and validates "Structured Creditor References" according to the IDO 11649 scheme or some national scheme.
    /// </summary>
    public static class CreditorReference
    {
        private static Regex BE_OGM = new Regex("^[0-9]{12}$", RegexOptions.Compiled);
        private static Regex NL_AGRef = new Regex("^([0-9]{7-14}|[0-9]{16})$", RegexOptions.Compiled); // Acceptgiro betalingskenmerk
        private static NProef NL_11ProefAcceptGiro = new NProef(11, new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6, 1 });

        /// <summary>
        /// Returns a formatted creditor reference for the given id.
        /// If minLength &lt;= 10 and countryCode = "BE", then a Belgian OGM reference is returned.
        /// Otherwise, an ISO 11649 formatted reference is returned.
        /// </summary>
        /// <param name="id">The id value.</param>
        /// <param name="minLength">The minimum length of the creditor reference.</param>
        /// <param name="countryCode">Optional alpha-2 ISO 3166 country code for which to use scheme if possible.</param>
        /// <returns>A formatted creditor reference.</returns>
        public static string From(long id, byte minLength = 0, string countryCode = null)
        {
            // Convert to string of minimum length:
            var idstr = id.ToString();
            if (idstr.Length < minLength)
                idstr = new String('0', minLength - idstr.Length) + idstr;

            // Choose scheme to apply:
            if (countryCode == "BE" && idstr.Length <= 10)
            {
                // Apply Belgian scheme:
                var checksum = (id % 97L);
                if (checksum == 0) checksum = 97;
                return String.Format("+++{0:000/0000/000}{1:00}+++", id, checksum);
            }
            //else if (countryCode == "NL" && idstr.Length <= 15)
            //{
            //    // Should be at least 7 chars (+ 1 checksum + 1 length char = 9 chars)
            //    if (idstr.Length < 7) return From(id, 7, countryCode);
            //
            //    if (idstr.Length < 15)
            //    {
            //        // Add length char:
            //        char lengthcode = (char)('0' + (idstr.Length % 10));
            //        idstr = lengthcode + idstr;
            //    }
            //
            //    // Add checksum:
            //    var remainder = NL_11ProefAcceptGiro.GetWeightedSumRemainder(Int64.Parse(idstr));
            //    if (remainder == 10) remainder = 1;
            //    idstr = (remainder) + idstr;
            //
            //    // Return result:
            //    return idstr;
            //}
            else
            {
                // Apply ISO 11649 scheme:
                return "RF " + ((int)BigInteger.Subtract(98, BigInteger.Remainder(BigInteger.Add(BigInteger.Multiply(new BigInteger(id), 1000000), 271500), 97))).ToString("00") + " " + Grouped(idstr, 4, ' ');
            }
        }

        /// <summary>
        /// Asserts the given value is a valid creditor reference.
        /// </summary>
        /// <param name="reference">The reference to validate.</param>
        /// <param name="allowNullOrEmpty">Whether a null or empty reference is considered valid.</param>
        /// <param name="countryCodeHint">Optional alpha-2 ISO 3166 country code hint to help determine the right reference scheme.</param>
        public static void AssertValid(string reference, bool allowNullOrEmpty, string countryCodeHint = null)
        {
            var info = Validate(reference, countryCodeHint);
            if (info.CreditorReference == null)
            {
                if (!allowNullOrEmpty)
                {
                    throw new ArgumentNullException("Credit reference null or empty.");
                }
            }
            else
            {
                if (!info.IsValid)
                {
                    var ex = new ArgumentException(info.ErrorMessage);
                    ex.Data["CreditReference"] = reference;
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Validates the given credit reference.
        /// </summary>
        /// <param name="reference">The reference to validate.</param>
        /// <param name="countryCodeHint">Optional alpha-2 ISO 3166 country code hint to help determine the right reference scheme.</param>
        /// <returns>Information about, and about the validity, of the given credit reference.</returns>
        public static CreditorReferenceInfo Validate(string reference, string countryCodeHint = null)
        {
            var freference = FilterAlfaNum(reference);

            if (String.IsNullOrWhiteSpace(freference)) return new CreditorReferenceInfo(false, CreditReferenceErrorCause.NoError, "Credit reference is null.", null, null, null);

            if (freference.Length < 5 || freference.Length > 25)
            {
                return new CreditorReferenceInfo(false, CreditReferenceErrorCause.InvalidLength, "Creditor reference had invalid length.", null, reference, null);
            }

            if (freference.StartsWith("RF"))
            {
                var baseNumber = freference.Substring(4);
                var str = ReplaceLettersByNumbers(baseNumber + freference.Substring(0, 4));

                if (BigInteger.Remainder(BigInteger.Parse(str), 97).IsOne)
                {
                    long id;
                    if (Int64.TryParse(str.Substring(0, str.Length-6), out id))
                    {
                        return new CreditorReferenceInfo(true, CreditReferenceErrorCause.NoError, "Credit reference is valid.", null, reference, id);
                    }
                    else
                    {
                        return new CreditorReferenceInfo(true, CreditReferenceErrorCause.NoError, "Credit reference is valid.", null, reference, null);
                    }
                }
                else
                {
                    return new CreditorReferenceInfo(false, CreditReferenceErrorCause.ChecksumFailed, "Credit reference checksum failed.", null, reference, null);
                }
            }

            if ((countryCodeHint == null || countryCodeHint == "BE") && BE_OGM.IsMatch(freference))
            {
                var baseNumber = Int64.Parse(freference.Substring(0, 10));
                var checkNumber = Int64.Parse(freference.Substring(10));
                var calculatedCheckNumber = baseNumber % 97L;
                if (calculatedCheckNumber == 0) calculatedCheckNumber = 97;
                if (checkNumber == calculatedCheckNumber)
                {
                    return new CreditorReferenceInfo(true, CreditReferenceErrorCause.NoError, "Credit reference is valid.", "BE", reference, baseNumber);
                }
                else if (countryCodeHint == "BE")
                {
                    return new CreditorReferenceInfo(false, CreditReferenceErrorCause.ChecksumFailed, "Credit reference checksum failed.", "BE", reference, null);
                }
                else
                {
                    // Ignore and proceed. Could be a Dutch number or another as well...
                }
            }

            //if ((countryCodeHint == null || countryCodeHint == "NL") && NL_AGRef.IsMatch(freference))
            //{
            //    if (freference.Length == 7)
            //    { 
            //        // Length 7 has no checksum or length code and is therefore always assumed valid:
            //        return new CreditorReferenceInfo(true, CreditReferenceErrorCause.NoError, "Credit reference is valid.", "NL", reference, Int64.Parse(freference));
            //    }
            //    else if (freference.Length == 8 || freference.Length == 16)
            //    {
            //        // Length 8/16 has only a checksum but no length code:
            //        var baseValue = Int64.Parse(freference.Substring(1));
            //        var xcs = freference[0] - '0';
            //        var rcs = NL_11ProefAcceptGiro.GetWeightedSumRemainder(baseValue);
            //        if (xcs == rcs)
            //        {
            //            return new CreditorReferenceInfo(true, CreditReferenceErrorCause.NoError, "Credit reference is valid.", "NL", reference, baseValue);
            //        }
            //        else if (countryCodeHint == "NL")
            //        {
            //            return new CreditorReferenceInfo(false, CreditReferenceErrorCause.ChecksumFailed, "Credit reference checksum failed.", "NL", reference, null);
            //        }
            //    }
            //    else
            //    {
            //        // Length 9+ has checksum and length code:
            //        var baseValue = Int64.Parse(freference.Substring(1));
            //        // Length code:
            //        var xlc = freference.Length - 2;
            //        var rlc = (freference[1] - '0') % 10;
            //        // Checksum:
            //        var xcs = freference[0] - '0';
            //        var rcs = NL_11ProefAcceptGiro.GetWeightedSumRemainder(baseValue);
            //        // Compare:
            //        if (xlc == rlc && xcs == rcs)
            //        {
            //            return new CreditorReferenceInfo(true, CreditReferenceErrorCause.NoError, "Credit reference is valid.", "NL", reference, baseValue);
            //        }
            //        else if (countryCodeHint == "NL")
            //        {
            //            return new CreditorReferenceInfo(false, CreditReferenceErrorCause.ChecksumFailed, "Credit reference checksum failed.", "NL", reference, null);
            //        }
            //    }
            //}

            // If all previous failed:
            return new CreditorReferenceInfo(false, CreditReferenceErrorCause.UnknownPattern, "Unknown credit reference pattern or invalid checksum.", null, reference, null);
        }

        #region class CreditorReferenceInfo, enum CreditReferenceErrorCause

        public class CreditorReferenceInfo
        {
            public CreditorReferenceInfo(bool isValid, CreditReferenceErrorCause errorCause, string errorMessage, string countryCode, string creditorReference, long? id)
            {
                this.IsValid = isValid;
                this.ErrorCause = errorCause;
                this.ErrorMessage = errorMessage;
                this.CountryCode = countryCode;
                this.CreditorReference = creditorReference;
                this.Id = id;
            }

            /// <summary>
            /// Whether the related IBAN number is valid.
            /// </summary>
            public bool IsValid { get; private set; }

            /// <summary>
            /// Cause of error or NoError.
            /// </summary>
            public CreditReferenceErrorCause ErrorCause { get; private set; }

            /// <summary>
            /// Error or success message.
            /// </summary>
            public string ErrorMessage { get; private set; }

            /// <summary>
            /// Alpha-2 ISO 3166 country code, if creditor reference was identified as a country-specific scheme.
            /// </summary>
            public string CountryCode { get; private set; }

            /// <summary>
            /// The original creditor reference.
            /// </summary>
            public string CreditorReference { get; private set; }

            /// <summary>
            /// The id value the creditor reference stands for, of this could be parsed.
            /// (Only numerical creditor references can be reverted to an id).
            /// </summary>
            public long? Id { get; set; }
        }

        public enum CreditReferenceErrorCause
        {
            NoError = 0,
            InvalidLength = 1,
            ChecksumFailed = 2,
            UnknownPattern = 3,
        }

        #endregion

        #region Private implementation

        private static string FilterAlfaNum(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;

            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z')
                {
                    sb.Append(c);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    sb.Append((char)(c - 32));
                }
            }

            return sb.ToString();
        }

        private static string Grouped(string str, int groupSize, char separator)
        {
            var result = new StringBuilder(str.Length);
            int index = 0;
            for(int i=0; i<(str.Length-1); i++)
            {
                var c = str[i];
                result.Append(c);
                index++;
                if (index == groupSize)
                {
                    index = 0;
                    result.Append(separator);
                }
            }
            result.Append(str[str.Length - 1]);
            return result.ToString();
        }

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
    }
}
