using System;

namespace MiniEcs.Core
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

    public interface IEcsSystem
    {
        void Update(float deltaTime, EcsWorld world);
    }
}