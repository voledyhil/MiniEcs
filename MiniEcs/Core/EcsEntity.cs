using System;

namespace MiniEcs.Core
{    
    /// <summary>
    /// An entity is a collection of unique components
    /// </summary>
    public class EcsEntity
    {
        /// <summary>
        /// Unique identifier of an entity
        /// </summary>
        public uint Id { get; }
        
        /// <summary>
        /// Current Entity Archetype
        /// </summary>
        private EcsArchetype _archetype;
        /// <summary>
        /// Archetype Manager
        /// </summary>
        private EcsArchetypeManager _archetypeManager;
        
        /// <summary>
        /// Сollection of components. Memory is allocated for all possible types of
        /// unique components, for quick access by index
        /// </summary>
        private IEcsComponent[] _components;

        /// <summary>
        /// Creates a new entity, with an initial set of components.
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <param name="archetypeManager">Archetype Manager</param>
        /// <param name="capacity">Number of all possible component types</param>
        /// <param name="components">Initial set of components</param>
        public EcsEntity(uint id, EcsArchetypeManager archetypeManager, int capacity, params IEcsComponent[] components)
        {
            Id = id;
        
            _components = new IEcsComponent[capacity];            
            _archetypeManager = archetypeManager;
            
            _archetype = _archetypeManager.Empty;
            _archetype.Entities.Add(this);
        
            foreach (IEcsComponent component in components)
            {
                this[component.Index] = component;
            }
        }

        /// <summary>
        /// Getting, adding, removing entity components
        /// </summary>
        /// <param name="index">Component type</param>
        /// <exception cref="ArgumentException">
        /// Occurs if an attempt is made to reassign a component,
        /// if a mismatch of component types occurs,
        /// if an attempt is made to remove non-existent components
        /// </exception>
        public IEcsComponent this[byte index]
        {
            get => _components[index];
            set
            {
                IEcsComponent component = _components[index];

                bool add = component == null && value != null;
                bool remove = component != null && value == null;

                if (!add && !remove)
                    throw new ArgumentException();
                
                if (add && value.Index != index)
                    throw new ArgumentException();

                _archetype.Entities.Remove(this);
                _archetype = add
                    ? _archetypeManager.FindOrCreateNextArchetype(_archetype, index)
                    : _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);
                _archetype.Entities.Add(this);

                _components[index] = value;
            }
        }

        /// <summary>
        /// Destroy entity
        /// </summary>
        public void Destroy()
        {
            _archetype.Entities.Remove(this);
            _archetype = null;
            _components = null;
            _archetypeManager = null;
        }
       
    }
}