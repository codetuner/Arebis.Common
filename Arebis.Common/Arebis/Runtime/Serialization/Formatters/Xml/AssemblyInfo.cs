using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Arebis.Runtime.Serialization.Formatters.Xml
{
    /// <summary>
    /// Holds information about an assembly.
    /// </summary>
	/// <remarks>
	/// Copied from VisualStudio PowerToys project.
	/// Initial version created by Koen Callens
	/// </remarks>
    internal class AssemblyInfo
    {
        public const string VERSION = "Version";
        public const string CULTURE = "Culture";
        public const string PUBLIC_KEY_TOKEN = "PublicKeyToken";
        public const string PROCESSOR_ARCHITECTURE = "processorArchitecture";

		#region fields

		private string name;
		private Dictionary<string, string> fields;

		#endregion fields

		#region constructors

		/// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfo"/> class.
        /// </summary>
        /// <param name="fullQualifiedName">Full qualified name of the assembly.</param>
		/// <remarks>
		/// The full qualified name has to start with the assembly name.
		/// </remarks>
        public AssemblyInfo(string fullQualifiedName)
        {
            this.fields = new Dictionary<string, string>();
            string[] parts = fullQualifiedName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            // Fill information fields
            this.name = parts[0];
            for (int i = 1 ; i < parts.Length ; i++) {
                string part = parts[i];
                string key = part.Substring(0, part.IndexOf("=")).Trim();
                string value = part.Substring(part.IndexOf("=") + 1).Trim();
                if (!value.Equals("null", StringComparison.InvariantCultureIgnoreCase)) //Only add fields that doesn't have a "null" value
                    this.fields.Add(key, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfo"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AssemblyInfo(Assembly assembly)
            : this(assembly.FullName) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyInfo"/> class.
		/// </summary>
		/// <param name="assemblyName">The assembly name.</param>
		public AssemblyInfo(AssemblyName assemblyName)
			: this(assemblyName.FullName) { }

        #endregion constructors

		#region properties

		/// <summary>
		/// Gets the name of the assembly.
		/// </summary>
		/// <value>The assembly name.</value>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Gets the fields.
		/// </summary>
		/// <value>The fields.</value>
		public Dictionary<string, string> Fields
		{
			get { return this.fields; }
		}

		#endregion properties

        #region public methods
        /// <summary>
        /// Determines whether the assembly contains the field with the given name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>
        /// 	<c>true</c> if the assembly contains the field; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsField(string name)
        {
            return this.fields.ContainsKey(name);
        }

        /// <summary>
        /// Gets the field with the given name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>The value of the field.</returns>
        public string GetField(string name)
        {
            if (this.fields.ContainsKey(name))
                return this.fields[name];
            return null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(this.name);
            if (this.ContainsField(VERSION))
                result.Append(string.Format(", {0}={1}", VERSION, this.GetField(VERSION)));
            if (this.ContainsField(CULTURE))
                result.Append(string.Format(", {0}={1}", CULTURE, this.GetField(CULTURE)));
            if (this.ContainsField(PUBLIC_KEY_TOKEN))
                result.Append(string.Format(", {0}={1}", PUBLIC_KEY_TOKEN, this.GetField(PUBLIC_KEY_TOKEN)));
            
            return result.ToString();
        }

		/// <summary>
		/// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data
		/// structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:AssemblyInfo"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            AssemblyInfo info = (AssemblyInfo)obj;

            if (!info.Name.Equals(this.Name))
                return false;
            if (!this.isFieldEqual(this, info, VERSION))
                return false;
            if (!this.isFieldEqual(this, info, CULTURE))
                return false;
            if (!this.isFieldEqual(this, info, PUBLIC_KEY_TOKEN))
                return false;

            return true;
        }

        #endregion public methods

        #region private methods

        /// <summary>
        /// Determines whether two field values are equal.
        /// </summary>
        /// <param name="original">The original assembly information.</param>
        /// <param name="compareTo">The compare to assembly information.</param>
        /// <param name="fieldName">Name of the field to compare.</param>
        /// <returns>
        /// 	<c>true</c> if the field values are identical; otherwise, <c>false</c>.
        /// </returns>
        private bool isFieldEqual(AssemblyInfo original, AssemblyInfo compareTo, string fieldName)
        {
            bool originalContainsField = original.ContainsField(fieldName);
            bool compareToContainsField = compareTo.ContainsField(fieldName);

			// The original assembly info doesn't contain the field,
			// so any value in the compare to assembly info is valid.
			if (!originalContainsField)
				return true;
			// Both do not contain the field -> equal
			if (!originalContainsField && !compareToContainsField)
				return true;
			// Both contain the field -> check if content is equal
			if (originalContainsField && compareToContainsField)
				return original.GetField(fieldName).Equals(compareTo.GetField(fieldName));
			// The original assembly info contains the field, but the compare to assembly info doesn't -> NOT equal
			return false;
        }

        #endregion private methods
    }
}
