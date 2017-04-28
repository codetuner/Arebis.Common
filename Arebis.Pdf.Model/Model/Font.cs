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
    /// Definition of a font.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Font : ModelItem, IDocumentItem
    {
        /// <summary>
        /// Reference to a (predefined) font.
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        [DefaultValue(null)]
        public string FontRef { get; set; }
    }
}
