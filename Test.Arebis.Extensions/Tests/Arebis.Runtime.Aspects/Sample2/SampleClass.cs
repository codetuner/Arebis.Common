using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arebis.Runtime.Aspects;
using System.Reflection;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample2
{
	class SampleClass : Advisable, SampleInterface
	{
		public SampleClass(int id)
		{
			this.Id = id;
		}

		public int Id { get; set; }

		[TestableAdvice("Class.A1")]
		public void InterfaceMethodA()
		{
			Console.WriteLine("Called method: \"{0}\"", MethodBase.GetCurrentMethod().Name);
		}

		[TestableAdvice("Class.A2")]
		public void InterfaceMethodA(int i)
		{
			Console.WriteLine("Called method: \"{0}\"", MethodBase.GetCurrentMethod().Name);
		}

		[TestableAdvice("Class.B1")]
		void SampleInterface.InterfaceMethodB()
		{
			Console.WriteLine("Called method: \"{0}\"", MethodBase.GetCurrentMethod().Name);
		}

		[TestableAdvice("Class.B2")]
		void SampleInterface.InterfaceMethodB(int i)
		{
			Console.WriteLine("Called method: \"{0}\"", MethodBase.GetCurrentMethod().Name);
		}

		[TestableAdvice("Class.C1")]
		public void InterfaceMethodC()
		{
			Console.WriteLine("Called method: \"{0}\"", MethodBase.GetCurrentMethod().Name);
		}

		[TestableAdvice("Class.C2")]
		public void InterfaceMethodC(int i)
		{
			Console.WriteLine("Called method: \"{0}\"", MethodBase.GetCurrentMethod().Name);
		}
	}
}
