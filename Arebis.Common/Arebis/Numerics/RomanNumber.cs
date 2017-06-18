using Arebis.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// Converts to and from Roman numbers.
    /// </summary>
    public static class RomanNumber
    {
        private static string[][] romanNumerals = null;
        private static Dictionary<char, int> CharValues = null;

        /// <summary>
        /// Converts an integer to a Roman number.
        /// </summary>
        /// <param name="number">Integer value between 1 and 9999.</param>
        /// <returns>String with Roman representation.</returns>
        [CodeSource("http://stackoverflow.com/a/7445709")]
        public static string From(int number)
        {
            if (romanNumerals == null)
            {
                romanNumerals = new string[][]
                {
                    new string[]{"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
                    new string[]{"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
                    new string[]{"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
                    new string[]{"", "M", "MM", "MMM", "MMMM", "MMMMM", "MMMMMM", "MMMMMMM", "MMMMMMMM", "MMMMMMMMM"} // thousands
                };
            }

            if (number < 1 || number > 9999)
                throw new ArgumentOutOfRangeException("number", "Number must be between 1 and 9999.");

            // split integer string into array and reverse array
            var intArr = number.ToString().Reverse().ToArray();
            var len = intArr.Length;
            var romanNumeral = new StringBuilder();
            var i = len;

            // starting with the highest place (for 3046, it would be the thousands 
            // place, or 3), get the roman numeral representation for that place 
            // and add it to the final roman numeral string
            while (i-- > 0)
            {
                romanNumeral.Append(romanNumerals[i][Int32.Parse(intArr[i].ToString())]);
            }

            return romanNumeral.ToString();
        }

        /// <summary>
        /// Converts a Roman number to its integer value.
        /// </summary>
        [CodeSource("http://csharphelper.com/blog/2016/04/convert-to-and-from-roman-numerals-in-c/")]
        public static int Parse(string roman)
        {
            // Initialize the letter map.
            if (CharValues == null)
            {
                CharValues = new Dictionary<char, int>();
                CharValues.Add('I', 1);
                CharValues.Add('V', 5);
                CharValues.Add('X', 10);
                CharValues.Add('L', 50);
                CharValues.Add('C', 100);
                CharValues.Add('D', 500);
                CharValues.Add('M', 1000);
            }

            if (roman.Length == 0) return 0;
            roman = roman.ToUpper();

            // See if the number begins with (.
            if (roman[0] == '(')
            {
                // Find the closing parenthesis.
                int pos = roman.LastIndexOf(')');

                // Get the value inside the parentheses.
                string part1 = roman.Substring(1, pos - 1);
                string part2 = roman.Substring(pos + 1);
                return 1000 * Parse(part1) + Parse(part2);
            }

            // The number doesn't begin with (.
            // Convert the letters' values.
            int total = 0;
            int last_value = 0;
            for (int i = roman.Length - 1; i >= 0; i--)
            {
                int new_value = CharValues[roman[i]];

                // See if we should add or subtract.
                if (new_value < last_value)
                    total -= new_value;
                else
                {
                    total += new_value;
                    last_value = new_value;
                }
            }

            // Return the result.
            return total;
        }
    }
}
