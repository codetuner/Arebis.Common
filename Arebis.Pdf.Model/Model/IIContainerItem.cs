using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Model
{
    public interface IIContainerItem<TItem> : IModelItem
        where TItem : IModelItem
    {
        IList<TItem> Items { get; }
    }
}
