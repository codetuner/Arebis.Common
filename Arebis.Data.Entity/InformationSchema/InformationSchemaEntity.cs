using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.InformationSchema
{
    public abstract class InformationSchemaEntity
    {
        [NotMapped]
        public InformationSchemaContext Context { get; set; }
    }
}
