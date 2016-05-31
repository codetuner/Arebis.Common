using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Threading;
using System.Globalization;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Activation;
using System.Reflection;

namespace Arebis.Runtime
{
	/// <summary>
	/// Proxy to an object hosted in a separate AppDomain.
	/// </summary>
	/// <example>
	/// // To host an instance of MyType in a separate AppDomain, and call
	/// // the MyMethod() on it:
	///	AppDomainSetup info = new AppDomainSetup();
	///	using (AppDomainProxy p = new AppDomainProxy(typeof(MyType), info))
	///	{
	///		MyType t = (MyType)p.GetTransparentProxy();
	/// 	t.MyMethod();
	///	}
	/// </example>
	public class AppDomainProxy : RealProxy, IDisposable
	{
		private AppDomainSetup setupInfo;
		private AppDomain appDomain;
		private object instance;

		public AppDomainProxy(Type classToProxy, AppDomainSetup setupInfo)
			: base(classToProxy)
		{
			this.setupInfo = setupInfo;
		}

		public override IMessage Invoke(IMessage msg)
		{
			if (this.instance == null)
			{
				AppDomain ad = this.AppDomain;

				this.instance = ad.CreateInstance(
					this.GetProxiedType().Assembly.FullName,
					this.GetProxiedType().FullName,
					false,
					BindingFlags.CreateInstance,
					null,
					new object[0],
					CultureInfo.CurrentCulture,
					null
				).Unwrap();
			}
			IMethodCallMessage mcm = msg as IMethodCallMessage;

			try
			{
				object result = this.GetProxiedType().InvokeMember(mcm.MethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, this.instance, mcm.Args);
				return new ReturnMessage(result, null, 0, mcm.LogicalCallContext, mcm);
			}
			catch (TargetInvocationException tex)
			{
				return new ReturnMessage(tex.InnerException, mcm);
			}
		}

		public virtual AppDomain AppDomain
		{
			get
			{
				if (this.appDomain == null)
				{
					this.appDomain = AppDomain.CreateDomain(
						this.AppDomainName,
						this.Evidence,
						this.SetupInfo
					);
				}
				return this.appDomain;
			}
		}

		public virtual string AppDomainName
		{
			get 
			{
				return String.Format("AppDomain for Proxy {0}", this.GetProxiedType().Name);
			}
		}

		public virtual AppDomainSetup SetupInfo
		{
			get 
			{
				return this.setupInfo;
			}
		}

		public virtual Evidence Evidence
		{
			get 
			{
				return AppDomain.CurrentDomain.Evidence;
			}
		}

		public void Dispose()
		{
			AppDomain.Unload(this.appDomain);
		}
	}
}
