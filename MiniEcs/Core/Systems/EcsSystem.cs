using System;

namespace MiniEcs.Core.Systems
{
    /// <inheritdoc />
    /// <summary>
    /// A system or system groups is updated
    /// after the specified system or system groups.
    /// If the specified type is not found, this attribute is ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EcsUpdateAfterAttribute : Attribute
    {
        /// <summary>
        /// Type of system or systems group
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates an object of class "EcsUpdateAfterAttribute"
        /// </summary>
        /// <param name="type">Type of system or systems group</param>
        public EcsUpdateAfterAttribute(Type type)
        {
            Type = type;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// A system or system groups is updated
    /// before the specified system or system groups.
    /// If the specified type is not found, this attribute is ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EcsUpdateBeforeAttribute : Attribute
    {
        /// <summary>
        /// Type of system or systems group
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates an object of class "EcsUpdateBeforeAttribute"
        /// </summary>
        /// <param name="type">Type of system or systems group</param>
        public EcsUpdateBeforeAttribute(Type type)
        {
            Type = type;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// A system or group of systems must belong to the specified group.
    /// If the specified group does not exist, the group will be created automatically.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EcsUpdateInGroupAttribute : Attribute
    {
        /// <summary>
        /// Type of system or systems group
        /// </summary>
        public Type Type { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates an object of class "EcsUpdateInGroupAttribute"
        /// </summary>
        /// <param name="type">Type of system or systems group</param>
        public EcsUpdateInGroupAttribute(Type type)
        {
            Type = type;
        }
    }

    /// <summary>
    /// Interface for all systems and systems groups
    /// </summary>
    public interface IEcsSystem
    {
        /// <summary>
        /// Updates the list of systems and system groups.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last update</param>
        /// <param name="world">Entity Manager <see cref="EcsWorld"/></param>
        void Update(float deltaTime, EcsWorld world);
    }
}