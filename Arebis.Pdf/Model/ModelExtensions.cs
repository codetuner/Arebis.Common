using Arebis.Pdf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Model
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Enumerates this and all contained model items.
        /// </summary>
        public static IEnumerable<IModelItem> All(this IModelItem item)
        {
            yield return item;

            {
                var subitems = item as IIContainerItem<IDocumentItem>;
                if (subitems != null)
                {
                    foreach (var subitem in subitems.Items)
                    {
                        foreach (var subsubitem in subitem.All())
                            yield return subsubitem;
                    }
                }
            }

            {
                var subitems = item as IIContainerItem<IPageItem>;
                if (subitems != null)
                {
                    foreach (var subitem in subitems.Items)
                    {
                        foreach (var subsubitem in subitem.All())
                            yield return subsubitem;
                    }
                }
            }
        }
    }
}
