using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class QueueExtensions
    {
        /// <summary>
        /// Dequeues the next item or returns default value if queue is empty.
        /// </summary>
        public static TItem DequeueOrDefault<TItem>(this Queue<TItem> queue, TItem ifEmpty = default(TItem))
        {
            if (queue.Count > 0)
                return queue.Dequeue();
            else
                return ifEmpty;
        }
    }
}
