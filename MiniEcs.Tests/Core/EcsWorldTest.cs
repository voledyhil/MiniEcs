using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class EcsWorldTest
    {
        private class ComponentA : IEcsComponent
        {
        }

        private class ComponentB : IEcsComponent
        {
        }

        private class ComponentC : IEcsComponent
        {
        }
        
        private class ComponentD : IEcsComponent
        {
        }
        
        private static EcsWorld _world;
        private static uint _entityAB;
        private static uint _entityABD;
        private static uint _entityAC;
        private static uint _entityAD;
        private static uint _entityBC;
        private static uint _entityBD0;
        private static uint _entityBD1;

        [ClassInitialize]
        public static void InitFilterWorld(TestContext testContext)
        {
            _world = new EcsWorld();
            _world.RegisterComponent<ComponentA>();
            _world.RegisterComponent<ComponentB>();
            _world.RegisterComponent<ComponentC>();
            _world.RegisterComponent<ComponentD>();

            _entityABD = _world.CreateEntity(new ComponentA(), new ComponentB(), new ComponentD());
            _entityAC = _world.CreateEntity(new ComponentA(), new ComponentC());
            _entityBD0 = _world.CreateEntity(new ComponentB(), new ComponentD());
            _entityBD1 = _world.CreateEntity(new ComponentB(), new ComponentD());
            _entityBC = _world.CreateEntity(new ComponentB(), new ComponentC());
            _entityAB = _world.CreateEntity(new ComponentA(), new ComponentB());
            _entityAD = _world.CreateEntity(new ComponentA(), new ComponentD());
        }

        [TestMethod]
        public void GetArchetypeTest()
        {
            EcsWorld world = new EcsWorld();
            world.RegisterComponent<ComponentA>();
            world.RegisterComponent<ComponentB>();
            world.RegisterComponent<ComponentC>();
            world.RegisterComponent<ComponentD>();
            
            uint entityA = world.CreateEntity(new ComponentA());
            uint entityBCA = world.CreateEntity(new ComponentB(), new ComponentC(), new ComponentA());

            world.CreateEntity();
            world.CreateEntity(new ComponentA(), new ComponentB(), new ComponentC());
            world.CreateEntity(new ComponentB(), new ComponentC(), new ComponentD());
            world.CreateEntity(new ComponentB(), new ComponentC());

            Assert.AreEqual(1, world.GetArchetype().EntitiesCount);
            Assert.AreEqual(1, world.GetArchetype(typeof(ComponentA)).EntitiesCount);           
            Assert.AreEqual(0, world.GetArchetype(typeof(ComponentB)).EntitiesCount);     
            Assert.AreEqual(1, world.GetArchetype(typeof(ComponentB), typeof(ComponentC)).EntitiesCount);
            Assert.AreEqual(0, world.GetArchetype(typeof(ComponentC), typeof(ComponentD)).EntitiesCount);
            Assert.AreEqual(2, world.GetArchetype(typeof(ComponentA), typeof(ComponentB), typeof(ComponentC)).EntitiesCount);
            Assert.AreEqual(1, world.GetArchetype(typeof(ComponentB), typeof(ComponentC), typeof(ComponentD)).EntitiesCount);   
            
            world.DestroyEntity(entityBCA);
            world.DestroyEntity(entityA);
            
            Assert.AreEqual(0, world.GetArchetype(typeof(ComponentA)).EntitiesCount); 
            Assert.AreEqual(1, world.GetArchetype(typeof(ComponentA), typeof(ComponentB), typeof(ComponentC)).EntitiesCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetSetHasRemoveComponentTest()
        {
            EcsWorld world = new EcsWorld();
            world.RegisterComponent<ComponentA>();
            world.RegisterComponent<ComponentB>();
            world.RegisterComponent<ComponentC>();
            
            ComponentB componentB = new ComponentB();
            uint entity = world.CreateEntity(new ComponentA());
            world.SetComponent(entity, componentB);
            world.SetComponent(entity, new ComponentC());

            IEcsArchetype archetypeAB = world.GetArchetype(typeof(ComponentA), typeof(ComponentB));
            IEcsArchetype archetypeAC = world.GetArchetype(typeof(ComponentA), typeof(ComponentC));
            IEcsArchetype archetypeABC = world.GetArchetype(typeof(ComponentA), typeof(ComponentB), typeof(ComponentC));

            Assert.AreEqual(0, archetypeAB.EntitiesCount);
            Assert.AreEqual(1, archetypeABC.EntitiesCount);
            Assert.IsTrue(world.HasComponent<ComponentC>(entity));
            Assert.AreEqual(componentB, world.GetComponent<ComponentB>(entity));

            world.RemoveComponent<ComponentB>(entity);
            Assert.IsFalse(world.HasComponent<ComponentB>(entity));
            Assert.AreEqual(0, archetypeAB.EntitiesCount);
            Assert.AreEqual(1, archetypeAC.EntitiesCount);
            Assert.AreEqual(0, archetypeABC.EntitiesCount);

            world.RemoveComponent<ComponentB>(entity); //ExpectedException
        }

        [TestMethod]
        public void AllFilterTest()
        {
            List<uint> entities = _world.FilterEntities(new EcsFilter().AllOf<ComponentB>());
            Assert.AreEqual(5, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            Assert.IsTrue(entities.Contains(_entityAB));
            
            entities = _world.FilterEntities(new EcsFilter().AllOf<ComponentB, ComponentD>());
            Assert.AreEqual(3, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
        }

        [TestMethod]
        public void AnyFilterTest()
        {
            List<uint> entities = _world.FilterEntities(new EcsFilter().AnyOf<ComponentB>());
            Assert.AreEqual(5, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            Assert.IsTrue(entities.Contains(_entityAB));
            
            entities = _world.FilterEntities(new EcsFilter().AnyOf<ComponentB, ComponentD>());
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
            List<uint> entities = _world.FilterEntities(new EcsFilter().NoneOf<ComponentB, ComponentD>());
            Assert.AreEqual(1, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityAC));
            
            entities = _world.FilterEntities(new EcsFilter().NoneOf<ComponentB, ComponentD, ComponentB>());
            Assert.AreEqual(1, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityAC));
        }
      
        [TestMethod]
        public void AllAnyFilterTest()
        {
            List<uint> entities = _world.FilterEntities(new EcsFilter().AllOf<ComponentB, ComponentB, ComponentD>().AnyOf<ComponentA>());
            Assert.AreEqual(1, entities.Count);
            Assert.IsTrue(entities.Contains(_entityABD));
            
            entities = _world.FilterEntities(new EcsFilter().AllOf<ComponentD, ComponentD>().AnyOf<ComponentB, ComponentC, ComponentC>());
            Assert.AreEqual(3, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityABD));
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
        }
        
  
        [TestMethod]
        public void AllNoneFilterTest()
        {
            List<uint> entities = _world.FilterEntities(new EcsFilter().AllOf<ComponentB>().NoneOf<ComponentA>());
            Assert.AreEqual(3, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
            Assert.IsTrue(entities.Contains(_entityBC));
            
            entities = _world.FilterEntities(new EcsFilter().AllOf<ComponentB, ComponentD>().NoneOf<ComponentA>());
            Assert.AreEqual(2, entities.Count);
            
            Assert.IsTrue(entities.Contains(_entityBD0));
            Assert.IsTrue(entities.Contains(_entityBD1));
        }

    }
}