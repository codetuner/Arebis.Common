using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Reflection
{
	/// <summary>
	/// Class providing default code language information.
	/// </summary>
	[Serializable]
	public class DefaultLanguageInfo : ILanguageInfo
	{
		private Dictionary<string, string[]> namespaces = new Dictionary<string, string[]>();

		/// <summary>
		/// Registers a namespace.
		/// </summary>
		public void RegisterNamespace(string namespase)
		{
			this.namespaces[namespase] = new string[] { namespase };
		}

		/// <summary>
		/// Registers a namespace with an alias.
		/// </summary>
		public void RegisterNamespace(string namespase, string alias)
		{
			this.namespaces[namespase] = new string[] { namespase, alias };
		}

		/// <summary>
		/// Returns a friendlyname for the given type.
		/// </summary>
		public string GetFiendlyName(Type forType)
		{
			Type[] genericArgs = forType.GetGenericArguments();

			if (forType.IsByRef)
			{
				return "ref " + this.GetFiendlyName(forType.GetElementType());
			}
			else if (forType.IsGenericType == false)
			{
				return this.GetFiendlyName(forType.ToString());
			}
			else
			{
				StringBuilder nameBuilder = new StringBuilder();
				nameBuilder.Append(this.GetFiendlyName(StringUpTo(forType.GetGenericTypeDefinition().FullName, "`")));
				nameBuilder.Append('<');
				nameBuilder.Append(this.GetFiendlyName(genericArgs[0]));
				for (int i = 1; i < genericArgs.Length; i++)
				{
					nameBuilder.Append(", ");
					nameBuilder.Append(this.GetFiendlyName(genericArgs[i]));
				}
				nameBuilder.Append('>');
				return nameBuilder.ToString();
			}
		}

		/// <summary>
		/// Returns a friendlyname for the given type.
		/// </summary>
		protected virtual string GetFiendlyName(string forTypeNamed)
		{
			int dotpos = forTypeNamed.LastIndexOf('.');
			if (dotpos == -1) return forTypeNamed;

			string ns = forTypeNamed.Substring(0, dotpos);
			string tn = forTypeNamed.Substring(dotpos + 1);

			string[] nsdef;
			if (this.namespaces.TryGetValue(ns, out nsdef))
			{
				if (nsdef.Length > 1)
					return nsdef[1] + "." + tn;
				else
					return tn;
			}
			else
				return forTypeNamed;
		}

		private static string StringUpTo(string str, string upto)
		{
			int index = str.IndexOf(upto);
			if (index == -1)
				return str;
			else
				return str.Substring(0, index);
		}
	}
}
