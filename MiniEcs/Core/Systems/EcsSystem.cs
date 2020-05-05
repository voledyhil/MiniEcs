using System;

namespace MiniEcs.Core.Systems
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EcsUpdateAfterAttribute : Attribute
    {
        public Type Type { get; }

        public EcsUpdateAfterAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EcsUpdateBeforeAttribute : Attribute
    {
        public Type Type { get; }

        public EcsUpdateBeforeAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EcsUpdateInGroupAttribute : Attribute
    {
        public Type Type { get; }

        public EcsUpdateInGroupAttribute(Type type)
        {
            Type = type;
        }
    }
}