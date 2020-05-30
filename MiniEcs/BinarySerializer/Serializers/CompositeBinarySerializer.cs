using System.Collections.Generic;
using BinarySerializer.Data;
using BinarySerializer.Serializers.Baselines;

namespace BinarySerializer.Serializers
{
    public class CompositeBinarySerializer : IBinarySerializer
    {
        public int Count => _items.Count;
        private readonly List<IBinarySerializer> _items;

        public CompositeBinarySerializer(List<IBinarySerializer> items)
        {
            _items = items;
        }

        public void Update(object obj, BinaryDataReader reader)
        {
            while (reader.Position < reader.Length)
            {
                _items[reader.ReadByte()].Update(obj, reader);
            }
        }

        public void Serialize(object obj, BinaryDataWriter writer)
        {
            for (int i = 0; i < _items.Count && i < byte.MaxValue; i++)
            {
                _items[i].Serialize(obj, writer);
            }
        }

        public void Serialize(object obj, BinaryDataWriter writer, IBaseline baseline)
        {
            for (int i = 0; i < _items.Count && i < byte.MaxValue; i++)
            {
                _items[i].Serialize(obj, writer, baseline);
            }
        }
    }
}