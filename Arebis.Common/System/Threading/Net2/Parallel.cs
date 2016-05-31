using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Threading.Net2
{
	public delegate void ForDelegate(int i);

	public delegate void ThreadDelegate();

    [Obsolete("Prefer using the System.Threading.Parallel class from System.Threading.dll in the Microsoft Parallel Extensions.", false)]
	public static class Parallel
	{
		/// <summary>
		/// Parallel ForEach method.
		/// </summary>
		/// <remarks>Source: http://www.codeproject.com/KB/dotnet/PoorMansParallelForEach.aspx</remarks>
		public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
		{
			int threadCount = Environment.ProcessorCount;

			object syncRoot = new object();

			if (enumerable == null) return;

			IEnumerator<T> enumerator = enumerable.GetEnumerator();

			InvokeAsync<T> del = InvokeAction;

			T[] seedItemArray = new T[threadCount];
			List<IAsyncResult> resultList = new List<IAsyncResult>(threadCount);

			for (int i = 0; i < threadCount; i++)
			{
				bool moveNext;

				lock (syncRoot)
				{
					moveNext = enumerator.MoveNext();
					seedItemArray[i] = enumerator.Current;
				}

				if (moveNext)
				{
					IAsyncResult iAsyncResult = del.BeginInvoke
			 (enumerator, action, seedItemArray[i], syncRoot, i, null, null);
					resultList.Add(iAsyncResult);
				}
			}

			foreach (IAsyncResult iAsyncResult in resultList)
			{
				del.EndInvoke(iAsyncResult);
				iAsyncResult.AsyncWaitHandle.Close();
			}
		}

		internal delegate void InvokeAsync<T>(IEnumerator<T> enumerator, Action<T> achtion, T item, object syncRoot, int i);

		static void InvokeAction<T>(IEnumerator<T> enumerator, Action<T> action, T item, object syncRoot, int i)
		{
			if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
				Thread.CurrentThread.Name =
			String.Format("Parallel.ForEach Worker Thread No:{0}", i);

			bool moveNext = true;

			while (moveNext)
			{
				action.Invoke(item);

				lock (syncRoot)
				{
					moveNext = enumerator.MoveNext();
					item = enumerator.Current;
				}
			}
		}

		/// <summary>
		/// Parallel for loop. Invokes given action, passing arguments 
		/// fromInclusive - toExclusive on multiple threads.
		/// Returns when loop finished.
		/// </summary>
		/// <remarks>Source: http://coding-time.blogspot.com/2008/03/implement-your-own-parallelfor-in-c.html</remarks>
		public static void For(int fromInclusive, int toExclusive, ForDelegate action)
		{
			// ChunkSize = 1 makes items to be processed in order.
			// Bigger chunk size should reduce lock waiting time and thus
			// increase paralelism.
			int chunkSize = 4;

			// number of process() threads
			int threadCount = Environment.ProcessorCount;
			int cnt = fromInclusive - chunkSize;

			// processing function
			// takes next chunk and processes it using action
			ThreadDelegate process = delegate()
			{
				while (true)
				{
					int cntMem = 0;
					lock (typeof(Parallel))
					{
						// take next chunk
						cnt += chunkSize;
						cntMem = cnt;
					}
					// process chunk
					// here items can come out of order if chunkSize > 1
					for (int i = cntMem; i < cntMem + chunkSize; ++i)
					{
						if (i >= toExclusive) return;
						action(i);
					}
				}
			};

			// launch process() threads
			IAsyncResult[] asyncResults = new IAsyncResult[threadCount];
			for (int i = 0; i < threadCount; ++i)
			{
				asyncResults[i] = process.BeginInvoke(null, null);
			}
			// wait for all threads to complete
			for (int i = 0; i < threadCount; ++i)
			{
				process.EndInvoke(asyncResults[i]);
			}
		}
	}
}
