using System;

namespace MiniEcs.Core
{
    public interface IEcsComponentPoolCreator
    {
        IEcsComponentPool InstantiatePool();
    }

    public interface IEcsComponentPool
    {
        void AddComponent(int index, IEcsComponent comp);
        void Replace(int freeIndex, int current);
        void Remove(int index);
        IEcsComponent Get(int index);
    }


    public class EcsComponentPoolCreator<TC> : IEcsComponentPoolCreator where TC : IEcsComponent
    {
        public IEcsComponentPool InstantiatePool()
        {
            return new EcsComponentPool<TC>();
        }
    }

    public class EcsComponentPool<TC> : IEcsComponentPool where TC : IEcsComponent
    {
        private TC[] _comps = new TC[1];

        public void AddComponent(int index, IEcsComponent comp)
        {
            if (index >= _comps.Length)
                Array.Resize(ref _comps, 2 * _comps.Length);

            _comps[index] = (TC) comp;
        }

        public void Replace(int freeIndex, int current)
        {
            TC comp = _comps[current];
            _comps[freeIndex] = comp;
            _comps[current] = default;
        }

        public void Remove(int index)
        {
            _comps[index] = default;
        }

        public IEcsComponent Get(int index)
        {
            return _comps[index];
        }

        public TC GetTyped(int index)
        {
            return _comps[index];
        }
    }
}