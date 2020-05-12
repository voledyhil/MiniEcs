using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Entitas;
using MiniEcs.Core;
using EcsEntity = MiniEcs.Core.EcsEntity;
using EcsFilter = MiniEcs.Core.EcsFilter;
using EcsWorld = MiniEcs.Core.EcsWorld;
using EntitasEntity = Entitas.Entity;
using EntitasWorld = Entitas.IContext<Entitas.Entity>;

namespace MiniEcs.Benchmark
{
    [MemoryDiagnoser]
    public class ComplexTest
    {
        private class EntitasComponentA : IComponent
        {
        }

        private class EntitasComponentB : IComponent
        {
        }

        private class EntitasComponentC : IComponent
        {
        }

        private class EntitasComponentD : IComponent
        {
        }
        
        

        private class MiniEcsComponentA : IEcsComponent
        {
        }

        private class MiniEcsComponentB : IEcsComponent
        {
        }

        private class MiniEcsComponentC : IEcsComponent
        {
        }

        private class MiniEcsComponentD : IEcsComponent
        {
        }
        
        private EntitasWorld _entitasWorld;
        private EcsWorld _world;
        
        [GlobalSetup]
        public void Setup()
        {
            _entitasWorld = new Context<EntitasEntity>(4, () => new EntitasEntity());
            _world = new EcsWorld();
            
            for (int i = 0; i < 1000; ++i)
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

            for (int i = 0; i < 100; i++)
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

            foreach (Entity entity in _entitasWorld.GetGroup(Matcher<EntitasEntity>.AllOf(0).NoneOf(1, 3)))
            {
                EntitasComponentA comp = (EntitasComponentA) entity.GetComponent(0);
            }

            foreach (Entity entity in entities)
            {
                entity.Destroy();
            }
        }

        [Benchmark]
        public void MiniEcsStressTest()
        {
            List<EcsEntity> entities = new List<EcsEntity>();

            for (int i = 0; i < 100; i++)
            {
                EcsEntity entityABD = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentB(), new MiniEcsComponentD());
                EcsEntity entityAC = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentC());
                EcsEntity entityBD0 = _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentD());
                EcsEntity entityBD1 = _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentD());
                EcsEntity entityBC = _world.CreateEntity(new MiniEcsComponentB(), new MiniEcsComponentC());
                EcsEntity entityAB = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentB());
                EcsEntity entityAD = _world.CreateEntity(new MiniEcsComponentA(), new MiniEcsComponentD());

                entities.Add(entityABD);
                entities.Add(entityAC);
                entities.Add(entityBD0);
                entities.Add(entityBD1);
                entities.Add(entityBC);
                entities.Add(entityAB);
                entities.Add(entityAD);
            }

            foreach (EcsEntity entity in _world.Filter(new EcsFilter().AllOf<MiniEcsComponentA>().NoneOf<MiniEcsComponentB, MiniEcsComponentD>()))
            {
                MiniEcsComponentA comp = entity.GetComponent<MiniEcsComponentA>();
            }

            foreach (EcsEntity entity in entities)
            {
                entity.Destroy();
            }
        }

    }
}