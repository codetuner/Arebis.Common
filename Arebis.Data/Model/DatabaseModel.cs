using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public class DatabaseModel : BaseModelElement
    {
        public DatabaseModel()
        {
            this.Tables = new List<ModelTable>();
            this.Relations = new List<ModelRelation>();
        }

        public virtual IList<ModelTable> Tables { get; set; }

        public virtual IList<ModelRelation> Relations { get; set; }

        public virtual double? Version { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}]", this.Name);
        }
    }
}
