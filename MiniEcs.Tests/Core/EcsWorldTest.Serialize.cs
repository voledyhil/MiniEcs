using BinarySerializer.Data;
using BinarySerializer.Serializers.Baselines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    public partial class EcsWorldTest
    {
        [TestMethod]
        public void SerializeUpdateTest()
        {
            byte[] data = _world.Serialize(new EcsFilter());

            EcsWorld target = new EcsWorld();
            target.Update(data);

            IEcsEntity entityAB = target[_entityAB.Id];
            Assert.AreEqual(_entityAB.GetComponent<ComponentA>(), entityAB.GetComponent<ComponentA>());
            Assert.AreEqual(_entityAB.GetComponent<ComponentB>(), entityAB.GetComponent<ComponentB>());

            IEcsEntity entityABD = target[_entityABD.Id];
            Assert.AreEqual(_entityABD.GetComponent<ComponentA>(), entityABD.GetComponent<ComponentA>());
            Assert.AreEqual(_entityABD.GetComponent<ComponentB>(), entityABD.GetComponent<ComponentB>());
            Assert.AreEqual(_entityABD.GetComponent<ComponentD>(), entityABD.GetComponent<ComponentD>());

            IEcsEntity entityAC = target[_entityAC.Id];
            Assert.AreEqual(_entityAC.GetComponent<ComponentA>(), entityAC.GetComponent<ComponentA>());
            Assert.AreEqual(_entityAC.GetComponent<ComponentC>(), entityAC.GetComponent<ComponentC>());

            IEcsEntity entityAD = target[_entityAD.Id];
            Assert.AreEqual(_entityAD.GetComponent<ComponentA>(), entityAD.GetComponent<ComponentA>());
            Assert.AreEqual(_entityAD.GetComponent<ComponentD>(), entityAD.GetComponent<ComponentD>());

            IEcsEntity entityBC = target[_entityBC.Id];
            Assert.AreEqual(_entityBC.GetComponent<ComponentB>(), entityBC.GetComponent<ComponentB>());
            Assert.AreEqual(_entityBC.GetComponent<ComponentC>(), entityBC.GetComponent<ComponentC>());

            IEcsEntity entityBD0 = target[_entityBD0.Id];
            Assert.AreEqual(_entityBD0.GetComponent<ComponentB>(), entityBD0.GetComponent<ComponentB>());
            Assert.AreEqual(_entityBD0.GetComponent<ComponentD>(), entityBD0.GetComponent<ComponentD>());

            IEcsEntity entityBD1 = target[_entityBD1.Id];
            Assert.AreEqual(_entityBD1.GetComponent<ComponentB>(), entityBD1.GetComponent<ComponentB>());
            Assert.AreEqual(_entityBD1.GetComponent<ComponentD>(), entityBD1.GetComponent<ComponentD>());
        }

        [TestMethod]
        public void SerializeUpdateBaselineTest()
        {
            Baseline<uint> baseline = new Baseline<uint>();
            byte[] data = _world.Serialize(new EcsFilter(), baseline);

            EcsWorld source = new EcsWorld();
            source.Update(data);
            

            IEcsEntity entityAB = source[_entityAB.Id];
            Assert.AreEqual(_entityAB.GetComponent<ComponentA>(), entityAB.GetComponent<ComponentA>());
            Assert.AreEqual(_entityAB.GetComponent<ComponentB>(), entityAB.GetComponent<ComponentB>());

            IEcsEntity entityABD = source[_entityABD.Id];
            Assert.AreEqual(_entityABD.GetComponent<ComponentA>(), entityABD.GetComponent<ComponentA>());
            Assert.AreEqual(_entityABD.GetComponent<ComponentB>(), entityABD.GetComponent<ComponentB>());
            Assert.AreEqual(_entityABD.GetComponent<ComponentD>(), entityABD.GetComponent<ComponentD>());

            IEcsEntity entityAC = source[_entityAC.Id];
            Assert.AreEqual(_entityAC.GetComponent<ComponentA>(), entityAC.GetComponent<ComponentA>());
            Assert.AreEqual(_entityAC.GetComponent<ComponentC>(), entityAC.GetComponent<ComponentC>());

            IEcsEntity entityAD = source[_entityAD.Id];
            Assert.AreEqual(_entityAD.GetComponent<ComponentA>(), entityAD.GetComponent<ComponentA>());
            Assert.AreEqual(_entityAD.GetComponent<ComponentD>(), entityAD.GetComponent<ComponentD>());

            IEcsEntity entityBC = source[_entityBC.Id];
            Assert.AreEqual(_entityBC.GetComponent<ComponentB>(), entityBC.GetComponent<ComponentB>());
            Assert.AreEqual(_entityBC.GetComponent<ComponentC>(), entityBC.GetComponent<ComponentC>());

            IEcsEntity entityBD0 = source[_entityBD0.Id];
            Assert.AreEqual(_entityBD0.GetComponent<ComponentB>(), entityBD0.GetComponent<ComponentB>());
            Assert.AreEqual(_entityBD0.GetComponent<ComponentD>(), entityBD0.GetComponent<ComponentD>());

            IEcsEntity entityBD1 = source[_entityBD1.Id];
            Assert.AreEqual(_entityBD1.GetComponent<ComponentB>(), entityBD1.GetComponent<ComponentB>());
            Assert.AreEqual(_entityBD1.GetComponent<ComponentD>(), entityBD1.GetComponent<ComponentD>());


            data = source.Serialize(new EcsFilter(), baseline);
            Assert.AreEqual(0, data.Length);


            EcsWorld target = new EcsWorld();
            // change Component
            entityAB.GetComponent<ComponentA>().Value = int.MaxValue;
            data = source.Serialize(new EcsFilter(), baseline);
            
            target.Update(data);

            Assert.AreEqual(1, target.EntitiesCount);
            Assert.AreEqual(entityAB.GetComponent<ComponentA>(), target[entityAB.Id].GetComponent<ComponentA>());


            //removeComponent
            entityAB.RemoveComponent<ComponentA>();
            data = source.Serialize(new EcsFilter(), baseline);
            
            target.Update(data);

            Assert.AreEqual(1, target.EntitiesCount);
            Assert.IsFalse(target[entityAB.Id].HasComponent<ComponentA>());

            //AddComponent
            entityAB.AddComponent(new ComponentA());
            data = source.Serialize(new EcsFilter(), baseline);
            
            target.Update(data);

            Assert.AreEqual(1, target.EntitiesCount);
            Assert.IsTrue(target[entityAB.Id].HasComponent<ComponentA>());

            //remove Entity
            entityAB.Destroy();
            data = source.Serialize(new EcsFilter(), baseline);
            
            target.Update(data);

            Assert.AreEqual(0, target.EntitiesCount);


            //create Entity
            entityAB = source.CreateEntity(new ComponentA {Value = 100}, new ComponentB {Value = 200});
            data = source.Serialize(new EcsFilter(), baseline);
            
            target.Update(data);

            Assert.AreEqual(1, target.EntitiesCount);
            IEcsEntity targetEntityAB = target[entityAB.Id];
            Assert.AreEqual(2, targetEntityAB.ComponentsCount);

            Assert.AreEqual(entityAB.GetComponent<ComponentA>(), targetEntityAB.GetComponent<ComponentA>());
            Assert.AreEqual(entityAB.GetComponent<ComponentB>(), targetEntityAB.GetComponent<ComponentB>());

        }
    }
}