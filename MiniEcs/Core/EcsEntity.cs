using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEcs.Core
{
    public class EcsEntity
    {
        public uint Id { get; }
        
        private EcsArchetype _archetype;
        private EcsArchetypeManager _archetypeManager;
        private IEcsComponent[] _components;
        private HashSet<byte> _indices;

        public EcsEntity(uint id, EcsArchetypeManager archetypeManager, int capacity, params IEcsComponent[] components)
        {
            Id = id;
            
            _archetypeManager = archetypeManager;
            _components = new IEcsComponent[capacity];

            byte[] indices = new byte[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                IEcsComponent component = components[i];
                _components[component.Index] = component;
                indices[i] = component.Index;
            }

            _archetype = _archetypeManager.FindOrCreateArchetype(indices);
            _archetype.Entities.Add(id, this);
            _indices = new HashSet<byte>(indices);
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

            _archetype.Entities.Remove(Id);

            if (!edges.TryGetValue(index, out _archetype))
                _archetype = _archetypeManager.FindOrCreateArchetype(_indices.ToArray());

            _archetype.Entities.Add(Id, this);
        }

        public void Destroy()
        {
            _archetype.Entities.Remove(Id);
            _archetype = null;
            _components = null;
            _indices = null;
            _archetypeManager = null;
        }
       
    }
}