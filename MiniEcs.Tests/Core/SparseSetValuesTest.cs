using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class SparseDictionaryTest
    {
        [TestMethod]
        public void AddTest()
        {
            SparseSetValues<ushort> dictionary = new SparseSetValues<ushort>(10);
            dictionary[5] = 3;
            dictionary[1] = 4;
            dictionary[1] = 4;
            
            Assert.AreEqual(2, dictionary.Count);
        }
        
        [TestMethod]
        public void RemoveTest()
        {
            SparseSetValues<ushort> dictionary = new SparseSetValues<ushort>(10);
            dictionary[5] = 3;
            dictionary.Remove(5);
            
            Assert.AreEqual(0, dictionary.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetTest()
        {
            SparseSetValues<ushort> dictionary = new SparseSetValues<ushort>(10);
            
            dictionary[5] = 3;
            
            Assert.AreEqual(3, dictionary[5]);
            Assert.AreEqual(3, dictionary[6]); //ExpectedException 
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ContainTest()
        {
            SparseSetValues<ushort> dictionary = new SparseSetValues<ushort>(10);
            dictionary[5] = 3;

            Assert.IsTrue(dictionary.Contain(5));
            Assert.IsFalse(dictionary.Contain(1));
            
            dictionary.Contain(10);  //ExpectedException
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TryGetValueTest()
        {
            SparseSetValues<ushort> dictionary = new SparseSetValues<ushort>(10);
            
            Assert.IsFalse(dictionary.TryGetValue(5, out ushort value));
            
            dictionary[5] = 3;
            dictionary[4] = 5;

            Assert.IsTrue(dictionary.TryGetValue(5, out value));
            Assert.AreEqual(3, value);
            Assert.IsFalse(dictionary.TryGetValue(1, out value));
            
            dictionary.TryGetValue(10, out value);  //ExpectedException
        }

        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ClearTest()
        {
            SparseSetValues<ushort> dictionary = new SparseSetValues<ushort>(10);
            
            dictionary[5] = 3;

            Assert.AreEqual(1, dictionary.Count);
            
            dictionary.Clear();
            
            Assert.AreEqual(0, dictionary.Count);
            Assert.IsFalse(dictionary.Contain(5));
            Assert.AreEqual(3, dictionary[5]); //ExpectedException
        }
    }
}