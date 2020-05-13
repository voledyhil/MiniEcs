namespace MiniEcs.Core
{
    public sealed class EcsTypeManager
    {
        public static readonly IEcsComponentPoolCreator[] ComponentPoolCreators =
            new IEcsComponentPoolCreator[byte.MaxValue];

        internal static int Length;

        internal static void InstantiateComponentPoolCreator<TC>() where TC : IEcsComponent
        {
            ComponentPoolCreators[Length] = new EcsComponentPoolCreator<TC>();
            Length++;
        }
    }

    public static class EcsComponentType<TC> where TC : IEcsComponent
    {
        public static readonly byte Index;

        static EcsComponentType()
        {
            Index = (byte) EcsTypeManager.Length;
            EcsTypeManager.InstantiateComponentPoolCreator<TC>();
        }
    }
}