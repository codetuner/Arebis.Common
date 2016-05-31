using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public abstract class BaseModelElement
    {
        public BaseModelElement()
        {
            this.ExtraData = new Dictionary<object, object>();
        }

        public virtual IDictionary<object, object> ExtraData { get; set; }

        public virtual string Name { get; set; }
    }
}
