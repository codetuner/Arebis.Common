using System;
using System.Reflection;

namespace Arebis.Runtime.Serialization.Formatters.Xml
{
    internal class AssembyResolver
    {
        #region constructors

        public AssembyResolver()
        {
        }

        #endregion constructors

        #region public methods

        /// <summary>
        /// Resolves the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The assembly with the specified name.</returns>
        public Assembly Resolve(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentException("string assemblyName should not be null or empty.", "assemblyName");

            AssemblyInfo assemblyInfo = new AssemblyInfo(assemblyName);

            foreach (Assembly loadedAssembly in this.GetLoadedAssemblies())
            {
                AssemblyInfo loadedAssemblyInfo = new AssemblyInfo(loadedAssembly);
                if (assemblyInfo.Equals(loadedAssemblyInfo))
                    return loadedAssembly;
            }

            foreach (AssemblyName referencedAssemblyName in Assembly.GetCallingAssembly().GetReferencedAssemblies())
            {
                AssemblyInfo referencedAssemblyInfo = new AssemblyInfo(referencedAssemblyName);
                if (assemblyInfo.Equals(referencedAssemblyInfo))
                    return Assembly.Load(referencedAssemblyName);
            }

            throw new Exception(string.Format("The assembly '{0}' could not be resolved.", assemblyInfo.ToString()));
        }

        /// <summary>
        /// Gets the loaded assemblies of the current application domain.
        /// </summary>
        /// <returns>The loaded assemblies of the current application domain.</returns>
        public Assembly[] GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        #endregion public methods
    }
}
