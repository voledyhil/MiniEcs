using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class EcsComponentArrayTest
    {
        private class ComponentMock : IEcsComponent
        {
            public int Value;
        }

        [TestMethod]
        public void AddComponentTest()
        {
            uint entity53 = EcsEntityIdentifiers.Generate(id: 5, version: 3);
            uint entity14 = EcsEntityIdentifiers.Generate(id: 1, version: 4);

            EcsComponentFromEntity<ComponentMock> components = new EcsComponentFromEntity<ComponentMock>(10);
            components.Set(entity53, new ComponentMock {Value = 53});
            components.Set(entity14, new ComponentMock {Value = 14});
            
            components.Set(entity14, new ComponentMock {Value = 14});
            Assert.AreEqual(2, components.Count);
        }
        
        [TestMethod]
        public void RemoveComponentTest()
        {
            uint entity53 = EcsEntityIdentifiers.Generate(id: 5, version: 3);

            EcsComponentFromEntity<ComponentMock> components = new EcsComponentFromEntity<ComponentMock>(10);
            components.Set(entity53, new ComponentMock {Value = 53});
            components.Remove(entity53);
            
            Assert.AreEqual(0, components.Count);
        }
        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetComponentTest()
        {
            uint entity53 = EcsEntityIdentifiers.Generate(id: 5, version: 3);
            uint entity63 = EcsEntityIdentifiers.Generate(id: 6, version: 3);

            EcsComponentFromEntity<ComponentMock> components = new EcsComponentFromEntity<ComponentMock>(10);
            const int componentValue = 53;
            
            components.Set(entity53, new ComponentMock {Value = componentValue});
            
            Assert.AreEqual(componentValue, components[entity53].Value);
            Assert.AreEqual(componentValue, components[entity63].Value); //ExpectedException 
        }
        
        [TestMethod]
        public void ContainComponentTest()
        {
            uint entity53 = EcsEntityIdentifiers.Generate(id: 5, version: 3);
            uint entity14 = EcsEntityIdentifiers.Generate(id: 1, version: 4);

            EcsComponentFromEntity<ComponentMock> components = new EcsComponentFromEntity<ComponentMock>(10);
            components.Set(entity53, new ComponentMock {Value = 53});

            Assert.IsTrue(components.Contain(entity53));
            Assert.IsFalse(components.Contain(entity14));
        }
    }
}