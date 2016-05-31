using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Linq
{
    public static class ExpressionHelper
    {
        public static IList<Expression> ParseIntoParts(LambdaExpression expr, bool reduce = false)
        {
            if (expr == null) throw new ArgumentNullException("expr");

            var result = new List<Expression>();
            Expression x = expr;

            if (reduce)
            {
                while (x.CanReduce)
                    x = x.Reduce();
            }

            while (x != null)
            {
                result.Insert(0, x);

                if (x is UnaryExpression)
                {
                    x = ((UnaryExpression)x).Operand;
                }
                else if (x is LambdaExpression)
                {
                    x = ((LambdaExpression)x).Body;
                }
                else if (x is MemberExpression)
                {
                    x = ((MemberExpression)x).Expression;
                }
                else if (x is MethodCallExpression)
                {
                    var method = ((MethodCallExpression)x).Method;
                    if (method.IsStatic)
                    {
                        x = ((MethodCallExpression)x).Arguments.FirstOrDefault();
                    }
                    else
                    {
                        x = ((MethodCallExpression)x).Object;
                    }
                }
                else if (x is ParameterExpression)
                {
                    x = null;
                }
                else
                {
                    throw new NotSupportedException(String.Format("Expression part \"{0}\" not supported.", x));
                }
            }

            return result;
        }

        /// <summary>
        /// For an expression as "x => x.Customer.Address.Town" returns the string "Customer.Address.Town".
        /// </summary>
        public static string GetPropertyPath(this Expression path)
        {
            StringBuilder sb = new StringBuilder();

            Expression x = path;
            while (true)
            {
                if (x.NodeType == ExpressionType.Lambda)
                {
                    x = ((LambdaExpression)x).Body;
                }
                else if (x.NodeType == ExpressionType.MemberAccess)
                {
                    sb.Insert(0, ((MemberExpression)x).Member.Name);
                    sb.Insert(0, '.');
                    x = ((MemberExpression)x).Expression;
                }
                else if (x.NodeType == ExpressionType.Convert)
                {
                    x = ((UnaryExpression)x).Operand;
                }
                else if (x.NodeType == ExpressionType.ConvertChecked)
                {
                    x = ((UnaryExpression)x).Operand;
                }
                else if (x.NodeType == ExpressionType.Parameter)
                {
                    sb.Remove(0, 1); // Remove first '.'
                    break;
                }
                else
                {
                    throw new ArgumentException(String.Format("Unable to parse property path \"{0}\" due to expression of type {1}.", path, x.NodeType));
                }
            }

            return sb.ToString();
        }
    }
}
