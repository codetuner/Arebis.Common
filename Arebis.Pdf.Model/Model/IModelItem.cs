using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Model
{
    public interface IModelItem
    {
        string Id { get; set; }

        string Class { get; set; }

        string[] ClassValues { get; }

        bool Hidden { get; set; }
    }
}
