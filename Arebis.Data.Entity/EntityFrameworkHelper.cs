using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Arebis.Data.Entity
{
	internal static class EntityFrameworkHelper
	{
		internal static void CollectRelationalMembers(Expression exp, IList<ExtendedPropertyInfo> members)
		{
			if (exp.NodeType == ExpressionType.Lambda)
			{
				// At root, handle body:
				CollectRelationalMembers(((LambdaExpression)exp).Body, members);
			}
			else if (exp.NodeType == ExpressionType.MemberAccess)
			{
				// Add expression property to collected members and handle remainder of expression:
				MemberExpression mexp = (MemberExpression)exp;
				CollectRelationalMembers(mexp.Expression, members);
				members.Add(new ExtendedPropertyInfo((PropertyInfo)mexp.Member));
			}
			else if (exp.NodeType == ExpressionType.Call)
			{
				MethodCallExpression cexp = (MethodCallExpression)exp;

				// Only static (extension) methods with 1 argument are supported:
				if (cexp.Method.IsStatic == false || cexp.Arguments.Count != 1)
					throw new InvalidOperationException("Invalid type of expression.");

				// Recursively handle arguments:
				foreach (var arg in cexp.Arguments)
					CollectRelationalMembers(arg, members);

				// Handle special marker method 'WithoutUpdate':
				if (cexp.Method.Name == "WithoutUpdate")
					members.Last().NoUpdate = true;

				// Handle special marker method 'ReferenceOnly':
				if (cexp.Method.Name == "ReferenceOnly")
					members.Last().ReferenceOnly = true;

			}
			else if (exp.NodeType == ExpressionType.Parameter)
			{
				// Reached the toplevel:
				return;
			}
			else
			{
				throw new InvalidOperationException("Invalid type of expression.");
			}
		}
	}

	internal class ExtendedPropertyInfo : IEquatable<ExtendedPropertyInfo>
	{
		public ExtendedPropertyInfo(PropertyInfo propertyInfo)
		{
			this.PropertyInfo = propertyInfo;
		}

		public PropertyInfo PropertyInfo { get; private set; }

		public bool NoUpdate { get; set; }

		public bool ReferenceOnly { get; set; }

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ExtendedPropertyInfo);
		}

		public bool Equals(ExtendedPropertyInfo other)
		{
			if (Object.ReferenceEquals(other, null))
				return false;
			else
				return (Object.Equals(this.PropertyInfo, other.PropertyInfo));
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode() ^ this.PropertyInfo.GetHashCode();
		}
	}
}
