using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// A generic Model class.
    /// By using Model&lt;T&gt;, your views will talk in terms of "Item" and will later easily be replaced by richer models.
    /// </summary>
    /// <typeparam name="TItem">The main model item type.</typeparam>
    public class Model<TItem>
    {
        public Model()
        { }

        public Model(TItem item)
        {
            this.Item = item;
        }

        public TItem Item { get; set; }
    }
}
