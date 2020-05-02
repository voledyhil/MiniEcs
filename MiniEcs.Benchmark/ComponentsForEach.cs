using BenchmarkDotNet.Attributes;
using Entitas;
using MiniEcs.Core;
using EntitasEntity = Entitas.Entity;
using EntitasWorld = Entitas.IContext<Entitas.Entity>;

namespace MiniEcs.Benchmark
{
    [MemoryDiagnoser]
    public class ComponentsForEach
    {
        private const float Time = 1f / 60f;


        private EntitasWorld _entitasWorld;
        private EntitasSystem _entitasSystem;

        private class EntitasSpeed : IComponent
        {
            public float X;
            public float Y;
        }

        private class EntitasPosition : IComponent
        {
            public float X;
            public float Y;
        }

        public class EntitasSystem : JobSystem<EntitasEntity>
        {
            public EntitasSystem(EntitasWorld world) : base(world.GetGroup(Matcher<EntitasEntity>.AllOf(0, 1)), 1)
            {
            }

            protected override void Execute(EntitasEntity entity)
            {
                EntitasSpeed speed = (EntitasSpeed) entity.GetComponent(0);
                EntitasPosition position = (EntitasPosition) entity.GetComponent(1);
                position.X += speed.X * Time;
                position.Y += speed.Y * Time;
            }
        }


        private EcsWorld _miniEcsWorld;
        private MiniEcsSystem _miniEcsSystem;

        private static readonly byte MiniEcsSpeedType = 0;
        private static readonly byte MiniEcsPositionType = 1;

        private class MiniEcsSpeed : IEcsComponent
        {
            public float X;
            public float Y;
        }

        private class MiniEcsPosition : IEcsComponent
        {
            public float X;
            public float Y;
        }

        private class MiniEcsSystem : IEcsSystem
        {
            private readonly EcsWorld _world;

            public MiniEcsSystem(EcsWorld world)
            {
                _world = world;
            }

            public void Update(float deltaTime, EcsWorld world)
            {
                _world.WithAll(MiniEcsSpeedType, MiniEcsPositionType).ForEach(entity =>
                {
                    MiniEcsSpeed speed = (MiniEcsSpeed) entity[MiniEcsSpeedType];
                    MiniEcsPosition position = (MiniEcsPosition) entity[MiniEcsPositionType];
                    position.X += speed.X * deltaTime;
                    position.Y += speed.Y * deltaTime;
                });
            }
        }


        [Params(50000)] public int EntityCount;

        [GlobalSetup]
        public void Setup()
        {
            _entitasWorld = new Context<EntitasEntity>(2, () => new EntitasEntity());
            _entitasSystem = new EntitasSystem(_entitasWorld);

            _miniEcsWorld = new EcsWorld(2);
            _miniEcsSystem = new MiniEcsSystem(_miniEcsWorld);

            for (int i = 0; i < EntityCount; ++i)
            {
                EntitasEntity entitasEntity = _entitasWorld.CreateEntity();
                if (i % 2 == 0)
                    entitasEntity.AddComponent(0, new EntitasSpeed {X = 42, Y = 42});
                if (i % 3 == 0)
                    entitasEntity.AddComponent(1, new EntitasPosition());

                EcsEntity entity = _miniEcsWorld.CreateEntity();
                if (i % 2 == 0)
                    entity[MiniEcsSpeedType] = new MiniEcsSpeed {X = 42, Y = 42};
                if (i % 3 == 0)
                    entity[MiniEcsPositionType] = new MiniEcsPosition();
            }
        }

        [Benchmark]
        public void EntitasForEach() => _entitasSystem.Execute();

        [Benchmark]
        public void MiniEcsForEach() => _miniEcsSystem.Update(Time, _miniEcsWorld);

    }
}