using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static
{
    /// <summary>
    /// Extension methods for handling lists of Method objects.
    /// </summary>
    public static class MethodListExtensions
    {
        /// <summary>
        /// Returns the Method for the given MethodBase.
        /// Returns null if not found.
        /// </summary>
        public static ModelMethod ForMethodBase(this IEnumerable<ModelMethod> methodList, MethodBase methodBase)
        {
            foreach (ModelMethod method in methodList)
                if (method.MethodBase == methodBase)
                    return method;
            return null;
        }

        /// <summary>
        /// Returns methods on given declaring type.
        /// </summary>
        public static IEnumerable<ModelMethod> WhereDeclaringTypeIs(this IEnumerable<ModelMethod> methodList, Type declaringType)
        {
            foreach (ModelMethod method in methodList)
                if (method.DeclaringType == declaringType)
                    yield return method;
        }

        /// <summary>
        /// Returns methods with given tag.
        /// </summary>
        public static IEnumerable<ModelMethod> WhereTagsContains(this IEnumerable<ModelMethod> methodList, string tag)
        {
            foreach (ModelMethod method in methodList)
                if (method.Tags.Contains(tag))
                    yield return method;
        }
    }
}
