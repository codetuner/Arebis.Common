using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace Arebis
{
    /// <summary>
    /// Delegate for methods assisting the ObjectFactory in factoring objects.
    /// </summary>
    /// <returns></returns>
    public delegate object FactoryMethod(Type requestedType, object[] constructorArgs);

    /// <summary>
    /// A generic Factory pattern implementation.
    /// </summary>
    public class ObjectFactory
	{
        private static ObjectFactory instance;

		private ReaderWriterLock rwlock = new ReaderWriterLock();

        private Dictionary<Type, FactoryMethod> factoryMethods = new Dictionary<Type,FactoryMethod>();
        private Dictionary<Type, Type> factoryTypes = new Dictionary<Type,Type>();
        private Dictionary<Type, Object> factoryInstances = new Dictionary<Type,object>();

        /// <summary>
        /// Default ObjectFactory instance.
        /// </summary>
        public static ObjectFactory Instance
        {
            get
            {
                if (instance == null) instance = new ObjectFactory();
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Constructs an object of type T.
        /// </summary>
        /// <typeparam name="T">Type of object to construct.</typeparam>
        /// <param name="constructorArgs">Constructor arguments.</param>
        public T Construct<T>(params object[] constructorArgs)
        {
			rwlock.AcquireReaderLock(100);
			try
			{
				FactoryMethod factoryMethod;
				if (this.factoryMethods.TryGetValue(typeof(T), out factoryMethod))
				{
					return (T)(object)factoryMethod(typeof(T), constructorArgs);
				}
				else
				{
					return (T)(object)this.FactorDefault(typeof(T), constructorArgs);
				}
			}
			finally 
			{
				rwlock.ReleaseReaderLock();
			}
        }

        /// <summary>
        /// Registers a factoryMethod as the method to be called to
        /// construct an object in reply to a request for typeRequested.
        /// </summary>
        /// <param name="typeRequested">The type requested (interface or base class).</param>
        /// <param name="factoryMethod">The factory method.</param>
        public void RegisterFactoryMethod(Type typeRequested, FactoryMethod factoryMethod)
        {
			#region Check preconditions
			if (typeRequested == null)
			{
				throw new ArgumentNullException("typeRequested");
			}

			if (factoryMethod == null)
			{
				throw new ArgumentNullException("factoryMethod");
			}
			#endregion Check preconditions

			rwlock.AcquireWriterLock(100);
			try
			{
	            this.factoryMethods[typeRequested] = factoryMethod;
			}
			finally
			{
				rwlock.ReleaseWriterLock();
			}
		}

        /// <summary>
        /// Registers a typeFactored as the type to be constructed
        /// when requested for typeRequested.
        /// </summary>
        /// <param name="typeRequested">The type requested (interface or base class).</param>
        /// <param name="typeFactored">The type to construct on request.</param>
        public void RegisterFactoryClass(Type typeRequested, Type typeFactored)
		{
			#region Check preconditions
			if (typeRequested == null)
			{
				throw new ArgumentNullException("typeRequested");
			}

			if (typeFactored == null)
			{
				throw new ArgumentNullException("typeFactored");
			}

			if ((!typeFactored.IsClass) || (typeFactored.IsAbstract) || (typeFactored.IsGenericTypeDefinition))
			{
				throw new ArgumentException(String.Format("Cannot register factory type '{1}' as it is not instantiatable.", typeRequested, typeFactored), "typeFactored");
			}

			if (typeRequested.IsAssignableFrom(typeFactored) == false)
			{
				throw new ArgumentException(String.Format("Cannot register factory class '{1}', it is incompatible to requested type '{0}'.", typeRequested, typeFactored), "typeFactored");
			}
			#endregion Check preconditions

			rwlock.AcquireWriterLock(100);
			try
			{
				this.factoryTypes[typeRequested] = typeFactored;
				this.factoryMethods[typeRequested] = this.FactorFromType;
			}
			finally
			{
				rwlock.ReleaseWriterLock();
			}
        }

        /// <summary>
        /// Registers an instance as the object to return whenever
        /// a request is made to construct a typeRequested instance.
        /// </summary>
        /// <param name="typeRequested">The type requested (interface or base class).</param>
        /// <param name="instance">The instance to be returned on request.</param>
        public void RegisterFacoryInstance(Type typeRequested, object instance)
        {
			#region Check preconditions
			if (typeRequested == null)
			{
				throw new ArgumentNullException("typeRequested");
			}

			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			if (typeRequested.IsAssignableFrom(instance.GetType()) == false)
			{
				throw new ArgumentException(String.Format("Cannot register given instance, it is incompatible to requested type '{0}'.", typeRequested), "instance");
			}
			#endregion Check preconditions

			rwlock.AcquireWriterLock(100);
			try
			{
			    this.factoryInstances[typeRequested] = instance;
				this.factoryMethods[typeRequested] = this.FactorFromInstance;
			}
			finally
			{
				rwlock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Default implementation for factoring.
		/// </summary>
		protected object FactorDefault(Type requestedType, object[] constructorArgs)
		{
			return requestedType.InvokeMember(null, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, constructorArgs);
		}

        /// <summary>
        /// Default implementation for factoring from a class.
        /// </summary>
        protected object FactorFromType(Type requestedType, object[] constructorArgs)
        {
            Type typeToFactor = this.factoryTypes[requestedType];
            return typeToFactor.InvokeMember(null, BindingFlags.CreateInstance, null, null, constructorArgs);
        }

        /// <summary>
        /// Default implementation for factoring from instance.
        /// </summary>
        protected object FactorFromInstance(Type requestedType, object[] constructorArgs)
        {
            return this.factoryInstances[requestedType];
        }
    }
}
