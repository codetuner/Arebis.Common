using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Pdf.Model
{
    internal static class StringExtensions
    {
        public static string CaseTranslate(this string str, StringComparison comparisonType, params string[] cases)
        {
            // Search for a matching case:
            for (int i = 0; i < cases.Length - 1; i += 2)
            {
                if (String.Equals(str, cases[i], comparisonType))
                    return cases[i + 1];
            }

            // If no matching case found, return default value:
            if ((cases.Length % 2) == 1)
                return cases[cases.Length - 1];
            else
                return null;
        }
    }
}
