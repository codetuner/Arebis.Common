using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Arebis.Extensions.Tests.Arebis.CodeModel.Sample
{
	class SampleClass : SampleInterface
	{
		public void InterfaceMethodA()
		{
			throw new NotImplementedException();
		}

		public void InterfaceMethodA(int i)
		{
			throw new NotImplementedException();
		}

		void SampleInterface.InterfaceMethodB()
		{
			throw new NotImplementedException();
		}

		void SampleInterface.InterfaceMethodB(int i)
		{
			throw new NotImplementedException();
		}

		public void InterfaceMethodC()
		{
			throw new NotImplementedException();
		}

		public void InterfaceMethodC(int i)
		{
			throw new NotImplementedException();
		}

		static void InterfaceMethodD()
		{
			throw new NotImplementedException();
		}

		static void InterfaceMethodD(int i)
		{
			throw new NotImplementedException();
		}
	}
}
