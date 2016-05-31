using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Commandline;

// Disable warning CS0649: Field is never assigned to
#pragma warning disable 0649

namespace Arebis.Extensions.Tests.Arebis.Runtime.Commandline.Sample
{
	class TestOptions
	{
		[CommandHelp(new string[] { "?", "h", "help" }, "Shows this help")]
		public bool HelpRequested;

		[CommandInfo("Input file")]
		[CommandArg(5, "Inputfile", true)]
		public string InputFile;

		[CommandInfo("Output file")]
		[CommandArg(15, "Outputfile", false)]
		public string OutputFile;

		[CommandInfo("Format of the generated file")]
		[CommandOption("format", TextFormats.Unicode, ShortName="f")]
		[CommandOptionValue(TextFormats.Ansii, "win", "Windows (ANSI) format")]
		[CommandOptionValue(TextFormats.Ascii, "dos", "DOS (ASCII) format")]
		[CommandOptionValue(TextFormats.Unicode, "unicode", "Unicode format")]
		public TextFormats Format;

		[CommandInfo("Overwrite readonly files")]
		[CommandOption("o", false)]
		public bool OverwriteReadonly;
	}

	class TestOptionsExt : TestOptions
	{
		[CommandInfo("Parameter files")]
		[CommandArgList("paramfiles", false)]
		public IList<string> ParamFiles;

		private string logFile;

		[CommandInfo("Log file")]
		[CommandOption("logfile", null, ShortName="l")]
		public string LogFile
		{
			get { return logFile; }
			set { logFile = value; }
		}
	}
}
