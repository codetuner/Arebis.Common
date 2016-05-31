using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public class ModelIndex : TableOwnedElement
    {
        public ModelIndex()
        {
            this.Columns = new List<ModelColumn>();
        }

        public virtual IList<ModelColumn> Columns { get; set; }

        public virtual bool IsPrimary { get; set; }
        
        public virtual bool IsUnique { get; set; }
    }
}
