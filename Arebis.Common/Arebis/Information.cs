using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Arebis
{
    /// <summary>
    /// An Exception subclass for informational messages.
    /// </summary>
    [Serializable]
    public class Information : Exception
    {
        /// <summary>
        /// Constructs a default Information.
        /// </summary>
        public Information()
            : this("An informational exception occurred.")
        { }

        /// <summary>
        /// Constructs a Information given a custom message.
        /// </summary>
        public Information(string message)
            : this(message, null)
        { }

        /// <summary>
        /// Constructs a Information for an inner exception.
        /// </summary>
        public Information(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected Information(SerializationInfo info, StreamingContext context)
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
