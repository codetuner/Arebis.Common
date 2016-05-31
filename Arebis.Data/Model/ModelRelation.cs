using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public class ModelRelation : DatabaseOwnedElement
    {
        public ModelRelation()
        {
            this.PrimaryColumns = new List<ModelColumn>();
            this.ForeignColumns = new List<ModelColumn>();
        }

        public virtual ModelTable PrimaryTable { get; set; }

        public virtual ModelTable ForeignTable { get; set; }

        public virtual IList<ModelColumn> PrimaryColumns { get; set; }
        
        public virtual IList<ModelColumn> ForeignColumns { get; set; }
    }
}
