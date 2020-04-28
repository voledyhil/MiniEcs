using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsWorld
    {
        public const ushort MAX_ENTITY_COUNT = ushort.MaxValue;
        public const ushort MAX_ARCHETYPE_COUNT = byte.MaxValue;
        public const ushort MAX_COMPONENT_TYPE_COUNT = byte.MaxValue;
        public const ushort MAX_COMPONENT_FILTER_COUNT = 10;
        
        private ushort _typeCounter;
        private ushort _archetypeCounter;
        private readonly EcsArchetype _rootArchetype;
        private readonly Map<Type, ushort> _ecsTypes = new Map<Type, ushort>();

        private readonly SparseSetValues<IEcsComponentFromEntity> _components = new SparseSetValues<IEcsComponentFromEntity>(MAX_COMPONENT_TYPE_COUNT);
        private readonly SparseSetValues<ushort> _entityArchetypes = new SparseSetValues<ushort>(MAX_ENTITY_COUNT);    
        private readonly SparseSetValues<SparseSet> _componentArchetypes = new SparseSetValues<SparseSet>(MAX_COMPONENT_TYPE_COUNT);
        private readonly SparseSetValues<EcsArchetype> _archetypes = new SparseSetValues<EcsArchetype>(MAX_ARCHETYPE_COUNT);
        private readonly SparseSet _archetypeBuffer0 = new SparseSet(MAX_ARCHETYPE_COUNT);
        private readonly SparseSet _archetypeBuffer1 = new SparseSet(MAX_ARCHETYPE_COUNT);
        private readonly SparseSet _archetypeBuffer2 = new SparseSet(MAX_ARCHETYPE_COUNT);
        private readonly SparseSet _typeBuffer = new SparseSet(MAX_COMPONENT_TYPE_COUNT);        
        private readonly EcsEntityIdentifiers _identifiers = new EcsEntityIdentifiers();

        public EcsWorld()
        {
            _rootArchetype = new EcsArchetype(_archetypeCounter++);
            _archetypes[_rootArchetype.Id] = _rootArchetype;
        }

        public void RegisterComponent<T>() where T : class, IEcsComponent
        {
            _ecsTypes.Add(typeof(T), _typeCounter);
            _components[_typeCounter] = new EcsComponentFromEntity<T>(MAX_ENTITY_COUNT);
            _componentArchetypes[_typeCounter++] = new SparseSet(MAX_ARCHETYPE_COUNT);
        }

        public uint CreateEntity(params IEcsComponent[] components)
        {
            _typeBuffer.Clear();

            uint entity = _identifiers.Next();

            foreach (IEcsComponent component in components)
            {
                ushort type = _ecsTypes.Forward[component.GetType()];
                _components[type].Set(entity, component);
                _typeBuffer.Set(type);
            }

            ushort entityId = EcsEntityIdentifiers.GetId(entity);

            EcsArchetype archetype = FindOrCreateArchetype(_typeBuffer);
            archetype.Entities[entityId] = entity;
            
            _entityArchetypes[entityId] = archetype.Id;

            return entity;
        }

        private EcsArchetype FindOrCreateArchetype(SparseSet types)
        {
            types.Sort();

            EcsArchetype curArchetype = _rootArchetype;
            foreach (ushort type in types)
            {
                if (!curArchetype.Next.TryGetValue(type, out EcsArchetype nextArchetype))
                {
                    nextArchetype = new EcsArchetype(_archetypeCounter++);
                    nextArchetype.Types.Set(curArchetype.Types);
                    nextArchetype.Types.Set(type);
                    nextArchetype.Prior[type] = curArchetype;

                    foreach (ushort componentType in nextArchetype.Types)
                    {
                        _componentArchetypes[componentType].Set(nextArchetype.Id);
                    }
                   
                    _archetypes[nextArchetype.Id] = nextArchetype;

                    curArchetype.Next[type] = nextArchetype;
                }

                curArchetype = nextArchetype;
            }

            return curArchetype;
        }

        public void DestroyEntity(uint entity)
        {
            ushort entityId = EcsEntityIdentifiers.GetId(entity);
            
            EcsArchetype archetype = _archetypes[_entityArchetypes[entityId]];

            foreach (ushort type in archetype.Types)
            {
                _components[type].Remove(entity);
            }

            archetype.Entities.Remove(entityId);

            _entityArchetypes.Remove(entityId);
            _identifiers.Recycle(entity);
        }

        public IEcsArchetype GetArchetype(params Type[] types)
        {
            _typeBuffer.Clear();

            foreach (Type type in types)
            {
                _typeBuffer.Set(_ecsTypes.Forward[type]);
            }

            return FindOrCreateArchetype(_typeBuffer);
        }

        public IEcsComponentFromEntity<T> GetComponentFromEntity<T>() where T : class, IEcsComponent
        {
            return (EcsComponentFromEntity<T>) _components[_ecsTypes.Forward[typeof(T)]];
        }

        public T GetComponent<T>(uint entity) where T : class, IEcsComponent
        {
            return GetComponentFromEntity<T>()[entity];
        }

        public bool HasComponent<T>(uint entity) where T : class, IEcsComponent
        {
            return _components[_ecsTypes.Forward[typeof(T)]].Contain(entity);
        }

        public T SetComponent<T>(uint entity, T component) where T : class, IEcsComponent
        {
            ushort type = _ecsTypes.Forward[typeof(T)];
            ushort entityId = EcsEntityIdentifiers.GetId(entity);  
            
            EcsArchetype curArchetype = _archetypes[_entityArchetypes[entityId]];

            _typeBuffer.Clear();
            _typeBuffer.Set(curArchetype.Types);
            _typeBuffer.Set(type);

            if (_typeBuffer.Length > curArchetype.Types.Length)
            {
                curArchetype.Entities.Remove(entityId);
                if (!curArchetype.Next.TryGetValue(type, out EcsArchetype nextArchetype))
                {
                    nextArchetype = FindOrCreateArchetype(_typeBuffer);
                    curArchetype.Next[type] = nextArchetype;
                }

                nextArchetype.Entities[entityId] = entity;
                _entityArchetypes[entityId] = nextArchetype.Id;
            }

            ((EcsComponentFromEntity<T>) _components[type]).Set(entity, component);

            return component;
        }

        public void RemoveComponent<T>(uint entity) where T : class, IEcsComponent
        {
            ushort type = _ecsTypes.Forward[typeof(T)];
            ushort entityId = EcsEntityIdentifiers.GetId(entity);
           
            EcsArchetype curArchetype = _archetypes[_entityArchetypes[entityId]];

            _typeBuffer.Clear();
            _typeBuffer.Set(curArchetype.Types);
            _typeBuffer.UnSet(type);

            if (_typeBuffer.Length == curArchetype.Types.Length)
                throw new InvalidOperationException($"component by type '{typeof(T)}' does not exists");

            curArchetype.Entities.Remove(entityId);
            if (!curArchetype.Prior.TryGetValue(type, out EcsArchetype priorArchetype))
            {
                priorArchetype = FindOrCreateArchetype(_typeBuffer);
                curArchetype.Prior[type] = priorArchetype;
            }

            priorArchetype.Entities[entityId] = entity;

            _entityArchetypes[entityId] = priorArchetype.Id;

            ((EcsComponentFromEntity<T>) _components[type]).Remove(entity);
        }  
        
        public List<uint> FilterEntities(EcsFilter filter)
        {
            SparseSet buffer0 = _archetypeBuffer0;
            SparseSet buffer1 = _archetypeBuffer1;
            SparseSet buffer2 = _archetypeBuffer2;

            buffer0.Clear();
            buffer1.Clear();
            buffer2.Clear();

            filter.Prepare(_ecsTypes, out SparseSet all, out SparseSet any, out SparseSet none);

            if (all?.Length > 0 || any?.Length > 0)
            {
                if (all?.Length > 0)
                {
                    if (all.Length > 1)
                    {
                        int length = 0;
                        SparseSet[] archetypesInfo = new SparseSet[all.Length];
                        foreach (ushort type in all)
                        {
                            archetypesInfo[length++] = _componentArchetypes[type];
                        }

                        Intersect(buffer0, archetypesInfo);
                    }
                    else
                    {
                        buffer0.Set(_componentArchetypes[all[0]]);
                    }
                }

                if (any?.Length > 0)
                {
                    foreach (ushort type in any)
                    {
                        buffer1.Set(_componentArchetypes[type]);
                    }
                }

                if (all?.Length > 0 && any?.Length > 0)
                {
                    Intersect(buffer2, buffer0, buffer1);
                }
                else if (all?.Length > 0)
                {
                    buffer2 = buffer0;
                }
                else
                {
                    buffer2 = buffer1;
                }
            }
            else
            {
                foreach (EcsArchetype archetype in _archetypes)
                {
                    buffer2.Set(archetype.Id);
                }
            }

            if (none?.Length > 0)
            {
                foreach (ushort type in none)
                {
                    buffer2.UnSet(_componentArchetypes[type]);
                }
            }

            List<uint> entities = new List<uint>();
            foreach (ushort archetype in buffer2)
            {
                entities.AddRange(_archetypes[archetype]);
            }
            return entities;
        }

        private static void Intersect(SparseSet result, params SparseSet[] input)
        {
            Array.Sort(input, (a, b) => a.Length - b.Length);

            result.Set(input[0]);

            ushort[] toRemove = new ushort[result.Length];
            
            for (int i = 1; i < input.Length; i++)
            {
                int length = 0;
                SparseSet archSet = input[i];
                foreach (ushort archetype in result)
                {
                    if (archSet.Contain(archetype))
                        continue;

                    toRemove[length++] = archetype;
                }

                for (int j = 0; j < length; j++)
                {
                    result.UnSet(toRemove[j]);
                }
            }
        }
    }
}