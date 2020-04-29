using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsFilter
    {  
        public IEnumerable<byte> Any => _any;
        public IEnumerable<byte> All => _all;
        public IEnumerable<byte> None => _none;
        
        private readonly HashSet<byte> _any = new HashSet<byte>();
        private readonly HashSet<byte> _all = new HashSet<byte>();
        private readonly HashSet<byte> _none = new HashSet<byte>();
        
        public EcsFilter AnyOf(params byte[] types)
        {
            foreach (byte type in types)
            {
                _any.Add(type);
            }
            return this;
        }

        public EcsFilter AllOf(params byte[] types)
        {
            foreach (byte type in types)
            {
                _all.Add(type);
            }
            return this;
        }

        public EcsFilter NoneOf(params byte[] types)
        {
            foreach (byte type in types)
            {
                _none.Add(type);
            }
            return this;
        }
    }
}