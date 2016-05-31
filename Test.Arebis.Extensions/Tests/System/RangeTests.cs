using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arebis.Extensions.Tests.System
{
	[TestClass()]
	public class RangeTests
	{
		[TestMethod()]
		public void RangeInt01Test()
		{
			Range<int> intr = new Range<int>(12, 20);
			Assert.IsFalse(intr.Includes(8));
			Assert.IsTrue(intr.Includes(12));
			Assert.IsTrue(intr.Includes(18));
			Assert.IsTrue(intr.Includes(20));
			Assert.IsFalse(intr.Includes(21));
		}

		[TestMethod()]
		public void RangeString01Test()
		{
			Range<string> intr = new Range<string>("ddd", "kkk");
			Assert.IsFalse(intr.Includes("dda"));
			Assert.IsTrue(intr.Includes("ddd"));
			Assert.IsTrue(intr.Includes("fff"));
			Assert.IsTrue(intr.Includes("kkk"));
			Assert.IsFalse(intr.Includes("zzz"));
		}

		[TestMethod()]
		public void RangeDateTime01Test()
		{
			DateTime d1 = new DateTime(2007, 06, 10);
			DateTime d2 = new DateTime(2008, 06, 10);
			Range<DateTime> intr = new Range<DateTime>(d1, d2);
			Assert.IsFalse(intr.Includes(new DateTime(2007, 06, 09)));
			Assert.IsTrue(intr.Includes(new DateTime(2007, 06, 10)));
			Assert.IsTrue(intr.Includes(new DateTime(2007, 12, 31)));
			Assert.IsTrue(intr.Includes(new DateTime(2008, 06, 10)));
			Assert.IsFalse(intr.Includes(new DateTime(2008, 06, 11)));
		}

		[TestMethod()]
		public void RangeIncludes01Test()
		{
			Range<int> range = new Range<int>(12, 20);
			Range<int> ra = new Range<int>(4, 8);
			Range<int> rb = new Range<int>(10, 16);
			Range<int> rc = new Range<int>(10, 24);
			Range<int> rd = new Range<int>(12, 14);
			Range<int> re = new Range<int>(14, 18);
			Range<int> rf = new Range<int>(18, 20);
			Range<int> rg = new Range<int>(12, 20);
			Range<int> rh = new Range<int>(12, 24);
			Range<int> ri = new Range<int>(20, 24);
			Range<int> rj = new Range<int>(22, 24);
			Assert.IsFalse(range.Includes(ra));
			Assert.IsFalse(range.Includes(rb));
			Assert.IsFalse(range.Includes(rc));
			Assert.IsTrue(range.Includes(rd));
			Assert.IsTrue(range.Includes(re));
			Assert.IsTrue(range.Includes(rf));
			Assert.IsTrue(range.Includes(rg));
			Assert.IsFalse(range.Includes(rh));
			Assert.IsFalse(range.Includes(ri));
			Assert.IsFalse(range.Includes(rj));
		}

		[TestMethod()]
		public void RangeOverlaps01Test()
		{
			Range<int> range = new Range<int>(12, 20);
			Range<int> ra = new Range<int>(4, 8);
			Range<int> rb = new Range<int>(10, 16);
			Range<int> rc = new Range<int>(10, 24);
			Range<int> rd = new Range<int>(12, 14);
			Range<int> re = new Range<int>(14, 18);
			Range<int> rf = new Range<int>(18, 20);
			Range<int> rg = new Range<int>(12, 20);
			Range<int> rh = new Range<int>(12, 24);
			Range<int> ri = new Range<int>(20, 24);
			Range<int> rj = new Range<int>(22, 24);
			Assert.IsFalse(range.Overlaps(ra));
			Assert.IsTrue(range.Overlaps(rb));
			Assert.IsTrue(range.Overlaps(rc));
			Assert.IsTrue(range.Overlaps(rd));
			Assert.IsTrue(range.Overlaps(re));
			Assert.IsTrue(range.Overlaps(rf));
			Assert.IsTrue(range.Overlaps(rg));
			Assert.IsTrue(range.Overlaps(rh));
			Assert.IsTrue(range.Overlaps(ri));
			Assert.IsFalse(range.Overlaps(rj));
		}

		[TestMethod()]
		public void RangeUnion01Test()
		{
			Range<int> r1;
			Range<int> r2;

			r1 = new Range<int>(10, 20);
			r2 = new Range<int>(10, 20);
			Assert.AreEqual(new Range<int>(10, 20), r1.Union(r2, false));

			r1 = new Range<int>(10, 20);
			r2 = new Range<int>(20, 30);
			Assert.AreEqual(new Range<int>(10, 30), r1.Union(r2, false));

			r1 = new Range<int>(10, 20);
			r2 = new Range<int>(12, 24);
			Assert.AreEqual(new Range<int>(10, 24), r1.Union(r2, false));

			r1 = new Range<int>(10, 20);
			r2 = new Range<int>(8, 12);
			Assert.AreEqual(new Range<int>(8, 20), r1.Union(r2, false));

			r1 = new Range<int>(10, 20);
			r2 = new Range<int>(12, 18);
			Assert.AreEqual(new Range<int>(10, 20), r1.Union(r2, false));

			r1 = new Range<int>(10, 20);
			r2 = new Range<int>(8, 22);
			Assert.AreEqual(new Range<int>(8, 22), r1.Union(r2, false));
		}

		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void RangeUnion02Test()
		{
			Range<int> r1;
			Range<int> r2;

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(12, 16);

			Range<int> union = r1.Union(r2, false);
		}

		[TestMethod()]
		public void RangeIntersection01Test()
		{
			Range<int> r1;
			Range<int> r2;

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(12, 16);
			Assert.AreEqual(null, r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(10, 15);
			Assert.AreEqual(new Range<int>(10, 10), r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(5, 15);
			Assert.AreEqual(new Range<int>(5, 10), r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(0, 10);
			Assert.AreEqual(new Range<int>(5, 10), r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(8, 10);
			Assert.AreEqual(new Range<int>(8, 10), r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(5, 8);
			Assert.AreEqual(new Range<int>(5, 8), r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(5, 10);
			Assert.AreEqual(new Range<int>(5, 10), r1.Intersection(r2));

			r1 = new Range<int>(5, 10);
			r2 = new Range<int>(7, 9);
			Assert.AreEqual(new Range<int>(7, 9), r1.Intersection(r2));

			r1 = new Range<int>(7, 9);
			r2 = new Range<int>(5, 10);
			Assert.AreEqual(new Range<int>(7, 9), r1.Intersection(r2));
		}
	}
}
