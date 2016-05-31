using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.ImportExport
{
    public class DataReaderMap
    {
        public DataReaderMap(IDataReader reader)
        { 
            this.ColumnNames = new string[reader.FieldCount];
            this.ColumnIndexes = new Dictionary<string, int>();
            for (int i = 0; i < this.ColumnNames.Length; i++)
            {
                var name = reader.GetName(i);
                this.ColumnNames[i] = name;
                this.ColumnIndexes[name] = i;
            }
        }

        public string[] ColumnNames { get; set; }

        public Dictionary<string, int> ColumnIndexes { get; set; }
    }
}
