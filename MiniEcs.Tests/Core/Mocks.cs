using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{

    public static class ComponentType
    {
        public const byte A = 0;
        public const byte B = 1;
        public const byte C = 2;
        public const byte D = 3;

        public const byte TotalComponents = 4;
    }

    public class ComponentA : IEcsComponent
    {
        public byte Index => ComponentType.A;
    }

    public class ComponentB : IEcsComponent
    {
        public byte Index => ComponentType.B;
    }

    public class ComponentC : IEcsComponent
    {
        public byte Index => ComponentType.C;
    }

    public class ComponentD : IEcsComponent
    {
        public byte Index => ComponentType.D;
    }
}