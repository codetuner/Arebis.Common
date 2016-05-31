using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Arebis.Reflection;

namespace Test.Arebis.Extensions.Tests.Arebis.Reflection
{
	[TestClass]
	public class MethodBodyReaderTests
	{
		[TestMethod]
		public void GetCalledMethodsTest()
		{
			MethodBase method = MethodBase.GetCurrentMethod();
			MethodBodyReader reader = new MethodBodyReader(method);

			IList<MethodBase> calledMethods = reader.GetCalledMethods(false, false);
			foreach (MethodBase calledMethod in calledMethods)
			{
				Console.WriteLine("     {0}.{1}", calledMethod.ReflectedType, calledMethod.Name);
			}

			Assert.IsTrue(calledMethods.Count > 3);
		}

		[TestMethod]
		public void InstructionsTest()
		{
			MethodBase method = MethodBase.GetCurrentMethod();
			ILanguageInfo language = new CsLanguageInfo();
			MethodBodyReader reader = new MethodBodyReader(method, language);

			language.RegisterNamespace("System");
			language.RegisterNamespace("System.Collections.Generic");
			language.RegisterNamespace("System.Reflection");
			language.RegisterNamespace("Arebis.Reflection");

			IList<ILInstruction> instructions = reader.Instructions;
			foreach (ILInstruction instruction in instructions)
			{
				Console.WriteLine("     {0}", instruction.GetCode());
			}

			Assert.IsTrue(instructions.Count > 12);
		}
	}
}
