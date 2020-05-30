using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Entitas;
using MiniEcs.Core;
using EcsFilter = MiniEcs.Core.EcsFilter;
using EcsWorld = MiniEcs.Core.EcsWorld;
using EntitasEntity = Entitas.Entity;
using EntitasWorld = Entitas.IContext<Entitas.Entity>;

namespace MiniEcs.Benchmark
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class ComplexTest
    {        
        private class EntitasComponentA : IComponent
        {
            public int Value;
        }

        private class EntitasComponentB : IComponent
        {
        }

        private class EntitasComponentC : IComponent
        {
            public int Value;
        }

        private class EntitasComponentD : IComponent
        {
        }
        
        

        private class MiniEcsComponentA : IEcsComponent
        {
            public int Value;
        }

        private class MiniEcsComponentB : IEcsComponent
        {
        }

        private class MiniEcsComponentC : IEcsComponent
        {
            public int Value;
        }

        private class MiniEcsComponentD : IEcsComponent
        {
        }
        
        private EntitasWorld _entitasWorld;
        private EcsWorld _world;

        
        [GlobalSetup]
        public void Setup()
        {
            EcsComponentType<MiniEcsComponentA>.Register();
            EcsComponentType<MiniEcsComponentB>.Register();
            EcsComponentType<MiniEcsComponentC>.Register();
            EcsComponentType<MiniEcsComponentD>.Register();

            _world = new EcsWorld();
            
            _entitasWorld = new Context<EntitasEntity>(4, () => new EntitasEntity());

            for (int i = 0; i < 15000; ++i)
            {
                EntitasEntity entity = _entitasWorld.CreateEntity();
                entity.AddComponent(0, new EntitasComponentA());
                entity.AddComponent(1, new EntitasComponentB());
                entity.AddComponent(3, new EntitasComponentD());

                entity = _entitasWorld.CreateEntity();
                entity.AddComponent(0, new EntitasComponentA());
                entity.AddComponent(2, new EntitasComponentC());

                entity = _entitasWorld.CreateEntity();
                entity.AddComponent(1, new EntitasComponentB());
                entity.AddComponent(3, new EntitasComponentD());

                entity = _entitasWorld.CreateEntity();
                entity.AddComponent(1, new EntitasComponentB());
                entity.AddComponent(3, new EntitasComponentD());

                entity = _entitasWorld.CreateEntity();
                entity.AddComponent(1, new EntitasComponentB());
                entity.AddComponent(2, new EntitasComponentC());

                entity = _entitasWorld.CreateEntity();
                entity.AddComponent(0, new EntitasComponentA());
                entity.AddComponent(1, new EntitasComponentB());

                entity = _entitasWorld.CreateEntity();
                entity.AddComponent(0, new EntitasComponentA());
                entity.AddComponent(3, new EntitasComponentD());

                _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentB(), new MiniEcsComponentD());
                _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentC());
                _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentD());
                _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentD());
                _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentC());
                _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentB());
                _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentD());
            }
        }

        [Benchmark]
        public void EntitasStressTest()
        {
            List<EntitasEntity> entities = new List<EntitasEntity>();

            for (int i = 0; i < 1000; i++)
            {
                EntitasEntity entityABD = _entitasWorld.CreateEntity();
                entityABD.AddComponent(0, new EntitasComponentA());
                entityABD.AddComponent(1, new EntitasComponentB());
                entityABD.AddComponent(3, new EntitasComponentD());

                EntitasEntity entityAC = _entitasWorld.CreateEntity();
                entityAC.AddComponent(0, new EntitasComponentA());
                entityAC.AddComponent(2, new EntitasComponentC());

                EntitasEntity entityBD0 = _entitasWorld.CreateEntity();
                entityBD0.AddComponent(1, new EntitasComponentB());
                entityBD0.AddComponent(3, new EntitasComponentD());

                EntitasEntity entityBD1 = _entitasWorld.CreateEntity();
                entityBD1.AddComponent(1, new EntitasComponentB());
                entityBD1.AddComponent(3, new EntitasComponentD());

                EntitasEntity entityBC = _entitasWorld.CreateEntity();
                entityBC.AddComponent(1, new EntitasComponentB());
                entityBC.AddComponent(2, new EntitasComponentC());

                EntitasEntity entityAB = _entitasWorld.CreateEntity();
                entityAB.AddComponent(0, new EntitasComponentA());
                entityAB.AddComponent(1, new EntitasComponentB());

                EntitasEntity entityAD = _entitasWorld.CreateEntity();
                entityAD.AddComponent(0, new EntitasComponentA());
                entityAD.AddComponent(3, new EntitasComponentD());

                entities.Add(entityABD);
                entities.Add(entityAC);
                entities.Add(entityBD0);
                entities.Add(entityBD1);
                entities.Add(entityBC);
                entities.Add(entityAB);
                entities.Add(entityAD);
            }

            EntitasForEachOneComp();
            EntitasForEachTwoComp();

            foreach (Entity entity in entities)
            {
                entity.Destroy();
            }
        }

        [Benchmark]
        public void MiniEcsStressTest()
        {
            List<IEcsEntity> entities = new List<IEcsEntity>();

            for (int i = 0; i < 1000; i++)
            {
                IEcsEntity entityABD = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentB(),
                    new MiniEcsComponentD());
                IEcsEntity entityAC = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentC());
                IEcsEntity entityBD0 = _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentD());
                IEcsEntity entityBD1 = _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentD());
                IEcsEntity entityBC = _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentC());
                IEcsEntity entityAB = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentB());
                IEcsEntity entityAD = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentD());

                entities.Add(entityABD);
                entities.Add(entityAC);
                entities.Add(entityBD0);
                entities.Add(entityBD1);
                entities.Add(entityBC);
                entities.Add(entityAB);
                entities.Add(entityAD);
            }

            MiniEcsForEachOneComp();
            MiniEcsForEachTwoComp();
            
            foreach (IEcsEntity entity in entities)
            {
                entity.Destroy();
            }
        }

        
        [Benchmark]
        public void EntitasForEachOneComp()
        {
            foreach (Entity entity in _entitasWorld.GetGroup(Matcher<EntitasEntity>.AllOf(0).NoneOf(1, 3)))
            {
                EntitasComponentA comp = (EntitasComponentA) entity.GetComponent(0);
                comp.Value++;
            }
        }
        
        [Benchmark]
        public void EntitasForEachTwoComp()
        {
            foreach (Entity entity in _entitasWorld.GetGroup(Matcher<EntitasEntity>.AllOf(0, 2).NoneOf(1, 3)))
            {
                EntitasComponentA compA = (EntitasComponentA) entity.GetComponent(0);
                EntitasComponentC compC = (EntitasComponentC) entity.GetComponent(2);
                compA.Value++;
                compC.Value++;
            }
        }
        

        [Benchmark]
        public void MiniEcsForEachOneComp()
        {
            _world.Filter(new EcsFilter().AllOf<MiniEcsComponentA>().NoneOf<MiniEcsComponentB, MiniEcsComponentD>())
                .ForEach((IEcsEntity entity, MiniEcsComponentA compA) =>
                {
                    compA.Value++;
                });
        }
        
        [Benchmark]
        public void MiniEcsForEachTwoComp()
        {
            _world.Filter(new EcsFilter().AllOf<MiniEcsComponentA, MiniEcsComponentC>().NoneOf<MiniEcsComponentB, MiniEcsComponentD>())
                .ForEach((IEcsEntity entity, MiniEcsComponentA compA, MiniEcsComponentC compC) =>
                {
                    compA.Value++;
                    compC.Value++;
                });
        }

    }
}