using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Arebis.Runtime
{
	/// <summary>
	/// A disposable path based assembly resolver.
	/// When installed, attempts to resolve unresolved assemblies by using the
	/// directories in the given paths.
	/// </summary>
	public class AssemblyResolver : MarshalByRefObject, IDisposable
	{
		private bool resolverInstalled = false;
		private string[] paths = null;

		/// <summary>
		/// Constructs an empty path based AssemblyResolver.
		/// </summary>
		public AssemblyResolver()
		{ 
		}

		/// <summary>
		/// Constructs a path based AssemblyResolver with the given semi-colon delimited
		/// list of paths.
		/// </summary>
		public AssemblyResolver(string pathsSemiColonDelimited)
			: this(pathsSemiColonDelimited.Split(';'))
		{ }

		/// <summary>
		/// Constructs a path based AssemblyResolver with the given list of paths.
		/// </summary>
		public AssemblyResolver(string[] paths)
		{
			this.Paths = paths;
		}

		/// <summary>
		/// Paths that are searched by the AssemblyResolver.
		/// </summary>
		public string[] Paths
		{
			get { return this.paths; }
			set {
				this.paths = value;
				if (value != null) this.InstallResolver(); else this.UninstallResolver();
			}
		}

		/// <summary>
		/// Installs the resolver eventhandler on the appdomain.
		/// </summary>
		protected void InstallResolver()
		{
			if (this.resolverInstalled == true) return;
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.AssemblyResolve);
			this.resolverInstalled = true;
		}

		/// <summary>
		/// Installs the resolver eventhandler on the appdomain.
		/// </summary>
		protected void UninstallResolver()
		{
			if (resolverInstalled == false) return;
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.AssemblyResolve);
			resolverInstalled = false;
		}

		/// <summary>
		/// Disposes the resolver by removing the eventhandler from the appdomain.
		/// </summary>
		public void Dispose()
		{
			this.UninstallResolver();
		}

		/// <summary>
		/// Attempts to resolve assemblies based on the given path.
		/// </summary>
		protected virtual Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (this.paths != null)
			{
				string filename = args.Name.Split(',')[0];
				foreach (string path in paths)
				{
					if (File.Exists(Path.Combine(path, filename + ".dll")))
					{
						return Assembly.LoadFile(Path.Combine(path, filename + ".dll"));
					}
					else if (File.Exists(Path.Combine(path, filename + ".exe")))
					{
						return Assembly.LoadFile(Path.Combine(path, filename + ".exe"));
					}
				}
			}
			return null;
		}
	}
}
