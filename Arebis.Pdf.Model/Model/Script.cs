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
    /// A script element.
    /// </summary>
    [DataContract]
    [ContentProperty("Content")]
    [Serializable]
    public class Script : ModelItem, IPageItem
    {
        /// <summary>
        /// Reference to a script object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string ScriptRef { get; set; }

        /// <summary>
        /// The PDF script.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Content { get; set; }
    }
}
