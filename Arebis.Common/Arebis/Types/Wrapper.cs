using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Types
{
    /// <summary>
    /// A basic value wrapper to allow reading &amp; writing values even on readonly properties (i.e. Tuples).
    /// Note: If you are looking for the old Wrapper implementation that supports ToString customization,
    /// see the ToStringWrapper class.
    /// </summary>
    [Serializable]
    public class Wrapper<T>
    {
        private T value;

        public Wrapper()
        { }

        public Wrapper(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// The wrapped value.
        /// </summary>
        public virtual T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public static implicit operator Wrapper<T>(T value)
        {
            return new Wrapper<T>(value);
        }

        public static implicit operator T(Wrapper<T> wrapper)
        {
            return wrapper.Value;
        }

        public override bool Equals(object obj)
        {
            var wrapper = obj as Wrapper<T>;
            if (wrapper != null)
                return this.Equals(wrapper);
            else
                return false;
        }

        public virtual bool Equals(Wrapper<T> obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            else
                return Object.Equals(this.value, obj.value);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode() ^ 1351321567;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}
