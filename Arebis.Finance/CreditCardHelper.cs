using Arebis.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arebis.Finance
{
    /// <summary>
    /// Helper methods to operate credit cards.
    /// </summary>
    public static class CreditCardHelper
    {
        // http://www.validcreditcardnumber.com/
        // http://blog.unibulmerchantservices.com/how-to-recognize-a-valid-credit-card/
        // https://www.regular-expressions.info/creditcard.html

        private static Dictionary<CreditCardType, Regex> CreditCardIdentificationRegexes = null;

        /// <summary>
        /// Tries to identify the type of credit card given it's number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static CreditCardType IdentifyType(string number)
        {
            if (String.IsNullOrWhiteSpace(number))
                return CreditCardType.Invalid;

            if (CreditCardIdentificationRegexes == null)
            {
                var regexes = new Dictionary<CreditCardType, Regex>();
                foreach (var member in typeof(CreditCardType).GetMembers())
                {
                    foreach (MetaDataAttribute rxattr in MetaDataAttribute.GetCustomAttributes(member, typeof(MetaDataAttribute)).Where(a => ((MetaDataAttribute)a).Name == "IdentificationRegex"))
                    {
                        regexes[(CreditCardType)Enum.Parse(typeof(CreditCardType), member.Name)] = new Regex((string)rxattr.Value, RegexOptions.Compiled);
                    }
                }

                CreditCardIdentificationRegexes = regexes;
            }

            foreach(var pair in CreditCardIdentificationRegexes)
            {
                if (pair.Value.IsMatch(number.Replace(" ", "").ToUpperInvariant())) return pair.Key;
            }

            return CreditCardType.Unknown;
        }
    }
}
