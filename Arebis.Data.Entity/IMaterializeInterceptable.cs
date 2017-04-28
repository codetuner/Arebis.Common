using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    public interface IMaterializeInterceptable
    {
        void OnMaterialized(DbContext context);
    }
}
