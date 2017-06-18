using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Types
{
    /// <summary>
    /// Marks a property as to auto initialize. Or mark a type for which all properties having that type as to auto initialize.
    /// Auto initialization requires you to call the AutoInitialize() extension method in the constructor of the object to
    /// auto initialize.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AutoInitializeAttribute : Attribute
    {
        /// <summary>
        /// Marks a property as to auto initialize. Or mark a type for which all properties having that type as to auto initialize.
        /// Auto initialization requires you to call the AutoInitialize() extension method in the constructor of the object to
        /// auto initialize.
        /// </summary>
        public AutoInitializeAttribute()
        {
            this.ThisConstructorArgIndex = -1;
        }

        /// <summary>
        /// Concrete type to instantiate.
        /// </summary>
        public Type ConcreteType { get; set; }

        /// <summary>
        /// Constructor arguments.
        /// </summary>
        public object[] ConstructorArgs { get; set; }

        /// <summary>
        /// Index of the constructor argument to pass 'this'.
        /// -1 to not pass this as a constructor argument.
        /// </summary>
        public int ThisConstructorArgIndex { get; set; }

        internal object CreateValue(Type ofType, object owner)
        {
            if (this.ThisConstructorArgIndex > -1)
            {
                if (this.ConstructorArgs == null)
                    this.ConstructorArgs = new Object[1];
                if (this.ConstructorArgs.Length <= this.ThisConstructorArgIndex)
                    throw new IndexOutOfRangeException("AutoInitialize attribute has a ThisConstructorArgIndex out of the range of given ConstructorArgs.");

                this.ConstructorArgs[this.ThisConstructorArgIndex] = owner;
            }

            var type = this.ConcreteType ?? ofType;
            return Activator.CreateInstance(type, this.ConstructorArgs);
        }
    }
}
