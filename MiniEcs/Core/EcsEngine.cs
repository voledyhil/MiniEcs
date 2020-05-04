using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsEngine
    {
        private readonly List<IEcsSystem> _systems = new List<IEcsSystem>();
        private bool _dirty;

        public void AddSystem(IEcsSystem system)
        {
            _systems.Add(system);
            _dirty = true;
        }

        public void Update(float deltaTime, EcsWorld world)
        {
            if (_dirty)
            {
                SystemSorter.Sort(_systems);
                _dirty = false;
            }
            
            foreach (IEcsSystem system in _systems)
            {
                system.Update(deltaTime, world);
            }
        }
        
    }
}