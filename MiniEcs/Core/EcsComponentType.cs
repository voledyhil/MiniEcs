using System.Threading;

namespace MiniEcs.Core
{
    internal sealed class EcsComponentTypeCount
    {
        internal static int ComponentTypesCount;
    }

    public static class EcsComponentType<T> where T : IEcsComponent
    {
        public static readonly byte Index;

        static EcsComponentType()
        {
            Index = (byte) Interlocked.Increment(ref EcsComponentTypeCount.ComponentTypesCount);
        }
    }
}