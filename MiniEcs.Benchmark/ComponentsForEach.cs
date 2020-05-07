using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Entitas;
using MiniEcs.Core;
using MiniEcs.Core.Systems;
using EntitasEntity = Entitas.Entity;
using EntitasWorld = Entitas.IContext<Entitas.Entity>;

namespace MiniEcs.Benchmark
{
    [MemoryDiagnoser]
    public class ComponentsForEach
    {
        private EntitasWorld _entitasWorld;
        public class EntitasComponentA : IComponent
        {
        }

        public class EntitasComponentB : IComponent
        {
        }

        public class EntitasComponentC : IComponent
        {
        }

        public class EntitasComponentD : IComponent
        {
        }
        

        private EcsWorld _world;
        public class ComponentA : IEcsComponent
        {
            public byte Index => 0;
        }

        public class ComponentB : IEcsComponent
        {
            public byte Index => 1;
        }

        public class ComponentC : IEcsComponent
        {
            public byte Index => 2;
        }

        public class ComponentD : IEcsComponent
        {
            public byte Index => 3;
        }
        

        [Params(0, 1000, 10000)] public int Iterations;

        [GlobalSetup]
        public void Setup()
        {
            _entitasWorld = new Context<EntitasEntity>(4, () => new EntitasEntity());
            _world = new EcsWorld(4);
            for (int i = 0; i < Iterations; ++i)
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

                _world.CreateEntity(new ComponentA(), new ComponentB(), new ComponentD());
                _world.CreateEntity(new ComponentA(), new ComponentC());
                _world.CreateEntity(new ComponentB(), new ComponentD());
                _world.CreateEntity(new ComponentB(), new ComponentD());
                _world.CreateEntity(new ComponentB(), new ComponentC());
                _world.CreateEntity(new ComponentA(), new ComponentB());
                _world.CreateEntity(new ComponentA(), new ComponentD());
            }
        }

        [Benchmark]
        public void EntitasForEach()
        {
            List<EntitasEntity> entities = new List<EntitasEntity>();

            for (int i = 0; i < 10; i++)
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

            IGroup<Entity> group = _entitasWorld.GetGroup(Matcher<EntitasEntity>.AllOf(1).AnyOf(0, 2).NoneOf(3));
            foreach (Entity entity in group)
            {
                EntitasComponentB comp = (EntitasComponentB) entity.GetComponent(1);
            }

            foreach (Entity entity in entities)
            {
                entity.Destroy();
            }
        }

        [Benchmark]
        public void MiniEcsForEach()
        {
            List<EcsEntity> entities = new List<EcsEntity>();

            for (int i = 0; i < 10; i++)
            {
                EcsEntity entityABD = _world.CreateEntity(new ComponentA(), new ComponentB(), new ComponentD());
                EcsEntity entityAC = _world.CreateEntity(new ComponentA(), new ComponentC());
                EcsEntity entityBD0 = _world.CreateEntity(new ComponentB(), new ComponentD());
                EcsEntity entityBD1 = _world.CreateEntity(new ComponentB(), new ComponentD());
                EcsEntity entityBC = _world.CreateEntity(new ComponentB(), new ComponentC());
                EcsEntity entityAB = _world.CreateEntity(new ComponentA(), new ComponentB());
                EcsEntity entityAD = _world.CreateEntity(new ComponentA(), new ComponentD());
                
                entities.Add(entityABD);
                entities.Add(entityAC);
                entities.Add(entityBD0);
                entities.Add(entityBD1);
                entities.Add(entityBC);
                entities.Add(entityAB);
                entities.Add(entityAD);
            }

            IEcsGroup group = _world.Filter(new EcsFilter().AllOf(1).AnyOf(0, 2).NoneOf(3));
            foreach (EcsEntity entity in group)
            {
                ComponentB comp = (ComponentB) entity[1];
            }
            
            foreach (EcsEntity entity in entities)
            {
                entity.Destroy();
            }
        }

    }
}