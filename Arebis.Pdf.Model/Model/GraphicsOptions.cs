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
    /// A definition of graphics options.
    /// </summary>
    [DataContract]
    [Serializable]
    public class GraphicsOptions : ModelItem, IDocumentItem
    {
        /// <summary>
        /// Reference to a graphics options to use as template.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string TemplateRef { get; set; }

        /// <summary>
        /// Stroke color.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string StrokeColor { get; set; }

        /// <summary>
        /// Fill color.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string FillColor { get; set; }

        /// <summary>
        /// Stroke width.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string StrokeWidth { get; set; }

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
        /// Line join style.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string LineJoinStyle { get; set; }
    }
}
