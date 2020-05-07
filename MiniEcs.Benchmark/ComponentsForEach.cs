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
        

        [Params(1000)] public int Iterations;

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
            EntitasEntity ent = _entitasWorld.CreateEntity();
            ent.AddComponent(0, new EntitasComponentA());
            ent.AddComponent(1, new EntitasComponentB());
            
            IGroup<Entity> group = _entitasWorld.GetGroup(Matcher<EntitasEntity>.AllOf(1).AnyOf(0, 2).NoneOf(3));
            foreach (Entity entity in group)
            {
                EntitasComponentB comp = (EntitasComponentB) entity.GetComponent(1);
            }
        }

        [Benchmark]
        public void MiniEcsForEach()
        {
            _world.CreateEntity(new ComponentA(), new ComponentB());
            
            IEcsGroup group = _world.Filter(new EcsFilter().AllOf(1).AnyOf(0, 2).NoneOf(3));
            foreach (EcsEntity entity in group)
            {
                ComponentB comp = (ComponentB) entity[1];
            }
        }

    }
}