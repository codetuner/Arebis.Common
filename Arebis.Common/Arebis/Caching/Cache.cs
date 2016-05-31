using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Arebis.Caching
{
	/// <summary>
	/// Delegate definition for providers of cache values.
	/// </summary>
	/// <typeparam name="T">Type of value.</typeparam>
	/// <returns>A new value to be cached.</returns>
	public delegate T ValueProvider<T>();

	/// <summary>
	/// Delegate definition for cache validators.
	/// A cache validator tells whether the value currently hold by the cache
	/// is to be considered valid.
	/// </summary>
	/// <typeparam name="T">Type of value.</typeparam>
	/// <param name="cache">The cache instance to be checked for validity.</param>
	/// <returns>True if the current value in the cache is valid, false otherwise.</returns>
	public delegate bool CacheValidator<T>(Cache<T> cache);

	/// <summary>
	/// A cache is a holder for a value that will transparantly request for a
	/// more recent value whenever needed.
	/// </summary>
	/// <typeparam name="T">Type of value to be hold by the cache.</typeparam>
	public class Cache<T>
	{
		private ReaderWriterLock valueLock = new ReaderWriterLock();
		private int lockTimeout;
		private T value;
		private DateTime valueTime = DateTime.MinValue;
		private bool initialized = false;
		private bool invalidated = false;
		private TimeSpan maxAge;

		/// <summary>
		/// Delegate that provides a new value to the cache.
		/// </summary>
		public ValueProvider<T> ValueProvider = null;

		/// <summary>
		/// Delegate that will inform wither cache is still valid or not.
		/// </summary>
		public CacheValidator<T> CacheValidator = null;

		/// <summary>
		/// Constructs a new cache instance.
		/// </summary>
		/// <param name="valueProvider">A ValueProvider delegate that will provide updated values to the cache.</param>
		/// <param name="cacheValidator">A CacheValidator delegate that will tell if cached value is still actual.</param>
		/// <param name="lockTimeout"></param>
		public Cache(ValueProvider<T> valueProvider, CacheValidator<T> cacheValidator, int lockTimeout)
		{
			if (valueProvider == null) throw new ArgumentNullException("valueProvider");
			this.ValueProvider = valueProvider;
			this.CacheValidator = cacheValidator;
			this.lockTimeout = lockTimeout;
		}
		
		/// <summary>
		/// Constructs a new cache instance.
		/// </summary>
		/// <param name="valueProvider">A ValueProvider delegate that will provide updated values to the cache.</param>
		/// <param name="lockTimeout"></param>
		public Cache(ValueProvider<T> valueProvider, int lockTimeout)
		{
			if (valueProvider == null) throw new ArgumentNullException("valueProvider");
			this.ValueProvider = valueProvider;
			this.lockTimeout = lockTimeout;
		}

		/// <summary>
		/// Constructs a new cache instance.
		/// </summary>
		/// <param name="initialValue">The value to initialize the cache with.</param>
		/// <param name="cacheValidator">A CacheValidator delegate that will tell if cached value is still actual.</param>
		/// <param name="lockTimeout"></param>
		public Cache(T initialValue, CacheValidator<T> cacheValidator, int lockTimeout)
		{
			this.setValue(initialValue);
			this.lockTimeout = lockTimeout;
		}

		/// <summary>
		/// Constructs a new cache instance.
		/// </summary>
		/// <param name="valueProvider">A ValueProvider delegate that will provide updated values to the cache.</param>
		/// <param name="maxAge"></param>
		/// <param name="lockTimeout"></param>
		public Cache(ValueProvider<T> valueProvider, TimeSpan maxAge, int lockTimeout)
		{
			if (valueProvider == null) throw new ArgumentNullException("valueProvider");
			this.ValueProvider = valueProvider;
			this.maxAge = maxAge;
			this.CacheValidator = this.DefaultAgeValidator;
			this.lockTimeout = lockTimeout;
		}

		/// <summary>
		/// Constructs a new cache instance.
		/// </summary>
		/// <param name="initialValue">The value to initialize the cache with.</param>
		/// <param name="lockTimeout"></param>
		public Cache(T initialValue, int lockTimeout)
		{
			this.setValue(initialValue);
			this.lockTimeout = lockTimeout;
		}

		/// <summary>
		/// Default validator checking the age of the value.
		/// </summary>
		private bool DefaultAgeValidator(Cache<T> cache)
		{
			return (cache.ValueAge < this.maxAge);
		}

		/// <summary>
		/// Whether the cache contains a value that is currently valid.
		/// </summary>
		public bool IsValueValid
		{
			get 
			{
				return (
					(this.initialized == true) 
					&& (this.invalidated == false)
					&& (((this.CacheValidator == null)) || (this.CacheValidator(this) == true)));
			}
		}

		/// <summary>
		/// The value maintained by the cache.
		/// </summary>
		public T Value
		{
			get
			{
				this.valueLock.AcquireReaderLock(lockTimeout);
				try
				{
					if ((this.ValueProvider != null) && (this.IsValueValid == false))
					{
						LockCookie lc = this.valueLock.UpgradeToWriterLock(lockTimeout);
						try
						{
							T value = this.ValueProvider();
							this.setValue(value);
						}
						catch (IgnoreFailureException)
						{
							// Ignore failure, except if not yet initialized:
							if (this.initialized == false)
								throw new Exception("Cache initialization failed.");
						}
						finally
						{
							this.valueLock.DowngradeFromWriterLock(ref lc);
						}
					}
					return this.value;
				}
				finally
				{
					this.valueLock.ReleaseReaderLock();
				}
			}
			set
			{
				this.valueLock.AcquireWriterLock(lockTimeout);
				try
				{
					this.setValue(value);
				}
				finally
				{
					this.valueLock.ReleaseWriterLock();
				}
			}
		}

		/// <summary>
		/// The DateTime (in localtime) the value maintained by the cache was last acquired.
		/// </summary>
		public DateTime ValueTime
		{
			get
			{
				this.valueLock.AcquireReaderLock(lockTimeout);
				try
				{
					return this.valueTime;
				}
				finally
				{
					this.valueLock.ReleaseReaderLock();
				}
			}
		}

		/// <summary>
		/// The age of the value in the cache.
		/// </summary>
		public TimeSpan ValueAge
		{
			get
			{
				this.valueLock.AcquireReaderLock(lockTimeout);
				try
				{
                    return new TimeSpan(System.Current.DateTime.Now.Ticks - this.valueTime.Ticks);
				}
				finally
				{
					this.valueLock.ReleaseReaderLock();
				}
			}
		}

		/// <summary>
		/// Marks the value in the cache as invalid.
		/// </summary>
		public void Invalidate()
		{
			if (this.ValueProvider == null) throw new InvalidOperationException("Only a cache having a ValueProvider can be invalidated.");
			
			this.valueLock.AcquireReaderLock(lockTimeout);
			try
			{
				this.invalidated = true;
			}
			finally
			{
				this.valueLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Updates the value without locking.
		/// </summary>
		private void setValue(T value)
		{
			this.value = value;
            this.valueTime = System.Current.DateTime.Now;
			this.initialized = true;
		}
	}
}
