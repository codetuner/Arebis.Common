using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Model
{
    /// <summary>
    /// An image object that can be referenced from in pages.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ImageObject : ModelItem, IDocumentItem
    {
        /// <summary>
        /// URL of the image.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Url { get; set; }

        /// <summary>
        /// Filename of the image.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string FileName { get; set; }
    }
}
