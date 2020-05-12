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
        public EcsWorld()
        {
            _archetypeManager = new EcsArchetypeManager();
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <returns>New Entity</returns>
        public EcsEntity CreateEntity()
        {
            EcsEntityExtended entity = _entitiesPool.Count <= 0
                ? new EcsEntityExtended(_entitiesPool, _archetypeManager)
                : _entitiesPool.Dequeue();
            entity.Initialize(_entityCounter++);
            return entity;
        }

        /// <summary>
        /// Creates a new entity, with an initial set of components
        /// </summary>
        /// <param name="component0">Component</param>
        /// <typeparam name="T0">Component Type</typeparam>
        /// <returns>New Entity</returns>
        public EcsEntity CreateEntity<T0>(T0 component0) where T0 : IEcsComponent
        {
            EcsEntityExtended entity = _entitiesPool.Count <= 0
                ? new EcsEntityExtended(_entitiesPool, _archetypeManager)
                : _entitiesPool.Dequeue();
            entity.Initialize(_entityCounter++, component0);
            return entity;
        }

        /// <summary>
        /// Creates a new entity, with an initial set of components
        /// </summary>
        /// <param name="component0">Component</param>
        /// <param name="component1">Component</param>
        /// <typeparam name="T0">Component Type</typeparam>
        /// <typeparam name="T1">Component Type</typeparam>
        /// <returns>New Entity</returns>
        public EcsEntity CreateEntity<T0, T1>(T0 component0, T1 component1)
            where T0 : IEcsComponent where T1 : IEcsComponent
        {
            EcsEntityExtended entity = _entitiesPool.Count <= 0
                ? new EcsEntityExtended(_entitiesPool, _archetypeManager)
                : _entitiesPool.Dequeue();
            entity.Initialize(_entityCounter++, component0, component1);
            return entity;
        }

        /// <summary>
        /// Creates a new entity, with an initial set of components
        /// </summary>
        /// <param name="component0">Component</param>
        /// <param name="component1">Component</param>
        /// <param name="component2">Component</param>
        /// <typeparam name="T0">Component Type</typeparam>
        /// <typeparam name="T1">Component Type</typeparam>
        /// <typeparam name="T2">Component Type</typeparam>
        /// <returns>New Entity</returns>
        public EcsEntity CreateEntity<T0, T1, T2>(T0 component0, T1 component1, T2 component2) where T0 : IEcsComponent
            where T1 : IEcsComponent
            where T2 : IEcsComponent
        {
            EcsEntityExtended entity = _entitiesPool.Count <= 0
                ? new EcsEntityExtended(_entitiesPool, _archetypeManager)
                : _entitiesPool.Dequeue();
            entity.Initialize(_entityCounter++, component0, component1, component2);
            return entity;
        }

        /// <summary>
        /// Creates a new entity, with an initial set of components
        /// </summary>
        /// <param name="component0">Component</param>
        /// <param name="component1">Component</param>
        /// <param name="component2">Component</param>
        /// <param name="component3">Component</param>
        /// <typeparam name="T0">Component Type</typeparam>
        /// <typeparam name="T1">Component Type</typeparam>
        /// <typeparam name="T2">Component Type</typeparam>
        /// <typeparam name="T3">Component Type</typeparam>
        /// <returns>New Entity</returns>
        public EcsEntity CreateEntity<T0, T1, T2, T3>(T0 component0, T1 component1, T2 component2, T3 component3)
            where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent where T3 : IEcsComponent
        {
            EcsEntityExtended entity = _entitiesPool.Count <= 0
                ? new EcsEntityExtended(_entitiesPool, _archetypeManager)
                : _entitiesPool.Dequeue();
            entity.Initialize(_entityCounter++, component0, component1, component2, component3);
            return entity;
        }

        /// <summary>
        /// Get (or create if necessary) a singleton component
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <returns>singleton component</returns>
        public T GetOrCreateSingleton<T>() where T : class, IEcsComponent, new()
        {
            EcsArchetype archetype = _archetypeManager.FindOrCreateArchetype(EcsComponentType<T>.Index);
            foreach (EcsEntity entity in archetype)
            {
                return entity.GetComponent<T>();
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

            public EcsEntityExtended(Queue<EcsEntityExtended> entitiesPool, EcsArchetypeManager archetypeManager) :
                base(archetypeManager)
            {
                _entitiesPool = entitiesPool;
            }

            public void Initialize(uint id)
            {
                InnerInitialize(id);
            }

            public void Initialize<T0>(uint id, T0 component0) where T0 : IEcsComponent
            {
                InnerInitialize(id, component0);
            }

            public void Initialize<T0, T1>(uint id, T0 component0, T1 component1)
                where T0 : IEcsComponent where T1 : IEcsComponent
            {
                InnerInitialize(id, component0, component1);
            }

            public void Initialize<T0, T1, T2>(uint id, T0 component0, T1 component1, T2 component2)
                where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
            {
                InnerInitialize(id, component0, component1, component2);
            }

            public void Initialize<T0, T1, T2, T3>(uint id, T0 component0, T1 component1, T2 component2, T3 component3)
                where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent where T3 : IEcsComponent
            {
                InnerInitialize(id, component0, component1, component2, component3);
            }


            protected override void OnDestroy()
            {
                _entitiesPool.Enqueue(this);
            }
        }
    }
}