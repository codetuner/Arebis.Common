using System;
using System.Collections;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{

	[AutoMock(true)]
	public class FooBar : ContextBoundObject
	{

		public object SomeMethodWithInsAndOuts(int firstin, ref int secondref, out int thirdout, int fourthin)
		{
			secondref += firstin;
			thirdout = secondref + fourthin;
			return "<Real implementation run>";
		}

		public bool IsTrue
		{
			get { return true; }
		}

		public bool IsItTrue(bool answer)
		{
			return answer;
		}

		public void SomeVoid()
		{
		}
	}
}
