using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Threading
{
	/// <summary>
	/// Delegate handler of workitems.
	/// </summary>
	/// <typeparam name="T">Type of workitem.</typeparam>
	public delegate void WorkItemHandler<T>(T workItem) where T : class;

	/// <summary>
	/// Delegate handler of workitem events.
	/// </summary>
	/// <typeparam name="T">Type of workitem.</typeparam>
	public delegate void WorkItemEventHandler<T>(object sender, WorkItemEventArgs<T> e) where T : class;

	/// <summary>
	/// Delegate handler of workitem failure events.
	/// </summary>
	/// <typeparam name="T">Type of workitem.</typeparam>
	public delegate void WorkItemExceptionEventHandler<T>(object sender, WorkItemExceptionEventArgs<T> e) where T : class;

	/// <summary>
	/// WorkItem handling event arguments.
	/// </summary>
	/// <typeparam name="T">Type of workitem.</typeparam>
	public class WorkItemEventArgs<T> : EventArgs where T : class
	{
		private T workItem;

		/// <summary>
		/// Creates a new WorkItemEventArgs object.
		/// </summary>
		public WorkItemEventArgs(T workItem)
		{
			this.workItem = workItem;
		}

		/// <summary>
		/// The workitem.
		/// </summary>
		public T WorkItem
		{
			get { return workItem; }
		}
	}

	/// <summary>
	/// WorkItem handling exception event arguments.
	/// </summary>
	/// <typeparam name="T">Type of workitem.</typeparam>
	public class WorkItemExceptionEventArgs<T> : WorkItemEventArgs<T> where T : class
	{
		private Exception exception;

		/// <summary>
		/// Creates a new WorkItemExceptionEventArgs object.
		/// </summary>
		public WorkItemExceptionEventArgs(T workItem, Exception exception)
			: base(workItem)
		{
			this.exception = exception;
		}

		/// <summary>
		/// The exception.
		/// </summary>
		public Exception Exception
		{
			get { return exception; }
		}
	}
}
