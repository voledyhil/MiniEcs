using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    public class EcsWorld
    {
        public int ArchetypeCount => _archetypeManager.ArchetypeCount;
        
        private uint _entityCounter = 0;
        private readonly int _capacity;
        private readonly EcsArchetypeManager _archetypeManager;
        private readonly Dictionary<EcsFilter, EcsGroup> _groups = new Dictionary<EcsFilter, EcsGroup>();

        public EcsWorld(byte capacity)
        {
            _capacity = capacity;
            _archetypeManager = new EcsArchetypeManager(capacity);
        }

        public EcsEntity CreateEntity(params IEcsComponent[] components)
        {
            return new EcsEntity(_entityCounter++, _archetypeManager, _capacity, components);
        }

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
                group.IncVersion(version, GetArchetypes(all, any, none, group.Version));
                return group;
            }

            group = new EcsGroup(version, GetArchetypes(all, any, none, 0));
            _groups.Add(filter.Clone(), group);
            return group;
        }

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
    }
}