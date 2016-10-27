using Arebis.Source;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    /// <summary>
    /// Trims spaces from all string fields.
    /// </summary>
    [CodeSource("https://romiller.com/2014/10/20/ef6-1workaround-trailing-blanks-issue-in-string-joins/")]
    public class StringTrimmerInterceptor : IDbCommandTreeInterceptor
    {
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    var newQuery = queryCommand.Query.Accept(new StringTrimmerQueryVisitor());
                    interceptionContext.Result = new DbQueryCommandTree(
                        queryCommand.MetadataWorkspace,
                        queryCommand.DataSpace,
                        newQuery);
                }
            }
        }

        private class StringTrimmerQueryVisitor : DefaultExpressionVisitor
        {
            private static readonly string[] _typesToTrim = { "char", "nchar", "varchar", "nvarchar", "long char", "long nchar", "long varchar", "long nvarchar" };

            public override DbExpression Visit(DbNewInstanceExpression expression)
            {
                var arguments = expression.Arguments.Select(a =>
                {
                    var propertyArg = a as DbPropertyExpression;

                    if (propertyArg != null && _typesToTrim.Contains(propertyArg.Property.TypeUsage.EdmType.Name))
                    {
                        return EdmFunctions.Trim(a);
                    }

                    return a;
                });

                return DbExpressionBuilder.New(expression.ResultType, arguments);
            }
        }
    }
}
