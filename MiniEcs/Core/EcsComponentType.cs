using System;
using Serializer = BinarySerializer.BinarySerializer;

namespace MiniEcs.Core
{
    public static class EcsTypeManager
    {
        public static readonly IEcsComponentPoolCreator[] ComponentPoolCreators = new IEcsComponentPoolCreator[byte.MaxValue];
        public static readonly Type[] Types = new Type[byte.MaxValue];
        
        private static byte _length;
        internal static byte RegisterType<TC>() where TC : class, IEcsComponent, new()
        {
            Type type = typeof(TC);

            int index = Array.IndexOf(Types, type);
            if (index > -1)
                return (byte)index;
            
            index = _length++;

            Types[index] = type;
            ComponentPoolCreators[index] = new EcsComponentPoolCreator<TC>();
            return (byte) index;
        }

        public static IEcsComponent CreateComponent(byte index)
        {
            return ComponentPoolCreators[index].CreateComponent();
        }
    }

    public static class EcsComponentType<TC> where TC : class, IEcsComponent, new()
    {
        public static byte Index
        {
            get
            {
                if (!_isRegister)
                    throw new InvalidOperationException($"component '{typeof(TC)}' is not register");
                return _index;
            }
        }

        private static byte _index;
        private static bool _isRegister;
        
        public static void Register()
        {
            _isRegister = true;
            _index = EcsTypeManager.RegisterType<TC>();
            
            Serializer.RegisterType(typeof(TC));
            
        }
    }
}