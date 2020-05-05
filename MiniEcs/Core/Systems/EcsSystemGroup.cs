using System;
using System.Collections.Generic;

namespace MiniEcs.Core.Systems
{
    public class EcsSystemGroup : IEcsSystem
    {
        public IEnumerable<IEcsSystem> Systems => _systems;
        
        private bool _dirty;
        private readonly List<IEcsSystem> _systems = new List<IEcsSystem>();

        public void AddSystem(IEcsSystem system)
        {
            AddSystem(GetSystemGroupHierarchy(system.GetType()), system);
        }

        private void AddSystem(Stack<Type> groupHierarchy, IEcsSystem system)
        {
            if (groupHierarchy.Count > 0)
            {
                Type parentType = groupHierarchy.Pop();

                if (!TryGetSystemGroup(parentType, out EcsSystemGroup systemGroup))
                {
                    systemGroup = (EcsSystemGroup) Activator.CreateInstance(parentType);

                    _systems.Add(systemGroup);
                    _dirty = true;
                }

                systemGroup.AddSystem(groupHierarchy, system);
            }
            else
            {
                _systems.Add(system);
                _dirty = true;
            }
        }

        private static Stack<Type> GetSystemGroupHierarchy(Type type)
        {
            Stack<Type> groupsType = new Stack<Type>();
            EcsUpdateInGroupAttribute attribute =
                (EcsUpdateInGroupAttribute) Attribute.GetCustomAttribute(type, typeof(EcsUpdateInGroupAttribute));

            while (attribute != null)
            {
                groupsType.Push(attribute.Type);
                attribute = (EcsUpdateInGroupAttribute) Attribute.GetCustomAttribute(attribute.Type,
                    typeof(EcsUpdateInGroupAttribute));
            }

            return groupsType;
        }

        private bool TryGetSystemGroup(Type type, out EcsSystemGroup systemGroup)
        {
            systemGroup = null;
            foreach (IEcsSystem ecsSystem in _systems)
            {
                if (ecsSystem.GetType() != type)
                    continue;

                systemGroup = (EcsSystemGroup) ecsSystem;
                return true;
            }

            return false;
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