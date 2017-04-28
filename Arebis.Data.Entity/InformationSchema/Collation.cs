using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [ComplexType]
    public class Collation
    {
        [Column("COLLATION_CATALOG")]
        public virtual string Catalog { get; set; }

        [Column("COLLATION_SCHEMA")]
        public virtual string Schema { get; set; }

        [Column("COLLATION_NAME")]
        public virtual string Name { get; set; }
    }
}
