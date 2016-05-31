using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public class ModelTable : DatabaseOwnedElement
    {
        public ModelTable()
        {
            this.Columns = new List<ModelColumn>();
            this.Indexes = new List<ModelIndex>();
        }

        public virtual IList<ModelColumn> Columns { get; set; }

        public virtual IList<ModelIndex> Indexes { get; set; }

        public virtual bool IsView {
            get { return false; }
        }
    }
}
