using System;
namespace Arebis.Data.Model
{
    public interface IModelBuilder
    {
        DatabaseModel Build(System.Data.Common.DbConnection connection);
    }
}
