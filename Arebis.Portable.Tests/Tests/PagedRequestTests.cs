using Arebis.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Portable.Tests
{
    [TestClass]
    public class PagedRequestTests
    {
        [TestMethod]
        public void SerializationTest()
        {
            var subject = new PagedRequest();
            subject.Echo = 123;
            subject.GlobalSearchValue = "foobar";

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, subject);

                stream.Seek(0L, SeekOrigin.Begin);

                var result = (PagedRequest)formatter.Deserialize(stream);

                Assert.AreEqual(subject.Echo, result.Echo);
                Assert.AreEqual(subject.GlobalSearchValue, result.GlobalSearchValue);
            }
        }
    }
}
