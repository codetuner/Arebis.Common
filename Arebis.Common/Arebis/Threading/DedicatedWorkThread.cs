using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Arebis.Threading
{
    /// <summary>
    /// A DedicatedWorkThread is a WorkThread dedicated to one single, embedded work queue.
    /// This thread handles only the embedded queue, and the embedded queue can only be handled
    /// by the thread.
    /// </summary>
    /// <typeparam name="T">Type of workitems.</typeparam>
    public class DedicatedWorkThread<T> : MarshalByRefObject, IDisposable where T : class
    {
        private Object syncRoot = new Object();
        private SynchronisedQueue<T> workQueue = new SynchronisedQueue<T>();
        private Thread workThread = null;
        private WorkItemHandler<T> workItemHandler;
        private int keepAliveMillis;
        private volatile int joinlevel = 0;
        private bool disposed = false;

        /// <summary>
        /// Event raised when a workitem handling is started.
        /// </summary>
        public event WorkItemEventHandler<T> Started;

        /// <summary>
        /// Event raised when a workitem handling is succesfully ended.
        /// </summary>
        public event WorkItemEventHandler<T> Terminated;

        /// <summary>
        /// Event raised when the work thread is (re)activated first time or after idle time.
        /// </summary>
        public event EventHandler Reactivate;

        /// <summary>
        /// Event raised when the work thread turns in idle state (typically when no work was provided for a keepAlive time).
        /// </summary>
        public event EventHandler Idle;

        /// <summary>
        /// Event raised when an exception occured while handling a workitem.
        /// </summary>
        public event WorkItemExceptionEventHandler<T> Failed;

        /// <summary>
        /// Instantiates a new DedicatedWorkThread given the WorkItemHandler.
        /// </summary>
        /// <param name="workItemHandler">Handler that will handle WorkItems.</param>
        /// <param name="keepAliveMillis">Milliseconds to keep the thread alive when idle, -1 to keep infinitely alive.</param>
        /// <param name="threadName">Optional name of the dedicated work thread.</param>
        public DedicatedWorkThread(WorkItemHandler<T> workItemHandler, int keepAliveMillis, string threadName = null)
        {
            this.workItemHandler = workItemHandler;
            this.keepAliveMillis = keepAliveMillis;
            this.ThreadName = threadName ?? Guid.NewGuid().ToString();
        }

        public string ThreadName { get; private set; }

        public bool IsJoining
        {
            get
            {
                return (this.joinlevel > 0);
            }
        }

        /// <summary>
        /// Adds a workItem to the embedded work queue.
        /// The workItem will automatically be handled as soon as it is next
        /// in the queue.
        /// </summary>
        public virtual void AddWork(T workItem)
        {
            lock (this.syncRoot)
            {
                // Check not joining:
                if (this.joinlevel > 0)
                    throw new InvalidOperationException("Cannot add work to a DedicatedWorkThread on which a Join operation is running.");

                // Check not disposed:
                if (this.disposed)
                    throw new InvalidOperationException("Cannot add work to a DedicatedWorkThread that is disposed.");

                // Enqueue the workitem:
                this.workQueue.Enqueue(workItem);

                // Initiate thread if needed:
                if (this.workThread == null)
                {
                    this.OnReactivate(EventArgs.Empty);

                    this.workThread = new Thread(this.HandleQueue);
                    this.workThread.Name = this.ThreadName;
                    this.workThread.IsBackground = true;
                    this.workThread.Start();
                }
            }
        }

        /// <summary>
        /// Flushes the workQueue, emptying the list of non-started workItems.
        /// </summary>
        public void Flush()
        {
            this.workQueue.Clear();
        }

        /// <summary>
        /// Join to the WorkThread, waiting until all work is handled.
        /// While joining, no new items can be added to the queue.
        /// </summary>
        public virtual void Join()
        {
            try
            {
                Thread t = null;
                lock (this.syncRoot)
                {
                    // Increase joinLevel:
                    this.joinlevel++;

                    // Retrieve thread:
                    t = this.workThread;
                }

                // Join thread:
                if (t != null)
                {
                    this.workQueue.Release(t);
                    t.Join();
                }
            }
            finally
            {
                lock (this.syncRoot)
                {
                    // Decrease joinLevel:
                    this.joinlevel--;
                }
            }
        }

        /// <summary>
        /// Join to the WorkThread, waiting unit all work is handled or timeout occurs.
        /// While joining, no new items can be added to the queue.
        /// </summary>
        /// <param name="timeOut">Timeout to wait for work to be handled.</param>
        /// <returns>True if all work handled, false if timeout reached.</returns>
        public virtual bool Join(TimeSpan timeOut)
        {
            try
            {
                Thread t = null;
                lock (this.syncRoot)
                {
                    // Increase joinLevel:
                    this.joinlevel++;

                    // Retrieve thread:
                    t = this.workThread;
                }

                // Join thread:
                if (t != null)
                {
                    this.workQueue.Release(t);
                    return t.Join(timeOut);
                }
                else
                {
                    return true;
                }
            }
            finally
            {
                lock (this.syncRoot)
                {
                    // Decrease joinLevel:
                    this.joinlevel--;
                }
            }
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        public override string ToString()
        {
            if (this.workThread == null)
                return ("WorkQueue (idle)");
            else
                return ("WorkQueue " + this.workThread.Name);
        }

        /// <summary>
        /// Disposes the WorkThread, forcing all queued work to be aborted.
        /// </summary>
        public virtual void Dispose()
        {
            lock (this.syncRoot)
            {
                if (this.workThread != null)
                    this.workThread.Abort();
                this.workThread = null;
                this.workQueue.Clear();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Handles start of handling a set of workItems.
        /// </summary>
        protected virtual void OnReactivate(EventArgs e)
        {
            if (this.Reactivate == null)
                return;

            foreach (EventHandler item in this.Reactivate.GetInvocationList())
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
                catch
                {
                }
            }
        }

        /// <summary>
        /// Handles end of handling a set of workItems.
        /// </summary>
        protected virtual void OnIdle(EventArgs e)
        {
            if (this.Idle == null)
                return;

            foreach (EventHandler item in this.Idle.GetInvocationList())
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
                catch
                {
                }
            }
        }

        /// <summary>
        /// Handles start of handling individual workItems.
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
                catch
                {
                }
            }
        }

        /// <summary>
        /// Handles succesful end of handling individual workItems.
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
                catch
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
                catch
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
                T workItem = this.workQueue.Dequeue(this.keepAliveMillis);

                // Handle workitem (if any):
                if (workItem != null)
                    this.HandleWorkItem(workItem);

                // Check if abort needed:
                lock (this.syncRoot)
                {
                    if (((workItem == null) || (this.joinlevel > 0)) && (this.workQueue.IsEmpty))
                    {
                        // Reset state and abort:
                        this.workThread = null;
                        break;
                    }
                }
            }

            // Signal worker thread ended:
            this.OnIdle(EventArgs.Empty);
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
