using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Types
{
    /// <summary>
    /// A MultiCounter is a set of counters where when the first overflows, the next is increased.
    /// </summary>
    public class MultiCounter
    {
        private int[] values;

        /// <summary>
        /// Creates a rollingover MultiCounter with the given amplitudes.
        /// </summary>
        /// <param name="amplitudes">Amplitudes. I.e. (10,10,10) creates a MultiCounter with 3 counters each amplitude 10 (thus each having values from 0 to 9 included).</param>
        public MultiCounter(params int[] amplitudes)
            : this(amplitudes, true)
        { }

        /// <summary>
        /// Creates a MultiCounter with the given amplitudes.
        /// </summary>
        /// <param name="amplitudes">Amplitudes. I.e. new int[] {10,10,10} creates a MultiCounter with 3 counters each amplitude 10 (thus each having values from 0 to 9 included).</param>
        /// <param name="rollingOver">Whether the last counter should also roll over: when it reaches it's maximum, set all values to 0 and proceed.</param>
        /// <exception cref="System.OverflowException">Thrown when over maximum and not rolling over.</exception>
        public MultiCounter(int[] amplitudes, bool rollingOver)
        {
            this.values = new int[amplitudes.Length];
            this.Amplitudes = amplitudes;
            this.RollingOver = rollingOver;
        }

        /// <summary>
        /// Whether the last counter should also roll over: when it reaches it's maximum, set all values to 0 and proceed.
        /// </summary>
        public bool RollingOver { get; set; }

        /// <summary>
        /// Amplitudes of this MultiCounter.
        /// </summary>
        public int[] Amplitudes { get; private set; }

        /// <summary>
        /// Actual values of the counters.
        /// </summary>
        /// <param name="position">Index of the counter.</param>
        /// <returns>The actual value of the indexed counter.</returns>
        public int this[int position]
        {
            get
            {
                return this.values[position];
            }
            set
            {
                this.values[position] = value;
            }
        }

        /// <summary>
        /// Increments the MultiCounter with the given value.
        /// </summary>
        public MultiCounter Increment(int withValue = 1)
        {
            if (withValue < 0) return this.Decrement(-withValue);

            var toadd = withValue;
            for (int c = 0; c < values.Length; c++)
            {
                var newval = values[c] + toadd;
                if (newval < Amplitudes[c])
                {
                    values[c] = newval;
                    return this;
                }
                else
                {
                    values[c] = newval % Amplitudes[c];
                    toadd = newval / Amplitudes[c];
                }
            }

            if (toadd > 0 && !RollingOver)
            {
                throw new OverflowException("MultiCounter overflowed.");
            }

            return this;
        }

        /// <summary>
        /// Decrements the MultiCounter with the given value.
        /// </summary>
        public MultiCounter Decrement(int withValue = 1)
        {
            if (withValue < 0) return this.Increment(-withValue);

            var torem = withValue;
            for (int c = 0; c < values.Length; c++)
            {
                var newval = values[c] - torem;
                if (newval >= 0)
                {
                    values[c] = newval;
                    return this;
                }
                else
                {
                    var newvalc = newval % Amplitudes[c];
                    var toremnext = 0;
                    if (newvalc < 0)
                    {
                        values[c] = Amplitudes[c] + newvalc;
                        toremnext = 1;
                    }
                    else
                    {
                        values[c] = newvalc;
                    }
                    torem = toremnext + ((-newval) / Amplitudes[c]);
                }
            }

            if (torem > 0 && !RollingOver)
            {
                throw new OverflowException("MultiCounter overflowed.");
            }

            return this;
        }

        /// <summary>
        /// Resets all counters to 0.
        /// </summary>
        public MultiCounter Reset()
        {
            for (int c = 0; c < values.Length; c++)
            {
                values[c] = 0;
            }

            return this;
        }

        public override string ToString()
        {
            return "[" + String.Join(",", this.values.Select(v => v.ToString())) + "]";
        }

        public static IEnumerable<int[]> Enumerate(params int[] amplitudes)
        {
            return Enumerate(amplitudes, 1);
        }

        /// <summary>
        /// Enumerates all values 
        /// </summary>
        /// <param name="amplitudes">Amplitudes. I.e. (10,10,10) creates a MultiCounter with 3 counters each amplitude 10 (thus each having values from 0 to 9 included).</param>
        /// <param name="stepSize">Step size (must be 1 or lager).</param>
        /// <returns></returns>
        public static IEnumerable<int[]> Enumerate(int[] amplitudes, int stepSize)
        {
            if (stepSize <= 0) throw new ArgumentOutOfRangeException("stepSize", "StepSize cannot be 0 or negative.");

            var maxValue = 1;
            for (int i = 0; i < amplitudes.Length; i++)
            {
                maxValue = maxValue * amplitudes[i];
            }

            var values = new int[amplitudes.Length];

            yield return values;

            for (int v = 0; v < maxValue; v += stepSize)
            {

                var toadd = stepSize;
                for (int c = 0; c < values.Length; c++)
                {
                    var newval = values[c] + toadd;
                    if (newval < amplitudes[c])
                    {
                        values[c] = newval;
                        yield return values;
                        break;
                    }
                    else
                    {
                        values[c] = newval % amplitudes[c];
                        toadd = newval / amplitudes[c];
                    }
                }
            }
        }

        public MultiCounter Reversed()
        {
            var reversedAmplitudes = Amplitudes.Reverse().ToArray();
            var reversedMultiCounter = new MultiCounter(reversedAmplitudes, this.RollingOver);
            reversedMultiCounter.values = this.values.Reverse().ToArray();
            return reversedMultiCounter;
        }
    }
}
