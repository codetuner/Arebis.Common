using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Arebis
{
    /// <summary>
    /// An Informational Exception subclass for warnings.
    /// </summary>
    [Serializable]
    public class Warning : Information
    {
        /// <summary>
        /// Constructs a default Warning.
        /// </summary>
        public Warning()
            : this("A warning informational exception occurred.")
        { }

        /// <summary>
        /// Constructs a Warning given a custom message.
        /// </summary>
        public Warning(string message)
            : this(message, null)
        { }

        /// <summary>
        /// Constructs a Warning for an inner exception.
        /// </summary>
        public Warning(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected Warning(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary>
        /// Serializes the current exception object.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
