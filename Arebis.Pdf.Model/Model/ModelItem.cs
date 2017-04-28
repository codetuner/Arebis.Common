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
    /// Base class for all PDF model types.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Font))]
    [KnownType(typeof(GraphicsOptions))]
    [KnownType(typeof(ImageObject))]
    [KnownType(typeof(Page))]
    [KnownType(typeof(PositionalItem))]
    [KnownType(typeof(Script))]
    [KnownType(typeof(ScriptObject))]
    [KnownType(typeof(TextOptions))]
    [Serializable]
    public abstract class ModelItem
    {
        [NonSerialized]
        private string[] classValues = null;

        /// <summary>
        /// An Id given to the element to facilitate identification by code.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Id { get; set; }

        /// <summary>
        /// Classes (space-separated class names) of the element to facilitate classification by code.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(null)]
        public string Class { get; set; }

        /// <summary>
        /// Read-only accessor to get all class names in array form.
        /// </summary>
        public string[] ClassValues
        {
            get
            { 
                if (this.classValues == null)
                {
                    if (String.IsNullOrWhiteSpace(this.Class))
                    {
                        this.classValues = new string[0];
                    }
                    else
                    {
                        this.classValues = this.Class.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }

                return this.classValues;
            }
        }

        /// <summary>
        /// Whether the element should be hidden.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        [DefaultValue(false)]
        public bool Hidden { get; set; }
    }
}
