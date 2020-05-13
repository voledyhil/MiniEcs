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
        public Type Type { get; }
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
        public Type Type { get; }
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
        public Type Type { get; }
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
        void Update(float deltaTime, EcsWorld world);
    }
}