using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    [ComplexType]
    public class UserDefinedType
    {
        [Column("USER_DEFINED_TYPE_CATALOG")]
        public virtual string Catalog { get; set; }

        [Column("USER_DEFINED_TYPE_SCHEMA")]
        public virtual string Schema { get; set; }

        [Column("USER_DEFINED_TYPE_NAME")]
        public virtual string Name { get; set; }
    }
}
