using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [ComplexType]
    public class Scope
    {
        [Column("SCOPE_CATALOG")]
        public virtual string Catalog { get; set; }

        [Column("SCOPE_SCHEMA")]
        public virtual string Schema { get; set; }

        [Column("SCOPE_NAME")]
        public virtual string Name { get; set; }
    }
}
