namespace MiniEcs.Core
{
    public class EcsEntityIdentifiers
    {
        private ushort _idCounter = 1;
        private ushort _length;
        private readonly uint[] _available = new uint[ushort.MaxValue];

        public uint Next()
        {   
            while (_length > 0)
            {
                uint entity = _available[--_length];
                ushort version = GetVersion(entity);
                
                if (++version == ushort.MaxValue)
                    continue;
                
                return Generate(GetId(entity), version);
            }
            
            return Generate(_idCounter++, 0);
        }

        public void Recycle(uint entity)
        {
            _available[_length++] = entity;
        }
        
        public static uint Generate(ushort id, ushort version)
        {
            return (uint) (id << 16) | version;
        }

        public static ushort GetId(uint entity)
        {
            return (ushort) (entity >> 16);
        }

        public static ushort GetVersion(uint entity)
        {
            return (ushort) (entity & ushort.MaxValue);
        }
    }
}