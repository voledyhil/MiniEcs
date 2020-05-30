using System;
using System.Collections.Generic;
using BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public partial class EcsWorldTest
    {
        private static EcsWorld _world;
        private static IEcsEntity _entityAB;
        private static IEcsEntity _entityABD;
        private static IEcsEntity _entityAC;
        private static IEcsEntity _entityAD;
        private static IEcsEntity _entityBC;
        private static IEcsEntity _entityBD0;
        private static IEcsEntity _entityBD1;

        [ClassInitialize]
        public static void InitFilterWorld(TestContext testContext)
        {
            EcsComponentType<ComponentA>.Register();
            EcsComponentType<ComponentB>.Register();
            EcsComponentType<ComponentC>.Register();
            EcsComponentType<ComponentD>.Register();

            _world = new EcsWorld();

            _entityABD = _world.CreateEntity(new ComponentA {Value = 1}, new ComponentB {Value = 2}, new ComponentD {Value = 3});
            _entityAC = _world.CreateEntity(new ComponentA {Value = 4}, new ComponentC {Value = 5});
            _entityBD0 = _world.CreateEntity(new ComponentB {Value = 6}, new ComponentD {Value = 7});
            _entityBD1 = _world.CreateEntity(new ComponentD {Value = 8}, new ComponentB {Value = 8});
            _entityBC = _world.CreateEntity(new ComponentC {Value = 9}, new ComponentB {Value = 10});
            _entityAB = _world.CreateEntity(new ComponentB {Value = 11}, new ComponentA {Value = 12});
            _entityAD = _world.CreateEntity(new ComponentA {Value = 13}, new ComponentD {Value = 14});
        }

        [TestMethod]
        public void GetSetRemoveComponentTest()
        {
            EcsWorld world = new EcsWorld();
            
            ComponentB componentB = new ComponentB();
            IEcsEntity entity = world.CreateEntity();
            entity.AddComponent(new ComponentA());
            entity.AddComponent(componentB);
            entity.AddComponent(new ComponentC());

            Assert.IsNotNull(entity.GetComponent<ComponentA>());
            Assert.IsNotNull(entity.GetComponent<ComponentC>());
            Assert.AreEqual(componentB, entity.GetComponent<ComponentB>());
            Assert.IsFalse(entity.HasComponent<ComponentD>());

            entity.RemoveComponent<ComponentB>();
            Assert.IsFalse(entity.HasComponent<ComponentB>());
        }

        [TestMethod]
        public void AllFilterTest()
        {
            List<IEcsEntity> entities = new List<IEcsEntity>
            {
                _entityABD, _entityBD0, _entityBD1, _entityBC, _entityAB
            };

            IEcsGroup group = _world.Filter(new EcsFilter().AllOf<ComponentB>());
            Assert.AreEqual(entities.Count, group.CalculateCount());
            group.ForEach((IEcsEntity entity, ComponentB compB) =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });

            
            
            entities = new List<IEcsEntity>
            {
                _entityABD, _entityBD0, _entityBD1
            };
            
            group = _world.Filter(new EcsFilter().AllOf<ComponentB, ComponentD>());
            Assert.AreEqual(3, group.CalculateCount());            
            group.ForEach((IEcsEntity entity, ComponentB compB, ComponentD compD) =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
            
        }

        [TestMethod]
        public void AnyFilterTest()
        {
            List<IEcsEntity> entities = new List<IEcsEntity>
            {
                _entityABD, _entityBD0, _entityBD1, _entityBC, _entityAB
            };

            IEcsGroup group = _world.Filter(new EcsFilter().AnyOf<ComponentB>());
            Assert.AreEqual(entities.Count, group.CalculateCount());
            group.ForEach((IEcsEntity entity, ComponentB compB) =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
            
             
            
            entities = new List<IEcsEntity>
            {
                _entityABD, _entityBD0, _entityBD1, _entityBC, _entityAB, _entityAD
            };
            
            group = _world.Filter(new EcsFilter().AnyOf<ComponentB, ComponentD>());
            Assert.AreEqual(entities.Count, group.CalculateCount());            
            group.ForEach(entity =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
        }
        
        [TestMethod]
        public void NoneFilterTest()
        {
            List<IEcsEntity> entities = new List<IEcsEntity>
            {
                _entityAC
            };

            IEcsGroup group = _world.Filter(new EcsFilter().NoneOf<ComponentB, ComponentD>());
            Assert.AreEqual(entities.Count, group.CalculateCount());
            group.ForEach(entity =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
            
            
            
            group = _world.Filter(new EcsFilter().NoneOf<ComponentB, ComponentD, ComponentB>());
            Assert.AreEqual(entities.Count, group.CalculateCount());
            group.ForEach(entity =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
        }
      
        
        [TestMethod]
        public void AllAnyFilterTest()
        {
            List<IEcsEntity> entities = new List<IEcsEntity>
            {
                _entityABD
            };

            IEcsGroup group = _world.Filter(new EcsFilter().AllOf<ComponentB, ComponentB, ComponentD>().AnyOf<ComponentA>());
            Assert.AreEqual(entities.Count, group.CalculateCount());
            group.ForEach(entity =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
            
            
            
            entities = new List<IEcsEntity>
            {
                _entityABD, _entityBD0, _entityBD1
            };
            
            group = _world.Filter(new EcsFilter().AllOf<ComponentD, ComponentD>().AnyOf<ComponentB, ComponentC, ComponentC>());
            Assert.AreEqual(entities.Count, group.CalculateCount());            
            group.ForEach(entity =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
        }
        
  
        [TestMethod]
        public void AllNoneFilterTest()
        {
            List<IEcsEntity> entities = new List<IEcsEntity>
            {
                _entityBD0, _entityBD1, _entityBC
            };

            IEcsGroup group = _world.Filter(new EcsFilter().AllOf<ComponentB>().NoneOf<ComponentA>());
            Assert.AreEqual(entities.Count, group.CalculateCount());
            group.ForEach((IEcsEntity entity, ComponentB compB) =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
            
            
                        
            entities = new List<IEcsEntity>
            {
                _entityBD0, _entityBD1
            };
            
            group = _world.Filter(new EcsFilter().AllOf<ComponentB, ComponentD>().NoneOf<ComponentA>());
            Assert.AreEqual(entities.Count, group.CalculateCount());            
            group.ForEach(entity =>
            {
                Assert.IsTrue(entities.Contains(entity));
            });
        }
        
        [TestMethod]
        public void GroupIncVersionFilterTest()
        {
            EcsWorld world = new EcsWorld();            
            IEcsEntity entity = world.CreateEntity(new ComponentA(), new ComponentB());
            Assert.AreEqual(1, world.Filter(new EcsFilter().AllOf<ComponentB>()).CalculateCount());
          
            entity.AddComponent(new ComponentC());  
            world.CreateEntity(new ComponentC(), new ComponentD());
            
            Assert.AreEqual(1, world.Filter(new EcsFilter().AllOf<ComponentB>()).CalculateCount());
        }

        
        [TestMethod]
        public void GetOrCreateSingletonTest()
        {
            EcsWorld world = new EcsWorld();

            ComponentA componentA0 = world.GetOrCreateSingleton<ComponentA>();
            ComponentA componentA1 = world.GetOrCreateSingleton<ComponentA>();
            
            Assert.AreEqual(componentA0, componentA1);
        }

        [TestMethod]
        public void CreateEntityFromProcessingTest()
        {
            EcsWorld world = new EcsWorld();
            Assert.AreEqual(0, world.EntitiesInProcessing);

            IEcsEntity entity = world.CreateEntity(new ComponentA(), new ComponentB(), new ComponentC(), new ComponentD());
            uint entityId = entity.Id;
            entity.Destroy();
            
            Assert.AreEqual(1, world.EntitiesInProcessing);
            IEcsEntity newEntity = world.CreateEntity(new ComponentB());
            uint newEntityId = newEntity.Id;
            Assert.AreEqual(0, world.EntitiesInProcessing);
            
            Assert.IsFalse(newEntity.HasComponent<ComponentA>());
            Assert.IsTrue(newEntity.HasComponent<ComponentB>());

            Assert.IsTrue(newEntityId > entityId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddComponentThrowExceptionTest()
        {
            EcsWorld world = new EcsWorld();
            IEcsEntity entity = world.CreateEntity();
            entity.AddComponent(new ComponentA());
            entity.AddComponent(new ComponentA());
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveComponentThrowExceptionTest()
        {
            EcsWorld world = new EcsWorld();
            IEcsEntity entity = world.CreateEntity();
            entity.RemoveComponent<ComponentA>();
        }


        [TestMethod]
        public void ArchetypeRemoveHolesTest()
        {
            EcsWorld world = new EcsWorld();
            IEcsArchetype archetype = world.GetArchetype<ComponentA, ComponentB>();
            
            IEcsEntity entity0 = world.CreateEntity(new ComponentA(), new ComponentB());
            IEcsEntity entity1 = world.CreateEntity(new ComponentA(), new ComponentB());
            IEcsEntity entity2 = world.CreateEntity(new ComponentA(), new ComponentB());
            IEcsEntity entity3 = world.CreateEntity(new ComponentA(), new ComponentB());
            
            entity1.RemoveComponent<ComponentA>();
            entity2.RemoveComponent<ComponentA>();
            Assert.AreEqual(2, archetype.EntitiesCount);
            Assert.AreEqual(entity0, archetype[0]);
            Assert.AreEqual(entity3, archetype[1]);
            
            entity0.RemoveComponent<ComponentA>();
            Assert.AreEqual(1, archetype.EntitiesCount);
            Assert.AreEqual(entity3, archetype[0]);

            
            entity1.AddComponent(new ComponentA()); 
            entity2.AddComponent(new ComponentA());
            Assert.AreEqual(3, archetype.EntitiesCount);
            Assert.AreEqual(entity3, archetype[0]);
            Assert.AreEqual(entity1, archetype[1]);
            Assert.AreEqual(entity2, archetype[2]);
        }
    }
}