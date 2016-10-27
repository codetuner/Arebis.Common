using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Entity
{
    public interface ISaveInterceptable
    {
        void OnSaving(DbContext context, DbEntityEntry entry);
    }
}
