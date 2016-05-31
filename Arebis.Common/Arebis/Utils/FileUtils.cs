using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Arebis.Utils
{
	/// <summary>
	/// Utility class containing several static utility related to the files or the file system.
	/// </summary>
	public static class FileUtils
	{
		/// <summary>
		/// Returns the full filepath of an existing file matching the given filename
		/// in one of the given path directories. Returns null if no file found.
		/// </summary>
		public static string FindInPath(string filename, string[] path)
		{
			// No filename results in null path:
			if ((filename == null) || (filename.Length == 0)) return null;

			// Search each path directory for the file:
			foreach (string dir in path)
			{ 
				string file = Path.Combine(dir, filename);
				if (File.Exists(file)) return file;
			}

			// Return null if not found:
			return null;
		}

		/// <summary>
		/// Returns the full filepath of an existing file matching the given filename
		/// in one of the given path directories. Returns null if no file found.
		/// </summary>
		public static string FindInPath(string filename, string path)
		{
			return FindInPath(filename, path.Split(';'));
		}

		/// <summary>
		/// Returns the full filepath of an existing file matching the given filename
		/// combined with the PATH environment variable. Returns null if no file found.
		/// </summary>
		public static string FindInPath(string filename)
		{
			return FindInPath(filename, Environment.GetEnvironmentVariable("PATH"));
		}
	}
}
