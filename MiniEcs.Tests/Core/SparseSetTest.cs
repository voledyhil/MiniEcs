using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class SparseSetTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetTest()
        {
            SparseSet set = new SparseSet(10);
            set.Set(5);
            set.Set(1);
            set.Set(1);

            Assert.AreEqual(2, set.Length);

            set.Set(10); //ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UnSetTest()
        {
            SparseSet set = new SparseSet(10);
            set.Set(5);
            set.Set(1);
            set.UnSet(5);
            set.UnSet(5);

            Assert.AreEqual(1, set.Length);

            set.UnSet(10); //ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ContainTest()
        {
            SparseSet set = new SparseSet(10);
            set.Set(5);
            set.Set(3);

            Assert.IsTrue(set.Contain(5));
            Assert.IsFalse(set.Contain(1));

            set.Contain(10); //ExpectedException
        }

        [TestMethod]
        public void ClearTest()
        {
            SparseSet set = new SparseSet(10);

            set.Set(5);
            set.Clear();

            Assert.AreEqual(0, set.Length);
        }

        [TestMethod]
        public void SortTest()
        {
            SparseSet set = new SparseSet(10);
            set.Set(2);
            set.Set(5);
            set.Set(3);
            set.Set(4);
            set.Set(1);

            List<ushort> values = new List<ushort>(set);
            
            Assert.AreEqual(2, values[0]);
            Assert.AreEqual(5, values[1]);
            Assert.AreEqual(3, values[2]);
            Assert.AreEqual(4, values[3]);
            Assert.AreEqual(1, values[4]);
            
            Assert.IsTrue(set.Contain(2, out ushort sparse));
            Assert.AreEqual(0, sparse);
            
            Assert.IsTrue(set.Contain(5, out sparse));
            Assert.AreEqual(1, sparse);
            
            Assert.IsTrue(set.Contain(3, out sparse));
            Assert.AreEqual(2, sparse);
            
            Assert.IsTrue(set.Contain(4, out sparse));
            Assert.AreEqual(3, sparse);
            
            Assert.IsTrue(set.Contain(1, out sparse));
            Assert.AreEqual(4, sparse);
            
            set.Sort();
            
            
            values = new List<ushort>(set);
            
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
            Assert.AreEqual(4, values[3]);
            Assert.AreEqual(5, values[4]);

            Assert.IsTrue(set.Contain(1, out sparse));
            Assert.AreEqual(0, sparse);
            
            Assert.IsTrue(set.Contain(2, out sparse));
            Assert.AreEqual(1, sparse);
            
            Assert.IsTrue(set.Contain(3, out sparse));
            Assert.AreEqual(2, sparse);
            
            Assert.IsTrue(set.Contain(4, out sparse));
            Assert.AreEqual(3, sparse);
            
            Assert.IsTrue(set.Contain(5, out sparse));
            Assert.AreEqual(4, sparse);
        }
    }
}