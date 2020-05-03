using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    
    [TestClass]
    public class EcsWorldTest
    {
        private static EcsWorld _world;
        private static EcsEntity _entityAB;
        private static EcsEntity _entityABD;
        private static EcsEntity _entityAC;
        private static EcsEntity _entityAD;
        private static EcsEntity _entityBC;
        private static EcsEntity _entityBD0;
        private static EcsEntity _entityBD1;

        [ClassInitialize]
        public static void InitFilterWorld(TestContext testContext)
        {
            _world = new EcsWorld(ComponentType.Capacity);

            _entityABD = _world.CreateEntity();
            _entityABD[ComponentType.A] = new ComponentA();
            _entityABD[ComponentType.B] = new ComponentB();
            _entityABD[ComponentType.D] = new ComponentD();
            
            _entityAC = _world.CreateEntity();
            _entityAC[ComponentType.A] = new ComponentA();
            _entityAC[ComponentType.C] = new ComponentC();
            
            _entityBD0 = _world.CreateEntity();
            _entityBD0[ComponentType.B] = new ComponentB();
            _entityBD0[ComponentType.D] = new ComponentD();
            
            _entityBD1 = _world.CreateEntity();
            _entityBD1[ComponentType.B] = new ComponentB();
            _entityBD1[ComponentType.D] = new ComponentD();

            _entityBC = _world.CreateEntity();
            _entityBC[ComponentType.B] = new ComponentB();
            _entityBC[ComponentType.C] = new ComponentC();

            _entityAB = _world.CreateEntity();
            _entityAB[ComponentType.A] = new ComponentA();
            _entityAB[ComponentType.B] = new ComponentB();
            
            _entityAD = _world.CreateEntity();
            _entityAD[ComponentType.A] = new ComponentA();
            _entityAD[ComponentType.D] = new ComponentD();
        }

        [TestMethod]
        public void GetArchetypeTest()
        {
            EcsWorld world = new EcsWorld(ComponentType.Capacity);
            
            EcsEntity entityA = world.CreateEntity();
            entityA[ComponentType.A] = new ComponentA();
            
            EcsEntity entityBCA = world.CreateEntity();
            entityBCA[ComponentType.B] = new ComponentB();
            entityBCA[ComponentType.C] = new ComponentB();
            entityBCA[ComponentType.A] = new ComponentB();

            world.CreateEntity();
            
            EcsEntity entityABC = world.CreateEntity();
            entityABC[ComponentType.A] = new ComponentA();
            entityABC[ComponentType.B] = new ComponentB();
            entityABC[ComponentType.C] = new ComponentC();
            
            EcsEntity entityBCD = world.CreateEntity();
            entityBCD[ComponentType.B] = new ComponentB();
            entityBCD[ComponentType.C] = new ComponentC();
            entityBCD[ComponentType.D] = new ComponentD();
            
            EcsEntity entityBC = world.CreateEntity();
            entityBC[ComponentType.B] = new ComponentB();
            entityBC[ComponentType.C] = new ComponentC();
            
            Assert.AreEqual(1, world.GetArchetype().ToArray().Length);
            Assert.AreEqual(1, world.GetArchetype(ComponentType.A).ToArray().Length);           
            Assert.AreEqual(0, world.GetArchetype(ComponentType.B).ToArray().Length);     
            Assert.AreEqual(1, world.GetArchetype(ComponentType.B, ComponentType.C).ToArray().Length);
            Assert.AreEqual(0, world.GetArchetype(ComponentType.C, ComponentType.D).ToArray().Length);
            Assert.AreEqual(2, world.GetArchetype(ComponentType.A, ComponentType.B, ComponentType.C).ToArray().Length);
            Assert.AreEqual(1, world.GetArchetype(ComponentType.B, ComponentType.C, ComponentType.D).ToArray().Length);   
            
            entityBCA.Destroy();
            entityA.Destroy();
            
            Assert.AreEqual(0, world.GetArchetype(ComponentType.A).ToArray().Length); 
            Assert.AreEqual(1, world.GetArchetype(ComponentType.A, ComponentType.B, ComponentType.C).ToArray().Length);
        }

        [TestMethod]
        public void GetSetRemoveComponentTest()
        {
            EcsWorld world = new EcsWorld(ComponentType.Capacity);
            
            ComponentB componentB = new ComponentB();
            EcsEntity entity = world.CreateEntity();
            entity[ComponentType.A] = new ComponentA();
            entity[ComponentType.B] = componentB;
            entity[ComponentType.C] = new ComponentC();

            Assert.IsNotNull(entity[ComponentType.A]);
            Assert.IsNotNull(entity[ComponentType.C]);
            Assert.AreEqual(componentB, entity[ComponentType.B]);
            Assert.IsNull(entity[ComponentType.D]);

            entity[ComponentType.B] = null;
            Assert.IsNull(entity[ComponentType.B]);
        }

        [TestMethod]
        public void AllFilterTest()
        {
            List<EcsEntity> entities = _world.Filter(new EcsFilter().AllOf(ComponentType.B)).ToList();
            Assert.AreEqual(5, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            Assert.IsTrue(entities.Contains(_entityAB));
            
            entities = _world.Filter(new EcsFilter().AllOf(ComponentType.B, ComponentType.D)).ToList();
            Assert.AreEqual(3, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
        }

        [TestMethod]
        public void AnyFilterTest()
        {
            List<EcsEntity> entities = _world.Filter(new EcsFilter().AnyOf(ComponentType.B)).ToList();
            Assert.AreEqual(5, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            Assert.IsTrue(entities.Contains(_entityAB));
            
            entities = _world.Filter(new EcsFilter().AnyOf(ComponentType.B, ComponentType.D)).ToList();
            Assert.AreEqual(6, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            Assert.IsTrue(entities.Contains(_entityAB));
            Assert.IsTrue(entities.Contains(_entityAD));
        }
        
        [TestMethod]
        public void NoneFilterTest()
        {
            List<EcsEntity> entities = _world.Filter(new EcsFilter().NoneOf(ComponentType.B, ComponentType.D)).ToList();
            Assert.AreEqual(1, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityAC));
            
            entities = _world.Filter(new EcsFilter().NoneOf(ComponentType.B, ComponentType.D, ComponentType.B)).ToList();
            Assert.AreEqual(1, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityAC));
        }
      
        [TestMethod]
        public void AllAnyFilterTest()
        {
            List<EcsEntity> entities = _world.Filter(new EcsFilter().AllOf(ComponentType.B, ComponentType.B, ComponentType.D).AnyOf(ComponentType.A)).ToList();
            Assert.AreEqual(1, entities.Count);
            Assert.IsTrue(entities.Contains(_entityABD));
            
            entities = _world.Filter(new EcsFilter().AllOf(ComponentType.D, ComponentType.D).AnyOf(ComponentType.B, ComponentType.C, ComponentType.C)).ToList();
            Assert.AreEqual(3, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
        }
        
  
        [TestMethod]
        public void AllNoneFilterTest()
        {
            List<EcsEntity> entities = _world.Filter(new EcsFilter().AllOf(ComponentType.B).NoneOf(ComponentType.A)).ToList();
            Assert.AreEqual(3, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            
            entities = _world.Filter(new EcsFilter().AllOf(ComponentType.B, ComponentType.D).NoneOf(ComponentType.A)).ToList();
            Assert.AreEqual(2, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
        }

    }
}