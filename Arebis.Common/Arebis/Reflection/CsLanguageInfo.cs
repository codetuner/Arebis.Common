using System;

namespace Arebis.Reflection
{
	/// <summary>
	/// Class providing code language information for the C# language.
	/// </summary>
	[Serializable]
	public class CsLanguageInfo : DefaultLanguageInfo
	{
		/// <summary>
		/// Returns a friendlyname for the given type.
		/// </summary>
		protected override string GetFiendlyName(string forTypeNamed)
		{
			if (forTypeNamed == "System.Void")
				return "void";
			else if (forTypeNamed == "System.Int32")
				return "int";
			else if (forTypeNamed == "System.String")
				return "string";
			else if (forTypeNamed == "System.Boolean")
				return "bool";
			else if (forTypeNamed == "System.Object")
				return "object";
			else if (forTypeNamed == "System.Char")
				return "char";
			else if (forTypeNamed == "System.Double")
				return "double";
			else if (forTypeNamed == "System.Single")
				return "float";
			else if (forTypeNamed == "System.Decimal")
				return "decimal";
			else if (forTypeNamed == "System.Byte")
				return "byte";
			else if (forTypeNamed == "System.Int16")
				return "short";
			else if (forTypeNamed == "System.Int64")
				return "long";
			else
				return base.GetFiendlyName(forTypeNamed);
		}
	}
}
