using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEcs.Core
{
    public class EcsEntity
    {
        private EcsArchetype _archetype;
        private EcsArchetypeManager _archetypeManager;
        private IEcsComponent[] _components;
        private HashSet<byte> _indices;
        
        private readonly uint _id;

        public EcsEntity(uint id, EcsArchetypeManager archetypeManager, int capacity)
        {
            _id = id;
            _archetypeManager = archetypeManager;
            _components = new IEcsComponent[capacity];
            _indices = new HashSet<byte>();
            
            _archetype = _archetypeManager.RootArchetype;
            _archetype.Entities.Add(id, this);
        }

        public IEcsComponent this[byte index]
        {
            get => _components[index];
            set
            {
                IEcsComponent component = _components[index];

                bool add = component == null && value != null;
                bool remove = component != null && value == null;

                if (!add && !remove) 
                    return;

                ChangeArchetype(index, add);
                
                _components[index] = value;
            }
        }

        private void ChangeArchetype(byte index, bool inc)
        {
            int indicesCount = _indices.Count;
            IDictionary<byte, EcsArchetype> edges;
            
            if (inc)
            {
                _indices.Add(index);
                edges = _archetype.Next;
            }
            else
            {
                _indices.Remove(index);
                edges = _archetype.Prior;
            }

            if (indicesCount == _indices.Count)
                throw new InvalidOperationException();

            _archetype.Entities.Remove(_id);

            if (!edges.TryGetValue(index, out _archetype))
                _archetype = _archetypeManager.FindOrCreateArchetype(_indices.ToArray());

            _archetype.Entities.Add(_id, this);
        }

        public void Destroy()
        {
            _archetype.Entities.Remove(_id);
            _archetype = null;
            _components = null;
            _indices = null;
            _archetypeManager = null;
        }
       
    }
}