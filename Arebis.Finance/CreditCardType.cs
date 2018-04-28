using Arebis.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Finance
{
    /// <summary>
    /// Type/brand of a credit card.
    /// </summary>
    public enum CreditCardType
    {
        /// <summary>
        /// Unknown credit card type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// American Express.
        /// </summary>
        [MetaData("IdentificationRegex", "^3(4|7)[0-9,X]{13}$")] // https://www.cybersource.com/developers/getting_started/test_and_manage/best_practices/card_type_id/
        AmericanExpress = 1,

        /// <summary>
        /// Diners Club or Carte Blanche.
        /// </summary>
        [MetaData("IdentificationRegex", "^3(0|6|8)[0-9,X]{12}$")]
        DinersClub = 2,

        /// <summary>
        /// Discover.
        /// </summary>
        Discover = 3,

        /// <summary>
        /// JCB.
        /// </summary>
        JCB = 4,

        /// <summary>
        /// Mastercard.
        /// </summary>
        [MetaData("IdentificationRegex", "^(5[1-5]|2[2-7])[0-9,X]{14}$")]
        Mastercard = 5,

        /// <summary>
        /// VISA or Visa Electron.
        /// </summary>
        [MetaData("IdentificationRegex", "^4[0-9,X]{11,18}$")]
        VISA = 6,

        /// <summary>
        /// Maestro.
        /// </summary>
        [MetaData("IdentificationRegex", "^(5(0|[6-9])|6(4|[6-9]))[0-9,X]{10,17}$")]
        Maestro = 7,

        /// <summary>
        /// UnionPay.
        /// </summary>
        UnionPay = 8,

        //...

        /// <summary>
        /// An other card type.
        /// </summary>
        Other = 9998,

        /// <summary>
        /// An invalid cart type.
        /// </summary>
        Invalid = 9999
    }
}
