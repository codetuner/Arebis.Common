using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Xml
{
	/// <summary>
	/// An XmlResolver that resolves URI of type "res://AssemblyName/ResourceName" to retrieve
	/// resources in assemblies.
	/// </summary>
	/// <remarks>
	/// The assembly containing the resource must be preloaded.
	/// </remarks>
	/// <example>
	/// Consider an assembly named MyCompany.XsltLibs in which two Xslt file are embedded as resource,
	/// one called "MyTemplate.xsl" and the other called "Utilities.xsl".
	/// The MyTemplate.xsl can now be envoked as follows:
	/// <code>
	///		XslCompiledTransform transformer = new XslCompiledTransform();
	///		transformer.Load(
	///			"res://MyCompany.XsltLibs/MyCompany.XsltLibs.MyTemplate.xsl",
	///			new XsltSettings(),
	///			new XmlResourceResolver()
	///		);
	/// </code>
	/// Inside the template, it would also be valid to include resources as follows:
	/// <code>
	///   	&lt;xsl:include href="res://MyCompany.XsltLibs/MyCompany.XsltLibs.Utilities.xsl"/&gt;
	/// </code>
	/// Note that the resource URI is made of "res://" followed by the assemblyname without file extension,
	/// separated by a slash from the full resource name being constructed from the default namespace of the assembly,
	/// eventually followed by the folder structure the file is in, followed by the filename.
	/// </example>
	public class XmlResourceResolver : XmlUrlResolver
	{
		/// <summary>
		/// Maps a URI to an object containing the actual resource.
		/// </summary>
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri.Scheme == "res")
			{
				string assemblyName = absoluteUri.Host;
				string resourceName = absoluteUri.AbsolutePath;

				// Remove initial slashes from resource name:
				while (resourceName.StartsWith("/"))
				{
					resourceName = resourceName.Substring(1);
				}

				// Identifie assembly containing the resource:
				Assembly resourceAssembly = Assembly.GetEntryAssembly();
				foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (asm.GetName().Name.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
					{
						resourceAssembly = asm;
						break;
					}
				}

				// Return stream to resource:
				return resourceAssembly.GetManifestResourceStream(resourceName);
			}
			else
			{
				return base.GetEntity(absoluteUri, role, ofObjectToReturn);
			}
		}
	}
}
