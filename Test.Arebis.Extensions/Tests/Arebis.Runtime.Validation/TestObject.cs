using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Validation;
using System.Reflection;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Validation
{
	class TestObject
	{
		private int intOne = 0;

		/// <summary>
		/// Between -5 and +5
		/// </summary>
		[AssertIntegerBetween(-5, +5)]
		public int IntOne
		{
			get { return intOne; }
			set { intOne = value; }
		}

		private int intTwo = 1;

		/// <summary>
		/// Between 0 and 100, not-null
		/// </summary>
		[AssertNotNull]
		[AssertIntegerBetween(1, 100)]
		public int IntTwo
		{
			get { return intTwo; }
			set { intTwo = value; }
		}

		private int? nintOne = null;

		/// <summary>
		/// Between -5 and +5
		/// </summary>
		[AssertIntegerBetween(-5, +5)]
		public int? NintOne
		{
			get { return nintOne; }
			set { nintOne = value; }
		}

		private int? nintTwo = 1;

		/// <summary>
		/// Between 0 and 100, not-null
		/// </summary>
		[AssertNotNull]
		[AssertIntegerBetween(1, 100)]
		public int? NintTwo
		{
			get { return nintTwo; }
			set { nintTwo = value; }
		}

		private double dblOne = 0.0;

		/// <summary>
		/// Between -5.0 and +5.0
		/// </summary>
		[AssertDoubleBetween(-5.0, +5.0)]
		public double DblOne
		{
			get { return dblOne; }
			set { dblOne = value; }
		}

		private string strOne = null;

		/// <summary>
		/// Like "??/??/???? ??:??:??"
		/// </summary>
		[AssertStringLike("??/??/???? ??:??:??")]
		public string StrOne
		{
			get { return strOne; }
			set { strOne = value; }
		}

		private string strTwo = "#";

		/// <summary>
		/// Between 1 and 20 chars, not-null
		/// </summary>
		[AssertNotNull]
		[AssertStringSize(1, 20, true)]
		public string StrTwo
		{
			get { return strTwo; }
			set { strTwo = value; }
		}
	}
}
