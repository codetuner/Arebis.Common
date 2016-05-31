using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Arebis.Runtime.Commandline;

// Disable warning CS0649: Field is never assigned to
#pragma warning disable 0649

namespace ArebisTestApp
{
	class Options {

		[CommandHelp(new string[] { "?", "h", "help" }, "Shows this help")]
		public bool HelpRequested;

		[CommandInfo("Input file")]
		[CommandArg(1, "input", true)]
		public string InputFile;

		[CommandInfo("Output file")]
		[CommandArg(2, "output", false)]
		public string OutputFile;

		[CommandInfo("Parameter files")]
		[CommandArgList("paramfiles", false)]
		public IList<string> ParamFiles;

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

	public class Program
	{

		[STAThread]
		static void Main()
		{
			// Retrieve command line options:
			CommandLineParser parser = new CommandLineParser();
			Options appOptions = new Options();

			//parser.Parse(appOptions);
            //
            //if (parser.IsHelpRequested) {
            //    parser.GenerateHelp(appOptions.GetType());
            //    Environment.Exit(0);
            //}
            //
            //if (appOptions.InputFile != String.Empty)
            //{
            //    Console.WriteLine("Input file given: " + appOptions.InputFile);
            //}





			Application.Run(new TestForm());
		}

	}
}
