using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Arebis.Runtime.Serialization.Formatters.Xml
{
	internal class TypeParser
	{
		#region constructors

		public TypeParser() {
		}

		#endregion constructors

		#region public methods

		/// <summary>
		/// Resolves the specified type name.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns></returns>
		public Type Resolve(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentException("string typeName should not be null or empty.", "typeName");

			AssembyResolver assemblyResolver = new AssembyResolver();

			Type resolvedType = null;
			List<int> arrayDimensionLengths = this.GetArrayDimensions(typeName);
			typeName = this.trimArrayDimensions(typeName);
			if (typeName.Contains("`")) { // Is generic type?
				if (typeName.Contains("]")) { // Is not a generic type definition?
					int indexBeginTypeArguments = typeName.IndexOf("[");
					int assemblyNameStartIndex = this.getMatchingClosingBracketIndex(typeName, indexBeginTypeArguments);
					if (-1 != typeName.IndexOf(',', assemblyNameStartIndex))
						assemblyNameStartIndex = typeName.IndexOf(',', assemblyNameStartIndex) + 1;
					string genericTypeDefinition = typeName.Substring(0, indexBeginTypeArguments).Trim();
					string qualifiedGenericTypeDefinition = genericTypeDefinition;
					if (-1 != assemblyNameStartIndex && typeName.Length-1 != assemblyNameStartIndex )
						qualifiedGenericTypeDefinition += string.Format(", {0}", typeName.Substring(assemblyNameStartIndex).Trim());
					Type typeDefinition = this.Resolve(qualifiedGenericTypeDefinition);
					
					string typeArguments = typeName.Substring(indexBeginTypeArguments + 1, this.getMatchingClosingBracketIndex(typeName, indexBeginTypeArguments) - (indexBeginTypeArguments + 1));
					List<Type> typeArgumentList = new List<Type>();
					int i = 0;
					while (i < typeArguments.Length) {
						if (',' == typeArguments[i]) {
							i++;
							continue;
						}

						int indexBeginTypeArgument = typeArguments.IndexOf("[", i);
						int indexEndTypeArgument = this.getMatchingClosingBracketIndex(typeArguments, indexBeginTypeArgument);
						string typeArgumentName = typeArguments.Substring(indexBeginTypeArgument + 1, indexEndTypeArgument - (indexBeginTypeArgument + 1));
						Type typeArgument = this.Resolve(typeArgumentName);
						typeArgumentList.Add(typeArgument);

						i = indexEndTypeArgument + 1;
					}

					resolvedType = typeDefinition.MakeGenericType(typeArgumentList.ToArray());
					if (null != arrayDimensionLengths)
						resolvedType = resolvedType.MakeArrayType(arrayDimensionLengths.Count);

					return resolvedType;
				}
			}

			string typeNamePart = typeName;
			string assemblyNamePart = null;
			int assemblyNamePartStartIndex = typeName.IndexOf(","); ;
			if (-1 != assemblyNamePartStartIndex) {
				typeNamePart = typeName.Substring(0, assemblyNamePartStartIndex).Trim();
				assemblyNamePart = typeName.Substring(assemblyNamePartStartIndex + 1).Trim();
			}

			if (!string.IsNullOrEmpty(assemblyNamePart)) {
				Assembly assembly = assemblyResolver.Resolve(assemblyNamePart);
				resolvedType = assembly.GetType(typeNamePart);
				if (null != arrayDimensionLengths)
					resolvedType = resolvedType.MakeArrayType(arrayDimensionLengths.Count);

				return resolvedType;
			}

			// No assembly specified, so look in loaded assemblies.
			foreach (Assembly loadedAssembly in assemblyResolver.GetLoadedAssemblies()) {
				Type type = loadedAssembly.GetType(typeNamePart);
				if (null != type) {
					resolvedType = type;
					if (null != arrayDimensionLengths)
						resolvedType = resolvedType.MakeArrayType(arrayDimensionLengths.Count);

					return resolvedType;
				}
			}
	
			// The type is still not found, let the framework resolve the type.
			resolvedType = Type.GetType(typeNamePart);
			if (null != resolvedType) {
				if (null != arrayDimensionLengths)
					resolvedType = resolvedType.MakeArrayType(arrayDimensionLengths.Count);

				return resolvedType;
			}

			throw new Exception(string.Format("The type '{0}' could not be resolved.", typeName));
		}

		public List<int> GetArrayDimensions(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentException("string typeName should not be null or empty.", "typeName");

			List<int> dimensions = null; // Return value
			Regex expression = new Regex(@"\[(?:[\d\*]*,?)+]");
			Match arrayMatch = expression.Match(typeName);
			if (arrayMatch.Success) {
				dimensions = new List<int>();
				expression = new Regex(@"[\d\*]+");
				MatchCollection dimensionMatches = expression.Matches(arrayMatch.Value);
				if (0 != dimensionMatches.Count) {
					foreach (Match dimension in dimensionMatches) {
						int dimensionLength = 0;
						if (!string.IsNullOrEmpty(dimension.Value) && dimension.Value != "*")
							dimensionLength = int.Parse(dimension.Value);
						dimensions.Add(dimensionLength);
					}
				}
				else {
					foreach (char c in arrayMatch.Value) {
						if (',' == c)
							dimensions.Add(0);
					}

					if (0 != dimensions.Count)
						dimensions.Add(0);
				}
			}

			return dimensions;
		}

		#endregion public methods

		#region private methods

		private int getMatchingClosingBracketIndex(string text, int indexOpeningBracket)
		{
			if (string.IsNullOrEmpty(text))
				throw new ArgumentException("text");
			if (indexOpeningBracket < 0 || text.Length < indexOpeningBracket)
				throw new IndexOutOfRangeException();
			if ('[' != text[indexOpeningBracket])
				throw new ArgumentException("indexOpeningBracket");

			int indexClosingBracket = indexOpeningBracket; // Return value
			int bracketCounter = 0;
			for (int i = indexOpeningBracket; i < text.Length; i++) {
				switch (text[i]) {
					case '[':
						bracketCounter++;
						break;
					case ']':
						bracketCounter--;
						break;
					default:
						break;
				}

				if (0 == bracketCounter) {
					indexClosingBracket = i;
					break;
				}
			}

			if (0 != bracketCounter)
				throw new Exception("No matching closing bracket found.");

			return indexClosingBracket;
		}

		private string trimArrayDimensions(string typeName)
		{
			string result = typeName; // Return value

			Regex expression = new Regex(@"\[(?:[\d\*]*,?)+]");
			Match arrayMatch = expression.Match(typeName);
			if (arrayMatch.Success)
				result = expression.Replace(typeName, "");

			return result;
		}

		#endregion private methods
	}
}
