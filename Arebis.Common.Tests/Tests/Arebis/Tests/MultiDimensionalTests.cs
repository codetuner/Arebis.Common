using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arebis.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arebis.Tests
{
    [TestClass]
    public class MultiDimensionalTests
    {
        [TestMethod]
        public void MultiDimensionalRedimensionTest()
        {
            Arebis.Types.MultiDimensional<int> md;

            md = new MultiDimensional<int>();
            Assert.AreEqual(md.Dimensions.Count, 0);

            var n = 0;
            md = new MultiDimensional<int>(2, 3, 4);
            for (int z = 0; z < md.Dimensions[2]; z++)
                for (int y = 0; y < md.Dimensions[1]; y++)
                    for (int x = 0; x < md.Dimensions[0]; x++)
                    md[x, y, z] = n++;

            // Fills the md with following data:
            // md[0,0,0] = 0
            // md[1,0,0] = 1
            // md[0,1,0] = 2
            // md[1,1,0] = 3
            // md[0,2,0] = 4
            // md[1,2,0] = 5
            // md[0,0,1] = 6
            // md[1,0,1] = 7
            // md[0,1,1] = 8
            // md[1,1,1] = 9
            // md[0,2,1] = 10
            // md[1,2,1] = 11
            // md[0,0,2] = 12
            // md[1,0,2] = 13
            // md[0,1,2] = 14
            // md[1,1,2] = 15
            // md[0,2,2] = 16
            // md[1,2,2] = 17
            // md[0,0,3] = 18
            // md[1,0,3] = 19
            // md[0,1,3] = 20
            // md[1,1,3] = 21
            // md[0,2,3] = 22
            // md[1,2,3] = 23

            Assert.AreEqual(md.Dimensions.Count, 3);
            Assert.AreEqual(md.Dimensions[0], 2);
            Assert.AreEqual(md.Dimensions[1], 3);
            Assert.AreEqual(md.Dimensions[2], 4);
            Assert.AreEqual(md[0, 0, 0], 0);
            Assert.AreEqual(md[1, 0, 0], 1);
            Assert.AreEqual(md[1, 2, 0], 5);
            Assert.AreEqual(md[1, 2, 3], 23);
            Assert.AreEqual(md[1, 1, 1], 9);
            Assert.AreEqual(md[1, 2, 2], 17);

            md.Resize(2, 3, 6);

            Assert.AreEqual(md.Dimensions.Count, 3);
            Assert.AreEqual(md.Dimensions[0], 2);
            Assert.AreEqual(md.Dimensions[1], 3);
            Assert.AreEqual(md.Dimensions[2], 6);
            Assert.AreEqual(md[0, 0, 0], 0);
            Assert.AreEqual(md[1, 0, 0], 1);
            Assert.AreEqual(md[1, 2, 0], 5);
            Assert.AreEqual(md[1, 2, 3], 23);
            Assert.AreEqual(md[1, 1, 1], 9);
            Assert.AreEqual(md[1, 2, 2], 17);
            Assert.AreEqual(md[1, 1, 4], 0);
            Assert.AreEqual(md[1, 2, 5], 0);

            md.Resize(2, 3, 3);

            Assert.AreEqual(md.Dimensions.Count, 3);
            Assert.AreEqual(md.Dimensions[0], 2);
            Assert.AreEqual(md.Dimensions[1], 3);
            Assert.AreEqual(md.Dimensions[2], 3);
            Assert.AreEqual(md[0, 0, 0], 0);
            Assert.AreEqual(md[1, 0, 0], 1);
            Assert.AreEqual(md[1, 2, 0], 5);
            Assert.AreEqual(md[1, 1, 1], 9);
            Assert.AreEqual(md[1, 2, 2], 17);

            md.Resize(2, 3, 4);

            Assert.AreEqual(md[1, 2, 3], 0);

            md.Resize(4, 4, 4);

            Assert.AreEqual(md.Dimensions.Count, 3);
            Assert.AreEqual(md.Dimensions[0], 4);
            Assert.AreEqual(md.Dimensions[1], 4);
            Assert.AreEqual(md.Dimensions[2], 4);
            Assert.AreEqual(md[0, 0, 0], 0);
            Assert.AreEqual(md[1, 0, 0], 1);
            Assert.AreEqual(md[1, 2, 0], 5);
            Assert.AreEqual(md[1, 2, 3], 0);
            Assert.AreEqual(md[1, 1, 1], 9);
            Assert.AreEqual(md[1, 2, 2], 17);
            Assert.AreEqual(md[2, 0, 0], 0);
            Assert.AreEqual(md[0, 3, 0], 0);
            Assert.AreEqual(md[2, 3, 0], 0);
            Assert.AreEqual(md[2, 3, 3], 0);


            n = 0;
            var mdt = new MultiDimensional<Tuple<int, int, int, int>>(2, 3, 4);
            for (int z = 0; z < mdt.Dimensions[2]; z++)
                for (int y = 0; y < mdt.Dimensions[1]; y++)
                    for (int x = 0; x < mdt.Dimensions[0]; x++)
                    {
                        //Console.WriteLine("// md[{0},{1},{2}] = {3}", x, y, z, n);
                        mdt[x, y, z] = new Tuple<int, int, int, int>(x, y, z, n++);
                    }

            Assert.IsTrue(true);
        }
    }
}
