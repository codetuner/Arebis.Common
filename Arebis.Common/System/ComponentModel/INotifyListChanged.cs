using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace System.ComponentModel
{
	/// <summary>
	/// Notifies clients of changes in the list.
	/// </summary>
	public interface INotifyListChanged
	{
		/// <summary>
		/// Occurs when the list changes or an item in the list changes.
		/// </summary>
		event ListChangedEventHandler ListChanged;
	}
}
