using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Arebis.Linq;

namespace Arebis.Common.Tests
{
    [TestClass]
    public class ExpressionHelperTests
    {
#if UPTONET40
#else
        [TestMethod]
        public void ParseIntoPartsTest()
        {
            var result = PerformParseIntoParts<MethodInfo, string>((MethodInfo a) => a.ReturnType.Assembly.CustomAttributes.First().Constructor.Module.ScopeName.ToString().Split(';').FirstOrDefault().ToLower());

            foreach (var item in result)
            {
                Console.WriteLine("- {0,-20}: {1}", item.NodeType, item);
            }

            Assert.AreEqual(1, result.Where(x => x.NodeType == ExpressionType.Lambda).Count());
            Assert.AreEqual(ExpressionType.Parameter, result.First().NodeType);
            Assert.AreEqual(ExpressionType.Lambda, result.Last().NodeType);
            Assert.AreEqual(6, result.Where(x => x.NodeType == ExpressionType.MemberAccess).Count());
            Assert.AreEqual("ScopeName", result.OfType<MemberExpression>().Last().Member.Name);
            Assert.AreEqual(5, result.Where(x => x.NodeType == ExpressionType.Call).Count());
            Assert.AreEqual("ToLower", result.OfType<MethodCallExpression>().Last().Method.Name);

        }
#endif

        private IList<Expression> PerformParseIntoParts<T, U>(Expression<Func<T, U>> expression)
        {
            return ExpressionHelper.ParseIntoParts(expression, true);
        }
    }
}
