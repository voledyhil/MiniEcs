using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEcs.Core
{
    public class EcsWorld
    {
        private uint _entityCounter = 0;
        
        private readonly Dictionary<uint, EcsEntity> _entities = new Dictionary<uint, EcsEntity>();
        private readonly EcsArchetypeManager _archetypeManager;

        private readonly int _capacity;

        public EcsWorld(byte capacity)
        {
            _capacity = capacity;
            _archetypeManager = new EcsArchetypeManager(capacity);
        }

        public EcsEntity this[uint id] => _entities[id];

        public EcsEntity CreateEntity(params IEcsComponent[] components)
        {
            uint id = _entityCounter++;
            EcsEntity entity = new EcsEntity(id, _archetypeManager, _capacity, components);
            _entities.Add(id, entity);

            return entity;
        }

        public void AddComponents(EcsFilter filter, params IEcsComponent[] components)
        {
            List<EcsEntity> entities = new List<EcsEntity>(Filter(filter));
            foreach (EcsEntity entity in entities)
            {
                foreach (IEcsComponent component in components)
                {
                    entity[component.Index] = component;
                }
            }
        }

        public IEcsArchetype GetArchetype(params byte[] indices)
        {
            return _archetypeManager.FindOrCreateArchetype(indices);
        }

        private readonly Dictionary<EcsFilter, EcsGroup> _groups = new Dictionary<EcsFilter, EcsGroup>();
        public IEcsGroup Filter(EcsFilter filter)
        {
            byte[] all = filter.All?.ToArray();
            byte[] any = filter.Any?.ToArray();
            byte[] none = filter.None?.ToArray();
            
            int version = _archetypeManager.ArchetypeCount - 1;
            if (_groups.TryGetValue(filter, out EcsGroup group))
            {
                if (group.Version < version)
                    group.Add(version, GetArchetypes(all, any, none, version));
                return group;
            }

            group = new EcsGroup(version, GetArchetypes(all, any, none, 0));
            _groups.Add(filter.Clone(), group);
            return group;
        }

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
    }
}