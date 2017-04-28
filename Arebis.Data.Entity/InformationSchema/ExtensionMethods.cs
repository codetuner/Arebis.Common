using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    public static class ExtensionMethods
    {
        public static IOrderedQueryable<Table> OrderedByName(this IQueryable<Table> set)
        {
            return set
                .OrderBy(t => t.CatalogName)
                .ThenBy(t => t.SchemaName)
                .ThenBy(t => t.Name);
        }

        public static IOrderedQueryable<TableColumn> OrderedByName(this IQueryable<TableColumn> set)
        {
            return set
                .OrderBy(t => t.CatalogName)
                .ThenBy(t => t.SchemaName)
                .ThenBy(t => t.TableName)
                .ThenBy(t => t.Name);
        }

        public static IOrderedQueryable<TableColumn> OrderedByPosition(this IQueryable<TableColumn> set)
        {
            return set
                .OrderBy(t => t.CatalogName)
                .ThenBy(t => t.SchemaName)
                .ThenBy(t => t.TableName)
                .ThenBy(t => t.OrdinalPosition);
        }
    }
}

