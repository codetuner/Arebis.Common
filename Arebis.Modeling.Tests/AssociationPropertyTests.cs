using Arebis.Modeling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Arebis.Modeling.Tests
{
    [TestClass()]
    public class AssociationPropertyTests
    {
        [TestMethod()]
        public void AssociationProperty_1_1_Test()
        {
            var subject = new AssociationProperty<A, B>("1A - 1B", AssociationMultiplicity.Single, AssociationMultiplicity.Single);

            A a1 = new A(), a2 = new A(), a3 = new A();
            B b1 = new B(), b2 = new B(), b3 = new B();

            Assert.AreEqual(0, subject.GetTargetCollectionFor(a1).Count);
            Assert.AreEqual(0, subject.GetSourceCollectionFor(b1).Count);
            Assert.AreEqual(0, subject.GetTargetCollectionFor(a2).Count);
            Assert.AreEqual(0, subject.GetSourceCollectionFor(b2).Count);

            subject.SetOrAdd(a1, b1);
            subject.SetOrAdd(a2, b2);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b2 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a2 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());

            subject.SetOrAdd(a1, b2);

            CollectionAssert.AreEqual((new B[] { b2 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());

            subject.SetOrAdd(a1, null);

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());

            subject.SetOrAdd(a1, b1);
            subject.SetOrAdd(a2, b1);

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a2 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());

            subject.SetOrAdd(null, b1);

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
        }

        [TestMethod()]
        public void AssociationProperty_1_n_Test()
        {
            var subject = new AssociationProperty<A, B>("1A - nB", AssociationMultiplicity.Single, AssociationMultiplicity.Multiple);

            A a1 = new A(), a2 = new A(), a3 = new A();
            B b1 = new B(), b2 = new B(), b3 = new B();

            subject.SetOrAdd(a1, b1);
            subject.SetOrAdd(a1, b2);
            subject.SetOrAdd(a1, b3);

            CollectionAssert.AreEqual((new B[] { b1, b2, b3 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a3, b3);

            CollectionAssert.AreEqual((new B[] { b1, b2 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.Remove(a1, b2);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(null, b3);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.GetTargetCollectionFor(a1).Clear();

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());
        }

        [TestMethod()]
        public void AssociationProperty_n_1_Test()
        {
            var subject = new AssociationProperty<A, B>("nA - 1B", AssociationMultiplicity.Multiple, AssociationMultiplicity.Single);

            A a1 = new A(), a2 = new A(), a3 = new A();
            B b1 = new B(), b2 = new B(), b3 = new B();

            subject.SetOrAdd(a1, b1);
            subject.SetOrAdd(a2, b1);
            subject.SetOrAdd(a3, b1);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a3, b3);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.Remove(a2, b1);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a3, null);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.GetSourceCollectionFor(b1).Clear();

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());
        }


        [TestMethod()]
        public void AssociationProperty_n_n_Test()
        {
            var subject = new AssociationProperty<A, B>("nA - nB", AssociationMultiplicity.Multiple, AssociationMultiplicity.Multiple);

            A a1 = new A(), a2 = new A(), a3 = new A();
            B b1 = new B(), b2 = new B(), b3 = new B();

            subject.SetOrAdd(a1, b1);
            subject.SetOrAdd(a2, b1);
            subject.SetOrAdd(a3, b1);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a3, b2);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a3, b3);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2, b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a2, b2);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a3, a2 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2, b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.SetOrAdd(a2, b2);

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a2, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2 }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a3, a2 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2, b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.GetTargetCollectionFor(a2).Clear();

            CollectionAssert.AreEqual((new B[] { b1 }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { a1, a3 }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b1, b2, b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.GetSourceCollectionFor(b1).Clear();

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { b2, b3 }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { a3 }).ToList(), subject.GetSourceCollectionFor(b3).ToList());

            subject.GetTargetCollectionFor(a3).Clear();

            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a1).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b1).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a2).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b2).ToList());
            CollectionAssert.AreEqual((new B[] { }).ToList(), subject.GetTargetCollectionFor(a3).ToList());
            CollectionAssert.AreEqual((new A[] { }).ToList(), subject.GetSourceCollectionFor(b3).ToList());
        }

        #region Helper classes

        class A { }

        class B { }

        #endregion
    }
}
