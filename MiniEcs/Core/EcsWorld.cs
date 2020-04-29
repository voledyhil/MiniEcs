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


        public EcsEntity CreateEntity()
        {
            return new EcsEntity(_entityCounter++, _archetypeManager, _capacity);
        }

        public IEcsArchetype GetArchetype(params byte[] indices)
        {
            return _archetypeManager.FindOrCreateArchetype(indices);
        }

        public IEnumerable<EcsEntity> Filter(EcsFilter filter)
        {
            byte[] all = filter.All.ToArray();
            byte[] any = filter.Any.ToArray();
            byte[] none = filter.None.ToArray();

            HashSet<EcsArchetype> buffer0 = null;
            HashSet<EcsArchetype> buffer1 = null;

            if (all.Length > 0 || any.Length > 0)
            {
                if (all.Length > 0)
                {
                    EcsArchetype[][] archetypes = new EcsArchetype[all.Length][];
                    for (int i = 0; i < all.Length; i++)
                    {
                        archetypes[i] = _archetypeManager[all[i]].ToArray();
                    }

                    Array.Sort(archetypes, (a, b) => a.Length - b.Length);

                    buffer0 = new HashSet<EcsArchetype>(archetypes[0]);
                    for (int i = 1; i < all.Length; i++)
                    {
                        buffer0.IntersectWith(archetypes[i]);
                    }
                }

                if (any.Length > 0)
                {
                    buffer1 = new HashSet<EcsArchetype>(_archetypeManager[any[0]]);
                    for (int i = 1; i < any.Length; i++)
                    {
                        buffer1.UnionWith(_archetypeManager[any[i]]);
                    }
                }

                if (all.Length > 0 && any.Length > 0)
                {
                    buffer0.IntersectWith(buffer1);
                }
                else if (any.Length > 0)
                {
                    buffer0 = buffer1;
                }
            }
            else
            {
                buffer0 = new HashSet<EcsArchetype>(_archetypeManager.AllArchetypes);
            }

            if (none.Length > 0)
            {
                foreach (byte type in none)
                {
                    buffer0.ExceptWith(_archetypeManager[type]);
                }
            }

            foreach (EcsArchetype archetype in buffer0)
            {
                foreach (EcsEntity entity in archetype.Entities.Values)
                {
                    yield return entity;
                }
            }
        }

        public EcsForEach WithAll(params byte[] indices)
        {
            return new EcsForEach(this, new EcsFilter().AllOf(indices));
        }
                
        public EcsForEach WithAny(params byte[] indices)
        {
            return new EcsForEach(this, new EcsFilter().AnyOf(indices));
        }

        public EcsForEach WithNone(params byte[] indices)
        {
            return new EcsForEach(this, new EcsFilter().NoneOf(indices));
        }
    }
}