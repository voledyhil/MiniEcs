using System;

namespace MiniEcs.Core
{
    public sealed class EcsComponentTypeManager
    {
        public static readonly IEcsComponentPoolCreator[] ComponentPoolCreators = new IEcsComponentPoolCreator[byte.MaxValue];
        
        internal static int Count;
        internal static void InstantiateComponentPoolCreator<T>() where T : IEcsComponent
        {
            ComponentPoolCreators[Count] = new EcsComponentPoolCreator<T>();
            Count++;
        }
    }

    public static class EcsComponentType<T> where T : IEcsComponent
    {
        public static readonly byte Index;

        static EcsComponentType()
        {
            Index = (byte) EcsComponentTypeManager.Count;
            EcsComponentTypeManager.InstantiateComponentPoolCreator<T>();
        }
    }

    
    public interface IEcsComponentPoolCreator
    {
        IEcsComponentPool InstantiatePool();
    }
    
    public interface IEcsComponentPool
    {
        void AddComponent(int index, IEcsComponent component);
        void Replace(int freeIndex, int current);
        void Remove(int index);
        IEcsComponent Get(int index);
    }
    
    
    public class EcsComponentPoolCreator<T> : IEcsComponentPoolCreator where T : IEcsComponent
    {
        public IEcsComponentPool InstantiatePool()
        {
            return new EcsComponentPool<T>();
        }
    }

    public class EcsComponentPool<T> : IEcsComponentPool where T : IEcsComponent
    {
        private T[] _components = new T[1];
        public void AddComponent(int index, IEcsComponent component)
        {
            if (index >= _components.Length)
                Array.Resize(ref _components, 2 * _components.Length);

            _components[index] = (T) component;
        }

        public void Replace(int freeIndex, int current)
        {
            T component = _components[current];
            _components[freeIndex] = component;
            _components[current] = default;
        }

        public void Remove(int index)
        {
            _components[index] = default;
        }

        public IEcsComponent Get(int index)
        {
            return _components[index];
        }

        public T GetTyped(int index)
        {
            return _components[index];
        }
    }
}