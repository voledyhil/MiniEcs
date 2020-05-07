namespace MiniEcs.Core
{    
    public class EcsEntity
    {
        public uint Id { get; }
        
        private EcsArchetype _archetype;
        private EcsArchetypeManager _archetypeManager;
        private IEcsComponent[] _components;

        public EcsEntity(uint id, EcsArchetypeManager archetypeManager, int capacity, params IEcsComponent[] components)
        {
            Id = id;
        
            _components = new IEcsComponent[capacity];            
            _archetypeManager = archetypeManager;
            
            _archetype = _archetypeManager.Root;
            _archetype.Entities.Add(this);
        
            foreach (IEcsComponent component in components)
            {
                this[component.Index] = component;
            }
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

                _archetype.Entities.Remove(this);
                _archetype = add
                    ? _archetypeManager.FindOrCreateNextArchetype(_archetype, index)
                    : _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);
                _archetype.Entities.Add(this);

                _components[index] = value;
            }
        }

        public void Destroy()
        {
            _archetype.Entities.Remove(this);
            _archetype = null;
            _components = null;
            _archetypeManager = null;
        }
       
    }
}