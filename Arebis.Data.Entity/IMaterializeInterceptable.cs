using System.Data.Entity;

namespace Arebis.Data.Entity
{
    public interface IMaterializeInterceptable
    {
        void OnMaterialized(DbContext context);
    }
}
