using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Arebis.Threading
{
	/// <summary>
	/// A WorkThread handles a items in a WorkQueue on a separate thread.
	/// </summary>
	/// <typeparam name="T">Type of workitems.</typeparam>
	public class WorkThread<T> : MarshalByRefObject, IDisposable where T : class
	{
		private Thread workThread = null;
		private WorkItemHandler<T> workItemHandler;
		private bool disconnecting = false;
		private SynchronisedQueue<T> workQueue;

		/// <summary>
		/// Event raised when a workitem handling is started.
		/// </summary>
		public event WorkItemEventHandler<T> Started;

		/// <summary>
		/// Event raised when a workitem handling is succesfully ended.
		/// </summary>
		public event WorkItemEventHandler<T> Terminated;

		/// <summary>
		/// Event raised when an exception occured while handling a workitem.
		/// </summary>
		public event WorkItemExceptionEventHandler<T> Failed;

		/// <summary>
		/// Instantiates a new WorkThread given the WorkItemHandler.
		/// </summary>
		/// <param name="workQueue">The queue containing workitems.</param>
		/// <param name="workItemHandler">Handler that will handle WorkItems.</param>
		public WorkThread(SynchronisedQueue<T> workQueue, WorkItemHandler<T> workItemHandler)
		{
			// Initialize:
			this.workQueue = workQueue;
			this.workItemHandler = workItemHandler;

			// Start workthread:
			this.workThread = new Thread(this.HandleQueue);
			this.workThread.Name = Guid.NewGuid().ToString();
			this.workThread.IsBackground = true;
			this.workThread.Start();
		}

		/// <summary>
		/// The workQueue the WorkThread is connected to.
		/// </summary>
		public SynchronisedQueue<T> WorkQueue
		{
			get { return this.workQueue; }
		}

		/// <summary>
		/// Disconnects the WorkThread gracefully from the work queue. As soon as this
		/// method is called, the thread will not accept new workItems anymore. The currently
		/// handled workItem will be completed. The method waits for completion of the currently
		/// handled workItem or for timeout.
		/// </summary>
		/// <param name="millisecondsTimeout">Timeout to wait for the handling of 
		/// the current workItem to end. -1 to wait endefinitely.
		/// </param>
		/// <returns>True if the thread is idle immediately after this call;
		/// false if the thread is still busy handling it's last workitem while
		/// timout has been reached.</returns>
		public virtual bool Disconnect(int millisecondsTimeout)
		{
			this.disconnecting = true;

			this.workQueue.Release(this.workThread);

			return this.workThread.Join(millisecondsTimeout);
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		public override string ToString()
		{
			return ("WorkThread " + this.workThread.Name);
		}

		/// <summary>
		/// Disposes the WorkQueue, forcing all queued workitems to be aborted.
		/// </summary>
		public virtual void Dispose()
		{
			if (this.workThread != null)
			{
				this.workThread.Abort();
				this.workThread = null;
			}
		}

		/// <summary>
		/// Handles start of handling workItems.
		/// </summary>
		protected virtual void OnStarted(WorkItemEventArgs<T> e)
		{
			if (this.Started == null)
				return;

			foreach (WorkItemEventHandler<T> item in this.Started.GetInvocationList())
			{
				try
				{
					ISynchronizeInvoke syncItem = (item.Target as ISynchronizeInvoke);
					if ((syncItem != null) && (syncItem.InvokeRequired))
					{
						syncItem.BeginInvoke(item, new object[] { this, e });
					}
					else
					{
						item(this, e);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		/// <summary>
		/// Handles succesful end of handling workItems.
		/// </summary>
		protected virtual void OnTerminated(WorkItemEventArgs<T> e)
		{
			if (this.Terminated == null)
				return;

			foreach (WorkItemEventHandler<T> item in this.Terminated.GetInvocationList())
			{
				try
				{
					ISynchronizeInvoke syncItem = (item.Target as ISynchronizeInvoke);
					if ((syncItem != null) && (syncItem.InvokeRequired))
					{
						syncItem.BeginInvoke(item, new object[] { this, e });
					}
					else
					{
						item(this, e);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		/// <summary>
		/// Handles exceptions when handling workItems.
		/// </summary>
		protected virtual void OnFailed(WorkItemExceptionEventArgs<T> e)
		{
			if (this.Failed == null)
				return;

			foreach (WorkItemExceptionEventHandler<T> item in this.Failed.GetInvocationList())
			{
				try
				{
					ISynchronizeInvoke syncItem = (item.Target as ISynchronizeInvoke);
					if ((syncItem != null) && (syncItem.InvokeRequired))
					{
						syncItem.BeginInvoke(item, new object[] { this, e });
					}
					else
					{
						item(this, e);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		/// <summary>
		/// Internal implementation of the work queue.
		/// </summary>
		protected virtual void HandleQueue()
		{
			while (true)
			{
				// Retrieve & dequeue workitem:
				T workItem = this.workQueue.Dequeue(-1);

				// Handle workitem (if any):
				if (workItem != null)
					this.HandleWorkItem(workItem);

				// Abort of disconnecting:
				if (this.disconnecting)
					break;
			}
		}

		/// <summary>
		/// Handles the given workItem.
		/// </summary>
		protected void HandleWorkItem(T workItem)
		{
			try
			{
				this.OnStarted(new WorkItemEventArgs<T>(workItem));

				if (this.workItemHandler != null)
					this.workItemHandler(workItem);

				this.OnTerminated(new WorkItemEventArgs<T>(workItem));
			}
			catch (Exception ex)
			{
				this.OnFailed(new WorkItemExceptionEventArgs<T>(workItem, ex));
			}
		}
	}
}
