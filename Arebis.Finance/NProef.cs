using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Finance
{
    /// <summary>
    /// Implements the Dutch "NegenProef", "ElfProef", ... "NProef" checksum algorithm.
    /// </summary>
    public class NProef
    {
        public NProef(int n, int[] weights)
        {
            this.N = n;

            if (weights == null || weights.Length == 0) weights = new int[] { 1 };
            this.Weights = weights;
        }

        public int N { get; private set; }

        public int[] Weights { get; private set; }

        public int GetWeightedSum(long value)
        {
            var sum = 0;
            var pos = 0;

            //for (int i = value.Length-1; i >= 0; i--)
            //{
            //    int rem = value[i] - '0';
            //    var factor = this.Weights[pos++ % this.Weights.Length];
            //    sum += (rem * factor);
            //    Console.WriteLine("+ ({0} x {1}) = {2}", rem, factor, sum);
            //}

            while (value > 0)
            {
                long rem;
                value = Math.DivRem(value, 10L, out rem);
                var factor = this.Weights[pos % this.Weights.Length];
                sum += ((int)rem * factor);
                Debug.WriteLine("+ ({0} x {1}) = {2}", rem, factor, sum);
                pos++;
            }

            return sum;
        }

        public int GetWeightedSumRemainder(long value)
        {
            var sum = GetWeightedSum(value);
            int rem;
            Math.DivRem(sum, this.N, out rem);
            return (this.N - rem);
        }

        public bool Verify(long value)
        {
            var sum = GetWeightedSum(value);
            //Console.WriteLine((sum % this.N));
            return ((sum % this.N) == 0);
        }
    }
}
