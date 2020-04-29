using System.Collections.Generic;

namespace MiniEcs.Core
{
    public delegate void ForEachEventHandler(EcsEntity entity);

    public class EcsForEach
    {
        private readonly EcsFilter _filter;
        private readonly EcsWorld _world;

        public EcsForEach(EcsWorld world, EcsFilter filter)
        {
            _world = world;
            _filter = filter;
        }

        public EcsForEach WithAll(params byte[] indices)
        {
            _filter.AllOf(indices);
            return this;
        }

        public EcsForEach WithAny(params byte[] indices)
        {
            _filter.AnyOf(indices);
            return this;
        }

        public EcsForEach WithNone(params byte[] indices)
        {
            _filter.NoneOf(indices);
            return this;
        }

        public void ForEach(ForEachEventHandler action)
        {
            foreach (EcsEntity entity in _world.Filter(_filter))
            {
                action(entity);
            }
        }
        
        public List<EcsEntity> ToList()
        {
            return new List<EcsEntity>(_world.Filter(_filter));
        }
    }
}