using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections.ObjectModel;
using System.IO;
using Arebis.CodeAnalysis.Static.Processors;
using Arebis.Reflection;

namespace Arebis.CodeAnalysis.Static
{
    /// <summary>
    /// A StaticCodeAnalyzerSession represents a session processable
    /// by a StaticCodeAnalyzer.
    /// </summary>
    public class StaticCodeAnalyzerSession
    {
        private HashSet<Assembly> assemblies = new HashSet<Assembly>();
        private List<IProcessor> processors = new List<IProcessor>();
        private ILanguageInfo languageInfo = new DefaultLanguageInfo();
        private IAnalyzerFilter analyzerFilter = null;

        /// <summary>
        /// Constructs a blank session.
        /// </summary>
        public StaticCodeAnalyzerSession()
        {
        }

        /// <summary>
        /// Constructs a session given a definition file.
        /// </summary>
        public StaticCodeAnalyzerSession(string sessionDefinitionFile)
        {
            XmlDocument sessionDefinition = new XmlDocument();
            sessionDefinition.Load(sessionDefinitionFile);
            this.Load(sessionDefinition);
        }

        /// <summary>
        /// Constructs a session given a definition.
        /// </summary>
        public StaticCodeAnalyzerSession(XmlDocument sessionDefinition)
        {
            this.Load(sessionDefinition);
        }

        /// <summary>
        /// Language information associated with this analyzer session.
        /// </summary>
        public ILanguageInfo LanguageInfo
        {
            get { return this.languageInfo; }
            set { this.languageInfo = value; }
        }

        /// <summary>
        /// Filter that can determine to skip processing of
        /// assemblies/types/methods.
        /// </summary>
        public IAnalyzerFilter AnalyzerFilter
        {
            get { return this.analyzerFilter; }
            set { this.analyzerFilter = value; }
        }

        /// <summary>
        /// Assemblies loaded in this analyzer session.
        /// </summary>
        public IEnumerable<Assembly> Assemblies
        {
            get { return this.assemblies; }
        }

        /// <summary>
        /// Processors registered in this analyzer session.
        /// </summary>
        public IEnumerable<IProcessor> Processors
        {
            get { return this.processors; }
        }

        /// <summary>
        /// Adds the given assembly to the analyzer session.
        /// </summary>
        /// <param name="asm">The assembly to add.</param>
        public void AddAssembly(Assembly asm)
        {
            this.AddAssembly(asm, false);
        }

        /// <summary>
        /// Adds the given assembly to the analyzer session.
        /// </summary>
        /// <param name="asm">The assembly to add.</param>
        /// <param name="includeReferencedAssemblies">Whether all referenced assemblies
        /// of the given assembly must be added recursively.</param>
        public void AddAssembly(Assembly asm, bool includeReferencedAssemblies)
        {
            if (!this.assemblies.Contains(asm))
            {
                this.assemblies.Add(asm);
                if (includeReferencedAssemblies)
                {
                    foreach (var asmRef in asm.GetReferencedAssemblies())
                    {
                        this.AddAssembly(Assembly.Load(asmRef), includeReferencedAssemblies);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the given processor to the analyzer session.
        /// </summary>
        /// <param name="processor"></param>
        public void AddProcessor(IProcessor processor)
        {
            this.processors.Add(processor);
        }

        #region Private implementation

        private void Load(XmlDocument sessionDefinition)
        {
            // Load assemblies:
            foreach (XmlNode path in sessionDefinition.DocumentElement.SelectNodes(@"/analyzer/assemblies/assembly"))
            {
                string file = path.Attributes["file"].Value;
                //Console.WriteLine("Loading: {0}", file);
                this.assemblies.Add(Assembly.Load(file));
            }
            foreach (XmlNode path in sessionDefinition.DocumentElement.SelectNodes(@"/analyzer/assemblies/filepatterns"))
            {
                //Console.WriteLine(">> {0}", path.Attributes["path"].Value);
                DirectoryInfo dir = new DirectoryInfo(path.Attributes["path"].Value);
                foreach (XmlNode filepattern in path.SelectNodes(@"add"))
                {
                    string pattern = filepattern.Attributes["value"].Value;
                    foreach (FileInfo file in dir.GetFiles(pattern))
                    {
                        //Console.WriteLine("Loading: {0}", file.FullName);
                        this.assemblies.Add(Assembly.LoadFrom(file.FullName));
                    }
                }
            }

            // Load language:
            XmlNode languageNode = sessionDefinition.DocumentElement.SelectSingleNode(@"/analyzer/language");
            if (languageNode != null)
            {
                // Retrieve language:
                if (languageNode.Attributes["code"] != null)
                {
                    switch (languageNode.Attributes["code"].Value.ToUpper())
                    {
                        case "CS":
                        case "C#":
                            this.languageInfo = new CsLanguageInfo();
                            break;
                        case "VB":
                        case "VB.NET":
                        case "VISUALBASIC":
                        case "VISUALBASIC.NET":
                            this.languageInfo = new VbLanguageInfo();
                            break;
                        default:
                            this.languageInfo = new DefaultLanguageInfo();
                            break;
                    }
                }

                // Retrieve imports:
                foreach (XmlNode importNode in languageNode.SelectNodes("import"))
                {
                    if (importNode.Attributes["alias"] == null)
                    {
                        this.languageInfo.RegisterNamespace(
                            importNode.Attributes["namespace"].Value);
                    }
                    else
                    {
                        this.languageInfo.RegisterNamespace(
                            importNode.Attributes["namespace"].Value,
                            importNode.Attributes["alias"].Value);
                    }
                }
            }

            // Load processors:
            foreach (XmlNode processorDef in sessionDefinition.DocumentElement.SelectNodes(@"/analyzer/processors/*[count(@handler) = 1]"))
            {
                // Instantiate and initialize processor:
                Type processorType = Type.GetType(processorDef.Attributes["handler"].Value);
                IProcessor processor = (IProcessor)Activator.CreateInstance(processorType);
                processor.Initialize(processorDef);

                // Register processor:
                this.processors.Add(processor);
            }
        }

        #endregion
    }
}
