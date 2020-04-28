using System;

namespace MiniEcs.Core
{
    public interface IEcsComponentFromEntity
    {
        void Set(uint entity, IEcsComponent component);
        void Remove(uint entity);
        bool Contain(uint entity); 
    }
    
    public interface IEcsComponentFromEntity<out T>
    {
        int Count { get; }
        T this[uint entity] { get; }
        bool Contain(uint entity);
    }
    
    public class EcsComponentFromEntity<T> : IEcsComponentFromEntity<T>, IEcsComponentFromEntity where T : class, IEcsComponent
    {
        public int Count => _components.Count;
        
        private readonly SparseSetValues<T> _components;
        
        public EcsComponentFromEntity(ushort capacity)
        {
            _components = new SparseSetValues<T>(capacity);
        }
        
        public T this[uint entity] => _components[EcsEntityIdentifiers.GetId(entity)];

        public void Set(uint entity, IEcsComponent component)
        {
            if (!(component is T value))
                throw new InvalidCastException();

            _components[EcsEntityIdentifiers.GetId(entity)] = value;
        }
        
        public void Set(uint entity, T component)
        {
            _components[EcsEntityIdentifiers.GetId(entity)] = component;
        }

        public void Remove(uint entity)
        {
            _components.Remove(EcsEntityIdentifiers.GetId(entity));
        }

        public bool Contain(uint entity)
        {
            return _components.Contain(EcsEntityIdentifiers.GetId(entity));
        }
    }
}