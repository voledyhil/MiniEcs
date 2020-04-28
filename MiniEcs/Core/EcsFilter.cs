using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsFilter
    {        
        private HashSet<Type> _anyType;
        private HashSet<Type> _allType;
        private HashSet<Type> _noneType;

        private SparseSet _all;
        private SparseSet _any;
        private SparseSet _none;

        private bool _prepared;

        public EcsFilter AnyOf(params Type[] types)
        {
            _anyType = _anyType ?? new HashSet<Type>();
            foreach (Type type in types)
            {
                if (_anyType.Add(type))
                    _prepared = false;
            }
            return this;
        }

        public EcsFilter AllOf(params Type[] types)
        {
            _allType = _allType ?? new HashSet<Type>();
            foreach (Type type in types)
            {
                if (_allType.Add(type))
                    _prepared = false;
            }
            return this;
        }

        public EcsFilter NoneOf(params Type[] types)
        {
            _noneType = _noneType ?? new HashSet<Type>();
            foreach (Type type in types)
            {
                if (_noneType.Add(type))
                    _prepared = false;
            }
            return this;
        }
        
        
        public EcsFilter AllOf<T>() where T : IEcsComponent
        {
            return AllOf(typeof(T));
        }

        public EcsFilter AllOf<T0, T1>() where T0 : IEcsComponent where T1 : IEcsComponent
        {
            return AllOf(typeof(T0), typeof(T1));
        }

        public EcsFilter AllOf<T0, T1, T2>() where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
        {
            return AllOf(typeof(T0), typeof(T1), typeof(T2));
        }

        
        public EcsFilter AnyOf<T>() where T : IEcsComponent
        {
            return AnyOf(typeof(T));
        }

        public EcsFilter AnyOf<T0, T1>() where T0 : IEcsComponent where T1 : IEcsComponent
        {
            return AnyOf(typeof(T0), typeof(T1));
        }

        public EcsFilter AnyOf<T0, T1, T2>() where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
        {
            return AnyOf(typeof(T0), typeof(T1), typeof(T2));
        }        


        public EcsFilter NoneOf<T>() where T : IEcsComponent
        {
            return NoneOf(typeof(T));
        }

        public EcsFilter NoneOf<T0, T1>() where T0 : IEcsComponent where T1 : IEcsComponent
        {
            return NoneOf(typeof(T0), typeof(T1));
        }

        public EcsFilter NoneOf<T0, T1, T2>() where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
        {
            return NoneOf(typeof(T0), typeof(T1), typeof(T2));
        }        
        

        public void Prepare(Map<Type, ushort> types, out SparseSet all, out SparseSet any, out SparseSet none)
        {
            all = _all;
            any = _any;
            none = _none;

            if (_prepared)
                return;

            if (_allType != null)
            {
                _all = _all ?? new SparseSet(EcsWorld.MAX_COMPONENT_FILTER_COUNT);
                foreach (Type type in _allType)
                {
                    _all.Set(types.Forward[type]);
                }
            }

            if (_anyType != null)
            {
                _any = _any ?? new SparseSet(EcsWorld.MAX_COMPONENT_FILTER_COUNT);
                foreach (Type type in _anyType)
                {
                    _any.Set(types.Forward[type]);
                }
            }

            if (_noneType != null)
            {
                _none = _none ?? new SparseSet(EcsWorld.MAX_COMPONENT_FILTER_COUNT);
                foreach (Type type in _noneType)
                {
                    _none.Set(types.Forward[type]);
                }
            }

            all = _all;
            any = _any;
            none = _none;

            _prepared = true;
        }
    }
}