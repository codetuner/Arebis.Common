using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Algorithms
{
    /// <summary>
    /// Implementation to help detect overlapping situations.
    /// Add elements to the detector with a start and end value, and it will
    /// tell if the value overlaps with a previous one.
    /// </summary>
    [Serializable]
    public class OverlappingDetector<T>
    {
        public OverlappingDetector()
        {
            this.Registrations = new List<Tuple<T, int, int>>();
        }

        /// <summary>
        /// Registrations made to the OverlappingDetector.
        /// </summary>
        public List<Tuple<T, int, int>> Registrations { get; protected set; }

        /// <summary>
        /// Add an item to the OverlappingDetector with a startValue and an endValue.
        /// Returns true if the startValue and endValue overlap with a previous item.
        /// </summary>
        /// <param name="item">Informative related item.</param>
        /// <param name="startValue">Start value or lower bound.</param>
        /// <param name="endValue">End value or upper bound.</param>
        /// <returns>True if item overlaps with a previously added one, else false.</returns>
        public bool Add(T item, int startValue, int endValue)
        {
            var outcome = false;

            var newRegistration = new Tuple<T,int,int>(item, startValue, endValue);

            foreach (var registration in Registrations)
            {
                if (Overlaps(registration, newRegistration))
                {
                    outcome = true;
                    break;
                }
            }

            Registrations.Add(newRegistration);

            return outcome;
        }

        /// <summary>
        /// Provided that item was added earlier, returns the other overlapping items.
        /// May return duplicate values when items overlap more than once.
        /// </summary>
        public IEnumerable<T> OverlapsWith(T item)
        {
            foreach (var first in Registrations)
            {
                if (!first.Equals(item)) continue;

                foreach (var second in Registrations)
                {
                    if (second.Equals(item)) continue;

                    if (Overlaps(first, second))
                        yield return second.Item1;
                }
            }
        }

        protected bool Overlaps(Tuple<T, int, int> first, Tuple<T, int, int> second)
        {
            return Overlaps(first.Item2, first.Item3, second.Item2, second.Item3);
        }

        protected virtual bool Overlaps(int start1, int end1, int start2, int end2)
        {
            // If start is >= end, no overlap:
            if (start1 >= end2)
                return false;

            // If end <= start, no overlap:
            if (end1 <= start1)
                return false;

            return true;
        }
    }
}
