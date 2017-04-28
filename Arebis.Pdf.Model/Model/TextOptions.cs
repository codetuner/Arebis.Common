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
    /// A definition of text options.
    /// </summary>
    [DataContract]
    [Serializable]
    public class TextOptions : ModelItem, IDocumentItem
    {
        /// <summary>
        /// Reference to a text option to use as template.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string TemplateRef { get; set; }

        /// <summary>
        /// Ink color.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string InkColor { get; set; }

        /// <summary>
        /// Reference to a (predefined) font to use.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string FontRef { get; set; }

        /// <summary>
        /// Size of the font.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string FontSize { get; set; }

        /// <summary>
        /// Line dash pattern.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string LineDashPattern { get; set; }

        /// <summary>
        /// Line cap style.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string LineCapStyle { get; set; }

        /// <summary>
        /// Rendering mode (Fill, Stroke, FillAndStroke,...).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string RenderingMode { get; set; }

        /// <summary>
        /// Outline color.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string OutlineColor { get; set; }

        /// <summary>
        /// Outline width.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string OutlineWidth { get; set; }
    }
}
