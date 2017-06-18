using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class HashSetExtensions
    {
        /// <summary>
        /// Generates a unique identifier, a unique value based on the previous values and a given proposed value.
        /// If no value is proposed, a value in the form "idX" (where X is a number) is generated.
        /// If a value is proposed that is not unique, it is extended with a number to get a unique value.
        /// </summary>
        public static string UniqueIdentifier(this HashSet<string> previousValues, string value = null)
        {
            if (value == null)
            {
                value = "id" + previousValues.Count;
            }

            if (!previousValues.Contains(value))
            {
                previousValues.Add(value);
                return value;
            }
            else
            {
                int c = 2;
                while (true)
                {
                    value = value + c++;
                    if (!previousValues.Contains(value))
                    {
                        previousValues.Add(value);
                        return value;
                    }
                }
            }
        }
    }
}
