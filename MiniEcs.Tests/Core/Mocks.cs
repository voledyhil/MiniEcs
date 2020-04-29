using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{

    public static class ComponentType
    {
        public static byte A = 0;
        public static byte B = 1;
        public static byte C = 2;
        public static byte D = 3;

        public static byte Capacity = 4;
    }

    public class ComponentA : IEcsComponent
    {
    }

    public class ComponentB : IEcsComponent
    {
    }

    public class ComponentC : IEcsComponent
    {
    }

    public class ComponentD : IEcsComponent
    {
    }
}