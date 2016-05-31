using System;

namespace Arebis.Reflection
{
	/// <summary>
	/// Defines a type providing information about a code language.
	/// </summary>
	public interface ILanguageInfo
	{
		/// <summary>
		/// Registers a namespace to be assumed imported.
		/// </summary>
		void RegisterNamespace(string namespase);

		/// <summary>
		/// Registers a namespace to be assumed imported with a given alias.
		/// </summary>
		void RegisterNamespace(string namespase, string alias);

		/// <summary>
		/// Returns a friendly name for the given type.
		/// </summary>
		string GetFiendlyName(Type forType);
	}
}
