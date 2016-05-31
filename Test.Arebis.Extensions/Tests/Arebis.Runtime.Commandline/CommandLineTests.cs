using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Runtime.Commandline;
using Arebis.Extensions.Tests.Arebis.Runtime.Commandline.Sample;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Commandline
{
	[TestClass()]
	public class CommandLineTests
	{
		[TestMethod()]
		public void CommandLine01Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
				@"app.exe first"
			);

			Assert.AreEqual("first", options.InputFile);
			Assert.AreEqual(null, options.OutputFile);
			Assert.AreEqual(0, options.ParamFiles.Count);
			Assert.AreEqual(TextFormats.Unicode, options.Format);
			Assert.AreEqual(false, options.OverwriteReadonly);
			Assert.IsFalse(options.HelpRequested);
		}

		[TestMethod()]
		public void CommandLine02Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
                @"app.exe first second third fourth"
			);

			Assert.AreEqual("first", options.InputFile);
			Assert.AreEqual("second", options.OutputFile);
			Assert.AreEqual("third", options.ParamFiles[0]);
			Assert.AreEqual("fourth", options.ParamFiles[1]);
			Assert.AreEqual(TextFormats.Unicode, options.Format);
			Assert.AreEqual(false, options.OverwriteReadonly);
			Assert.IsFalse(options.HelpRequested);
		}

		[TestMethod()]
		public void CommandLine03Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
                @"app.exe first /f dos"
			);

			Assert.AreEqual("first", options.InputFile);
			Assert.AreEqual(null, options.OutputFile);
			Assert.AreEqual(0, options.ParamFiles.Count);
			Assert.AreEqual(TextFormats.Ascii, options.Format);
			Assert.AreEqual(false, options.OverwriteReadonly);
			Assert.IsFalse(options.HelpRequested);
		}

		[TestMethod()]
		public void CommandLine04Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
                @"app.exe first /f dos /o"
			);

			Assert.AreEqual("first", options.InputFile);
			Assert.AreEqual(null, options.OutputFile);
			Assert.AreEqual(0, options.ParamFiles.Count);
			Assert.AreEqual(TextFormats.Ascii, options.Format);
			Assert.AreEqual(true, options.OverwriteReadonly);
			Assert.IsFalse(options.HelpRequested);
		}

		[TestMethod()]
		public void CommandLine05Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
                @"app.exe /?"
			);

			Assert.AreEqual(null, options.InputFile);
			Assert.AreEqual(null, options.OutputFile);
			Assert.AreEqual(0, options.ParamFiles.Count);
			Assert.AreEqual(TextFormats.Unicode, options.Format);
			Assert.AreEqual(false, options.OverwriteReadonly);
			Assert.IsTrue(options.HelpRequested);
		}

		[TestMethod()]
		public void CommandLine06Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
                @"app.exe first /f dos /o /?"
			);

			Assert.AreEqual("first", options.InputFile);
			Assert.AreEqual(null, options.OutputFile);
			Assert.AreEqual(0, options.ParamFiles.Count);
			Assert.AreEqual(TextFormats.Ascii, options.Format);
			Assert.AreEqual(true, options.OverwriteReadonly);
			Assert.IsTrue(options.HelpRequested);
		}

		[TestMethod()]
		[ExpectedException(typeof(TooManyArgumentsException))]
		public void CommandLine07Test()
		{
			TestOptions options = (TestOptions)new CommandLineParser().Parse(
				new TestOptions(),
                @"app.exe first second third"
			);
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOption))]
		public void CommandLine08Test()
		{
			TestOptions options = (TestOptions)new CommandLineParser().Parse(
				new TestOptions(),
                @"app.exe first /x"
			);
		}

		[TestMethod()]
		[ExpectedException(typeof(InvalidOptionValue))]
		public void CommandLine09Test()
		{
			TestOptions options = (TestOptions)new CommandLineParser().Parse(
				new TestOptions(),
                @"app.exe first /f html"
			);
		}

		[TestMethod()]
		public void CommandLine10Test()
		{
			TestOptionsExt options = (TestOptionsExt)new CommandLineParser().Parse(
				new TestOptionsExt(),
                "app.exe first /logfile \"MyLog 123.log\""
			);

			Assert.AreEqual("first", options.InputFile);
			Assert.AreEqual(null, options.OutputFile);
			Assert.AreEqual("MyLog 123.log", options.LogFile);
		}
	}
}
