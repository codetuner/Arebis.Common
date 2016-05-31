using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Arebis.Reflection;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using Arebis.CodeAnalysis.Static;
using System.Threading;
using Arebis.CodeAnalysis.Static.Processors;
using System.Diagnostics;
using System.Linq;

namespace Arebis.CodeAnalysis.Static
{
    /// <summary>
    /// A StaticCodeAnalyzer constructs a codemodel containing an 
    /// in-memory method call networks for code analysis.
    /// </summary>
    public class StaticCodeAnalyzer
    {
        /// <summary>
        /// Process the given session and return a codemodel containing
        /// the in-memory method call network.
        /// </summary>
        public CodeModel Process(StaticCodeAnalyzerSession session)
        {
            List<ModelAssembly> massemblies = new List<ModelAssembly>();
            List<ModelType> mtypes = new List<ModelType>();
            Dictionary<string, ModelMethod> mmethods = new Dictionary<string, ModelMethod>();
            ILanguageInfo languageInfo = session.LanguageInfo;
            IAnalyzerFilter analyzerFilter = session.AnalyzerFilter;

            // Retrieve all methods and constructors:
            foreach (Assembly asm in session.Assemblies)
            {
                try
                {
                    if ((analyzerFilter != null)
                        && (!analyzerFilter.ProcessAssembly(asm)))
                        continue;

                    ModelAssembly masm = new ModelAssembly(asm, languageInfo);
                    massemblies.Add(masm);

                    foreach (Type type in asm.GetTypes())
                    {
                        try
                        {
                            if ((analyzerFilter != null)
                                && (!analyzerFilter.ProcessType(type)))
                                continue;

                            ModelType mtype = new ModelType(masm, type, languageInfo);
                            mtypes.Add(mtype);

                            foreach (MethodBase mb in type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                            {
                                if ((analyzerFilter != null)
                                    && (!analyzerFilter.ProcessMethod(mb)))
                                    continue;

                                mmethods[GetMethodKey(mb)] = new ModelMethod(mtype, mb, languageInfo);
                            }
                            foreach (MethodBase mb in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                            {
                                if ((analyzerFilter != null)
                                    && (!analyzerFilter.ProcessMethod(mb)))
                                    continue;

                                mmethods[GetMethodKey(mb)] = new ModelMethod(mtype, mb, languageInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Rethrow with mode info:
                            throw new TargetInvocationException(
                                String.Format("Error reading type {0}: {1} (See innerException.)", type.FullName, ex.Message),
                                ex
                            );
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Rethrow with mode info:
                    throw new TargetInvocationException(
                        String.Format("Error reading assembly {0}: {1} (See innerException.)", asm.FullName, ex.Message),
                        ex.LoaderExceptions.FirstOrDefault() ?? ex
                    );

                }
                catch (Exception ex)
                {
                    // Rethrow with mode info:
                    throw new TargetInvocationException(
                        String.Format("Error reading assembly {0}: {1} (See innerException.)", asm.FullName, ex.Message),
                        ex
                    );
                }
            }

            // Build network of method calls:
            foreach (ModelMethod m in mmethods.Values)
            {
                try
                {
                    MethodBodyReader reader = new MethodBodyReader(m.MethodBase);

                    foreach (MethodBase calledmb in reader.GetCalledMethods(true, true))
                    {
                        ModelMethod calledm = FindMethod(mmethods, calledmb);
                        if (calledm != null)
                        {
                            m.CallsMethods.Add(calledm);
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            // Construct a code model:
            CodeModel codeModel = new CodeModel(
                massemblies,
                mtypes,
                mmethods.Values);

            // Apply processors:
            foreach (IProcessor processor in session.Processors)
            {
                processor.Process(codeModel);
            }

            // Construct & return a code model:
            return codeModel;
        }

        #region Private implementation

        private static ModelMethod FindMethod(Dictionary<string, ModelMethod> methods, MethodBase methodBase)
        {
            ModelMethod result;
            if (methods.TryGetValue(GetMethodKey(methodBase), out result))
                return result;
            else
                return null;
        }

        private static string GetMethodKey(MethodBase methodBase)
        {
            StringBuilder key = new StringBuilder(64);
            key.Append(methodBase.DeclaringType);
            key.Append('.');
            key.Append(methodBase.Name);
            foreach (ParameterInfo param in methodBase.GetParameters())
            {
                key.Append('(');
                key.Append(param.ParameterType);
            }
            return key.ToString();
        }

        #endregion
    }
}
