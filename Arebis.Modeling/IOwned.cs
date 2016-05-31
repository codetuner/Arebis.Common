using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Modeling
{
    public interface IOwned<T>
    {
        T Owner { get; set; }
    }
}
