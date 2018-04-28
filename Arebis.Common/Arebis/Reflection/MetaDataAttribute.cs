using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Reflection
{
    /// <summary>
    /// A MateDataAttribute allows adding metadata to code elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class MetaDataAttribute : Attribute
    {
        private object value;

        public MetaDataAttribute(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Name of the metadata property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the metadata property.
        /// </summary>
        public object Value
        {
            get
            {
                if (OverridingAppSettingsKey == null)
                {
                    return this.value;
                }
                else
                {
                    return ConfigurationManager.AppSettings[this.OverridingAppSettingsKey] ?? value;
                }
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// An optional AppSettings keyname that overrides the value of this metadata element.
        /// </summary>
        public string OverridingAppSettingsKey { get; set; }
    }
}
