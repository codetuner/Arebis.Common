using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Arebis.Pdf.Model
{
    /// <summary>
    /// A text element.
    /// </summary>
    [DataContract]
    [ContentProperty("Content")]
    [Serializable]
    public class Text : PositionalItem
    {
        /// <summary>
        /// Reference to a text options to use.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string TextOptionsRef { get; set; }

        /// <summary>
        /// Content (text) of this text element.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Content { get; set; }

        /// <summary>
        /// Content key of this text element for lookup in the ContentSource dictionary.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string ContentKey { get; set; }

        /// <summary>
        /// Rotation of the text (i.e. "45", "45°", "45deg", "0.7854rad", "12.5%" and "0.125frac" are all equivalent).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Rotation { get; set; }
    }
}
