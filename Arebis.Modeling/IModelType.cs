using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Modeling
{
    public interface IModelType : IOwned<Package>
    {
        string Name { get; set; }

        bool IsAbstract { get; set; }
    }
}
