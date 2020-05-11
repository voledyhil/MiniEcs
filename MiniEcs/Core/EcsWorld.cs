using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    /// <summary>
    /// EcsWorld manages the filtering of all entities in the World.
    /// EcsWorld maintains a list of archetypes and organizes object-related data
    /// for optimal performance.
    /// </summary>
    public class EcsWorld
    {
        /// <summary>
        /// Number of existing archetypes
        /// </summary>
        public int ArchetypeCount => _archetypeManager.ArchetypeCount;
        /// <summary>
        /// Number Entities In Processing
        /// </summary>
        public int EntitiesInProcessing => _entitiesPool.Count;
        /// <summary>
        /// Entity unique identifier generator
        /// </summary>
        private uint _entityCounter;
        /// <summary>
        /// Number of all possible component types
        /// </summary>
        private readonly int _capacity;
        /// <summary>
        /// Archetype Manager
        /// </summary>
        private readonly EcsArchetypeManager _archetypeManager;
        /// <summary>
        /// Stores a group of archetypes for the specified filter.
        /// </summary>
        private readonly Dictionary<EcsFilter, EcsGroup> _groups = new Dictionary<EcsFilter, EcsGroup>();
        /// <summary>
        /// Pool of entities sent for processing
        /// </summary>
        private readonly Queue<EcsEntityExtended> _entitiesPool = new Queue<EcsEntityExtended>();
        /// <summary>
        /// Create new EcsWorld
        /// </summary>
        /// <param name="capacity">Number of all possible component types</param>
        public EcsWorld(byte capacity)
        {
            _capacity = capacity;
            _archetypeManager = new EcsArchetypeManager(capacity);
        }

        /// <summary>
        /// Creates a new entity, with an initial set of components
        /// </summary>
        /// <param name="components">Initial set of components</param>
        /// <returns>New Entity</returns>
        public EcsEntity CreateEntity(params IEcsComponent[] components)
        {
            if (_entitiesPool.Count <= 0)
                return new EcsEntityExtended(_entityCounter++, _entitiesPool, _archetypeManager, _capacity, components);
            
            EcsEntityExtended entity = _entitiesPool.Dequeue();
            entity.Initialize(_entityCounter++, components);
            return entity;
        }

        /// <summary>
        /// Get (or create if necessary) a singleton component
        /// </summary>
        /// <param name="index">Component Type</param>
        /// <typeparam name="T">Component Type</typeparam>
        /// <returns>singleton component</returns>
        public T GetOrCreateSingleton<T>(byte index) where T : class, IEcsComponent, new()
        {
            EcsArchetype archetype = _archetypeManager.FindOrCreateArchetype(index);
            foreach (EcsEntity entity in archetype)
            {
                return (T) entity[index];
            }

            T component = new T();
            CreateEntity(component);
            return component;
        }

        /// <summary>
        /// Get a collection of archetypes for the specified filter.
        /// Each request caches the resulting set of archetypes for future use.
        /// As new archetypes are added to the world, the group of archetypes is updated.
        /// </summary>
        /// <param name="filter">
        /// A query defines a set of types of components that
        /// an archetype should include
        /// </param>
        /// <returns>Archetypes group</returns>
        public IEcsGroup Filter(EcsFilter filter)
        {
            int version = _archetypeManager.ArchetypeCount - 1;
            if (_groups.TryGetValue(filter, out EcsGroup group))
            {
                if (group.Version >= version) 
                    return group;
            }
            
            byte[] all = filter.All?.ToArray();
            byte[] any = filter.Any?.ToArray();
            byte[] none = filter.None?.ToArray();

            if (group != null)
            {
                group.Update(version, GetArchetypes(all, any, none, group.Version));
                return group;
            }

            group = new EcsGroup(version, GetArchetypes(all, any, none, 0));
            _groups.Add(filter.Clone(), group);
            return group;
        }

        /// <summary>
        /// Retrieves all archetypes that match the search criteria.
        /// </summary>
        /// <param name="all">All component types in this array must exist in the archetype</param>
        /// <param name="any">At least one of the component types in this array must exist in the archetype</param>
        /// <param name="none">None of the component types in this array can exist in the archetype</param>
        /// <param name="startId">Archetype start id</param>
        /// <returns>Archetype enumerator</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<EcsArchetype> GetArchetypes(byte[] all, byte[] any, byte[] none, int startId)
        {
            HashSet<EcsArchetype> buffer0 = null;
            HashSet<EcsArchetype> buffer1 = null;

            if (all != null || any != null)
            {
                if (all != null)
                {
                    IReadOnlyList<EcsArchetype>[] archetypes = new IReadOnlyList<EcsArchetype>[all.Length];
                    for (int i = 0; i < all.Length; i++)
                    {
                        archetypes[i] = _archetypeManager.GetArchetypes(all[i], startId).ToArray();
                    }

                    Array.Sort(archetypes, (a, b) => a.Count - b.Count);

                    buffer0 = new HashSet<EcsArchetype>(archetypes[0]);
                    for (int i = 1; i < all.Length; i++)
                    {
                        buffer0.IntersectWith(archetypes[i]);
                    }
                }

                if (any != null)
                {
                    buffer1 = new HashSet<EcsArchetype>(_archetypeManager.GetArchetypes(any[0], startId));
                    for (int i = 1; i < any.Length; i++)
                    {
                        buffer1.UnionWith(_archetypeManager.GetArchetypes(any[i], startId));
                    }
                }

                if (buffer0 != null && buffer1 != null)
                {
                    buffer0.IntersectWith(buffer1);
                }
                else if (buffer1 != null)
                {
                    buffer0 = buffer1;
                }
            }
            else
            {
                buffer0 = new HashSet<EcsArchetype>(_archetypeManager.GetArchetypes(startId));
            }

            if (none != null)
            {
                foreach (byte type in none)
                {
                    buffer0.ExceptWith(_archetypeManager.GetArchetypes(type, startId));
                }
            }

            return buffer0;
        }


        private class EcsEntityExtended : EcsEntity
        {
            private readonly Queue<EcsEntityExtended> _entitiesPool;

            public EcsEntityExtended(uint id, Queue<EcsEntityExtended> entitiesPool,
                EcsArchetypeManager archetypeManager, int capacity, params IEcsComponent[] components) : base(id, archetypeManager, capacity, components)
            {
                _entitiesPool = entitiesPool;
            }
            
            public void Initialize(uint id, params IEcsComponent[] components)
            {
                InnerInitialize(id, components);
            }

            protected override void OnDestroy()
            {
                _entitiesPool.Enqueue(this);
            }
        }
    }
}