using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    public abstract class ChangeDataCaptureEntity
    {
        [NotMapped]
        public ChangeDataCaptureContext Context { get; set; }
    }
}
