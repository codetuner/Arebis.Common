using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity.ChangeDataCapture
{
    public enum CdcOperation
    {
        Unknown = 0,
        Delete = 1,
        Insert = 2,
        UpdateBefore = 3,
        UpdateAfter = 4,
    }
}
