using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public abstract class TableOwnedElement : BaseModelElement
    {
        public virtual ModelTable Table { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}].[{1}].[{2}]", this.Table.Schema, this.Table.Name, this.Name);
        }
    }
}
