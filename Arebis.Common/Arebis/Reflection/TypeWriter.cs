using System;
using System.IO;
using System.Text;

namespace Arebis.Reflection
{
	/// <summary>
	/// Writes a string representation of types with the requested attributes,
	/// such that the type can be recovered (assuming it has sufficient attributes)
	/// using the Type.GetType() method. Allows writing generic and non generic
	/// types with or without assemly name/version/culture/public key information.
	/// </summary>
	public class TypeWriter
	{
		private bool withAssembly = true;
		private bool withVersion = false;
		private bool withCulture = false;
		private bool withPublicKeyToken = false;

		/// <summary>
		/// Whether the assembly name should be written.
		/// </summary>
		public bool WithAssembly
		{
			get { return withAssembly; }
			set { withAssembly = value; }
		}

		/// <summary>
		/// Whether the version number of the assembly should be written.
		/// </summary>
		public bool WithVersion
		{
			get { return withVersion; }
			set { withVersion = value; }
		}

		/// <summary>
		/// Whether the culture of the assembly should be written.
		/// </summary>
		public bool WithCulture
		{
			get { return withCulture; }
			set { withCulture = value; }
		}

		/// <summary>
		/// Whether the public key token of the assembly should be written.
		/// </summary>
		public bool WithPublicKeyToken
		{
			get { return withPublicKeyToken; }
			set { withPublicKeyToken = value; }
		}

		/// <summary>
		/// Writes the given type. Returns a string representation of the type.
		/// </summary>
		public string WriteType(Type t)
		{
			StringBuilder sb = new StringBuilder();
			this.WriteType(t, sb);
			return sb.ToString();
		}

		/// <summary>
		/// Writes the type to the given target.
		/// </summary>
		public void WriteType(Type t, TextWriter target)
		{
			StringBuilder sb = new StringBuilder();
			this.WriteType(t, sb);
			target.Write(sb.ToString());
		}

		/// <summary>
		/// Writes the type to the given target.
		/// </summary>
		public void WriteType(Type t, StringBuilder target)
		{
			// Get the base type (if array, the type of the array):
			Type baseType = (t.IsArray) ? t.GetElementType() : t;

			// Write typename:
			if ((baseType.IsGenericType) && (!baseType.IsGenericTypeDefinition))
			{
				target.Append(baseType.GetGenericTypeDefinition().FullName);
				string sep = "";
				target.Append("[[");
				foreach (Type gt in baseType.GetGenericArguments())
				{
					target.Append(sep);
					this.WriteType(gt, target);
					sep = "],[";
				}
				target.Append("]]");
			}
			else
			{
				target.Append(baseType.FullName);
			}

			// If original type is array, prepend with array marker:
			if (t.IsArray)
			{
				target.Append("[]");
			}

			// Write requested assembly information:
			string[] assemblyRef = baseType.Assembly.FullName.Split(',');
			if (this.withAssembly)
			{
				target.Append(", ");
				target.Append(assemblyRef[0]);
				// equivalent to: t2.Assembly.GetName().Name
			}
			if (this.withVersion && (assemblyRef.Length >= 2))
			{
				target.Append(",");
				target.Append(assemblyRef[1]);
			}
			if (this.withCulture && (assemblyRef.Length >= 3))
			{
				target.Append(",");
				target.Append(assemblyRef[2]);
			}
			if (this.withPublicKeyToken && (assemblyRef.Length >= 4))
			{
				target.Append(",");
				target.Append(assemblyRef[3]);
			}
		}
	}
}
