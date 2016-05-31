using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;

namespace Arebis.Extensions.Tests.Arebis
{
	[TestClass]
	[Advisable]
	public class ObjectFactoryTests : ContextBoundObject
	{
		#region Initialization methods

		private ObjectFactory defaultFactory;

		[TestInitialize()]
		public void MyTestInitialize() 
		{
			// Store current objectFactory:
			this.defaultFactory = ObjectFactory.Instance;

			// Install virgin objectFactory:
			ObjectFactory.Instance = new ObjectFactory();
		}

		[TestCleanup()]
		public void MyTestCleanup() 
		{
			// Restore current objectFactory:
			ObjectFactory.Instance = this.defaultFactory;
		}

		#endregion Initialization methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RegisterFactoryClass01Test()
		{
			ObjectFactory.Instance.RegisterFactoryClass(null, typeof(FactorableSub));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RegisterFactoryClass02Test()
		{
			ObjectFactory.Instance.RegisterFactoryClass(typeof(IFactorable), null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterFactoryClass03Test()
		{
			ObjectFactory.Instance.RegisterFactoryClass(typeof(FactorableSub), typeof(IFactorable));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterFactoryClass04Test()
		{
			ObjectFactory.Instance.RegisterFactoryClass(typeof(FactorableSub), typeof(Factorable));
		}

		[TestMethod]
		public void RegisterFactoryClass05Test()
		{
			ObjectFactory.Instance.RegisterFactoryClass(typeof(IFactorable), typeof(FactorableSub));

			IFactorable f = ObjectFactory.Instance.Construct<IFactorable>();

			Assert.IsInstanceOfType(f, typeof(FactorableSub));
		}

		[TestMethod]
		public void RegisterFactoryClass06Test()
		{
			ObjectFactory.Instance.RegisterFactoryClass(typeof(IFactorable), typeof(FactorableSub));

			IFactorable f = ObjectFactory.Instance.Construct<IFactorable>(12);

			Assert.IsInstanceOfType(f, typeof(FactorableSub));
			Assert.AreEqual(12, f.Value);
		}

		[TestMethod]
		public void RegisterFactoryClass07Test()
		{
			// When registering IFactorable, but constructing Factorable, the registration
			// is ignored (as IFactorable != Factorable), and we get a regular Factorable back:

			ObjectFactory.Instance.RegisterFactoryClass(typeof(IFactorable), typeof(FactorableSub));

			Factorable f = ObjectFactory.Instance.Construct<Factorable>(12);

			Assert.IsInstanceOfType(f, typeof(Factorable));
			Assert.IsNotInstanceOfType(f, typeof(FactorableSub));
			Assert.AreEqual(12, f.Value);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void RegisterFactoryInstance01Test()
		{
			// Cannot register an instance that does not match the requesttype:
			ObjectFactory.Instance.RegisterFacoryInstance(typeof(ISerializable), new FactorableSub(17));
		}

		[TestMethod]
		public void RegisterFactoryInstance02Test()
		{
			ObjectFactory.Instance.RegisterFacoryInstance(typeof(IFactorable), new FactorableSub(17));

			IFactorable f = ObjectFactory.Instance.Construct<IFactorable>();
			IFactorable g = ObjectFactory.Instance.Construct<IFactorable>();

			Assert.AreEqual(17, f.Value);
			Assert.AreEqual(17, g.Value);
			Assert.AreEqual(f, g);
		}

		[TestMethod]
		public void RegisterFactoryMethod01Test()
		{
			ObjectFactory.Instance.RegisterFactoryMethod(
				typeof(IFactorable), 
				delegate(Type req, object[] args) { return new FactorableSub(48); }
			);

			IFactorable f = ObjectFactory.Instance.Construct<IFactorable>();
			IFactorable g = ObjectFactory.Instance.Construct<IFactorable>();

			Assert.AreEqual(48, f.Value);
			Assert.AreEqual(48, g.Value);
			Assert.AreNotEqual(f, g);
		}
	
		#region Test types

		interface IFactorable
		{
			int Value { get; set; }
		}

		class Factorable : IFactorable
		{
			protected Factorable()
			{ 
			}

			protected Factorable(int value)
			{
				this.Value = value;
			}

			public int Value { get; set; }
		}

		class FactorableSub : Factorable
		{
			public FactorableSub()
				: base()
			{ }

			public FactorableSub(int value)
				: base(value)
			{ }
		}

		#endregion Test types
	}
}
