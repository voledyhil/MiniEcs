using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEcs.Core
{
    public class EcsWorld
    {
        private uint _entityCounter = 0;
        private readonly EcsArchetypeManager _archetypeManager;

        private readonly int _capacity;

        public EcsWorld(byte capacity)
        {
            _capacity = capacity;
            _archetypeManager = new EcsArchetypeManager(capacity);
        }

        public EcsEntity CreateEntity(params IEcsComponent[] components)
        {
            return new EcsEntity(_entityCounter++, _archetypeManager, _capacity, components);
        }

        public IEcsArchetype GetArchetype(params byte[] indices)
        {
            return _archetypeManager.FindOrCreateArchetype(indices);
        }

        private readonly Dictionary<EcsFilter, EcsGroup> _groups = new Dictionary<EcsFilter, EcsGroup>();
        public IEcsGroup Filter(EcsFilter filter)
        {
            int version = _archetypeManager.ArchetypeCount - 1;
            if (_groups.TryGetValue(filter, out EcsGroup group))
            {
                if (group.Version < version)
                    group.Add(version, GetArchetypes(filter, version));
                return group;
            }

            group = new EcsGroup(version, GetArchetypes(filter, 0));
            _groups.Add(filter.Clone(), group);
            return group;
        }

        private IEnumerable<EcsArchetype> GetArchetypes(EcsFilter filter, int startId)
        {
            HashSet<EcsArchetype> buffer0 = null;
            HashSet<EcsArchetype> buffer1 = null;

            if (filter.All != null || filter.Any != null)
            {
                if (filter.All != null)
                {
                    byte[] all = filter.All.ToArray();

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

                if (filter.Any != null)
                {
                    byte[] any = filter.Any.ToArray();

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

            if (filter.None != null)
            {
                foreach (byte type in filter.None)
                {
                    buffer0.ExceptWith(_archetypeManager.GetArchetypes(type, startId));
                }
            }

            return buffer0;
        }
    }
}