using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Arebis.Threading
{
	/// <summary>
	/// A SynchronisedQueue is a queue in which multiple threads can enqueue
	/// and dequeue items safely.
	/// </summary>
	/// <typeparam name="T">Type of queue members.</typeparam>
	public class SynchronisedQueue<T> where T : class
	{
		private Object syncRoot = new Object();
		private Queue<T> internalQueue = new Queue<T>();
		private EventWaitHandle queueSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
		private List<Thread> waitingThreads = new List<Thread>();

		/// <summary>
		/// Adds an element to the queue.
		/// </summary>
		public void Enqueue(T item)
		{
			lock (this.syncRoot)
			{
				this.internalQueue.Enqueue(item);
				this.queueSignal.Set();
			}
		}

		/// <summary>
		/// Removes and returns the oldest not dequeued element of the queue.
		/// If no element is available, the Dequeue method waits up to the given
		/// timeout, and returns null if the timeout was reached.
		/// </summary>
		/// <param name="millisecondsTimeout">Timeout to wait for an element to be
		/// added to the queue. -1 is infinite.</param>
		/// <returns>The dequeued item, or null if timeout was reached.</returns>
		public T Dequeue(int millisecondsTimeout)
		{
			// Reset waithandle if queue empty:
			lock (this.syncRoot)
			{
				if (this.internalQueue.Count == 0)
					this.queueSignal.Reset();
			}

			// Wait for a signal (allow the thread to be interrupted):
			try
			{
				// Allow thread to be interrupted (released):
				lock (this.syncRoot)
				{
					this.waitingThreads.Add(Thread.CurrentThread);
				}

				// Wait for a signal an item is queued:
				this.queueSignal.WaitOne(millisecondsTimeout, false);
			}
			catch (ThreadInterruptedException)
			{
				// If thread has been interrupted; just continue.
			}
			finally
			{
				// Disallow thread to be interrupted (released):
				lock (this.syncRoot)
				{
					this.waitingThreads.Remove(Thread.CurrentThread);
				}

				// 'Eat' pending interrupt requests:
				Thread.Sleep(0);
			}

			// Return the dequeued item:
			lock (this.syncRoot)
			{
				if (this.internalQueue.Count > 0)
				{
					return this.internalQueue.Dequeue();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Clears the queue.
		/// </summary>
		public void Clear()
		{
			lock (this.syncRoot)
			{
				this.internalQueue.Clear();
			}
		}

		/// <summary>
		/// Releases the given thread if it is locked in a Dequeue call.
		/// </summary>
		public void Release(Thread waitingThread)
		{
			lock (this.syncRoot)
			{
				if (this.waitingThreads.Contains(waitingThread))
					waitingThread.Interrupt();
			}
		}

		/// <summary>
		/// Whether the queue is empty at this moment.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				lock (this.syncRoot)
				{
					return (this.internalQueue.Count == 0);
				}
			}
		}
	}
}
