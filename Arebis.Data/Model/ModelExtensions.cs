using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Retrieves the table or view with the given schema name and table/view name.
        /// </summary>
        public static ModelTable GetTableOrView(this DatabaseModel model, string schemaName, string name)
        {
            return model.Tables.SingleOrDefault(t => schemaName.Equals(t.Schema, StringComparison.OrdinalIgnoreCase) && name.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Retrieves the table or view with the given name which may or may not contain the schema, and may or
        /// may not contain square brackets or double quotes.
        /// I.e. valid values include "dbo.Orders", "[dbo].Orders", "[dbo].[Orders]" or even just "Orders".
        /// If more than one table is found to match the given name, an exception is thrown.
        /// </summary>
        public static ModelTable GetTableOrView(this DatabaseModel model, string fullName)
        {
            var parts = fullName.Replace("[", "").Replace("]", "").Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return model.Tables.SingleOrDefault(t => parts[0].Equals(t.Name, StringComparison.OrdinalIgnoreCase));
            else if (parts.Length == 2)
                return model.Tables.SingleOrDefault(t => parts[0].Equals(t.Schema, StringComparison.OrdinalIgnoreCase) && parts[1].Equals(t.Name, StringComparison.OrdinalIgnoreCase));
            else
                throw new ArgumentException("Invalid or empty table name.", "name");
        }

        /// <summary>
        /// Retrieves the relation with the given schema name and relation name.
        /// </summary>
        public static ModelRelation GetRelation(this DatabaseModel model, string schemaName, string name)
        {
            return model.Relations.SingleOrDefault(t => schemaName.Equals(t.Schema, StringComparison.OrdinalIgnoreCase) && name.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Retrieves the relation with the given name which may or may not contain the schema, and may or
        /// may not contain square brackets or double quotes.
        /// If more than one relation is found to match the given name, an exception is thrown.
        /// </summary>
        public static ModelRelation GetRelation(this DatabaseModel model, string fullName)
        {
            var parts = fullName.Split(new char[] { '[', ']', '\"', '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return model.Relations.SingleOrDefault(t => parts[0].Equals(t.Name, StringComparison.OrdinalIgnoreCase));
            else if (parts.Length == 2)
                return model.Relations.SingleOrDefault(t => parts[0].Equals(t.Schema, StringComparison.OrdinalIgnoreCase) && parts[1].Equals(t.Name, StringComparison.OrdinalIgnoreCase));
            else
                throw new ArgumentException("Invalid or empty relation name.", "name");
        }

        
        public static ModelColumn GetColumn(this ModelTable table, string name)
        {
            return table.Columns.SingleOrDefault(c => name.Equals(c.Name, StringComparison.OrdinalIgnoreCase));
        }

        public static ModelColumn[] GetPrimaryKeyColumns(this ModelTable table)
        {
            return table.Columns.Where(c => c.IsPrimaryKey).ToArray();
        }

        public static ModelRelation[] GetFromRelations(this ModelTable table)
        {
            return table.Model.Relations.Where(r => r.PrimaryTable == table).ToArray();
        }

        public static ModelRelation[] GetToRelations(this ModelTable table)
        {
            return table.Model.Relations.Where(r => r.ForeignTable == table).ToArray();
        }
    }
}
