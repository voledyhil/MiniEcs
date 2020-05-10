using System;
using System.Collections.Generic;

namespace MiniEcs.Core.Systems
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for systems or other groups of systems.
    /// Using the attributes <see cref="T:MiniEcs.Core.Systems.EcsUpdateAfterAttribute" /> and <see cref="T:MiniEcs.Core.Systems.EcsUpdateAfterAttribute" />,
    /// the order of updating the nodes in the group is indicated.
    /// Using the attribute <see cref="T:MiniEcs.Core.Systems.EcsUpdateInGroupAttribute" /> can be added to the parent group.
    /// </summary>
    public class EcsSystemGroup : IEcsSystem
    {
        /// <summary>
        /// Enumerator of child systems and systems group
        /// </summary>
        public IEnumerable<IEcsSystem> Systems => _systems;

        /// <summary>
        /// Indicates that after updating the list, nodes should be sorted.
        /// </summary>
        private bool _dirty;
        
        /// <summary>
        /// List of child systems and systems group
        /// </summary>
        private readonly List<IEcsSystem> _systems = new List<IEcsSystem>();

        /// <summary>
        /// Adds a system or group of systems to the list
        /// </summary>
        /// <param name="system">System or group of systems</param>
        public void AddSystem(IEcsSystem system)
        {
            AddSystem(GetSystemGroupHierarchy(system.GetType()), system);
        }

        /// <summary>
        /// Adds a system or group of systems to the list
        /// </summary>
        /// <param name="groupHierarchy">Parent group hierarchy</param>
        /// <param name="system">System or group of systems</param>
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

        /// <summary>
        /// Gets the parent group hierarchy
        /// </summary>
        /// <param name="type">Node type</param>
        /// <returns>Stack of parent nodes</returns>
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

        /// <summary>
        /// Trying to find a node of the specified type
        /// </summary>
        /// <param name="type">Node type</param>
        /// <param name="systemGroup">found node</param>
        /// <returns>true - the node exists. false - node not found</returns>
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

        /// <summary>
        /// Updates the list of systems and system groups.
        /// Before updating, sorts the items in the list, if necessary.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last update</param>
        /// <param name="world">Entity Manager <see cref="EcsWorld"/></param>
        public void Update(float deltaTime, EcsWorld world)
        {
            if (_dirty)
            {
                EcsSystemSorter.Sort(_systems);
                _dirty = false;
            }

            foreach (IEcsSystem system in _systems)
            {
                system.Update(deltaTime, world);
            }
        }
    }
}