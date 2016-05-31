using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    [Serializable]
    public class ModelView : ModelTable
    {
        public override bool IsView
        {
            get { return true; }
        }

        /// <summary>
        /// The SQL query behind the view.
        /// </summary>
        public virtual string ViewExpression { get; set; }
    }
}
