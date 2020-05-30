using System;
using System.Reflection;
using BinarySerializer.Data;
using BinarySerializer.Expressions;
using BinarySerializer.Serializers.Baselines;

namespace BinarySerializer.Serializers
{
    public abstract class WrapperBinarySerializer<TKey, TChildKey> : IBinarySerializer<TKey>
        where TKey : unmanaged where TChildKey : unmanaged
    {
        private readonly TKey _index;
        private readonly Getter<object> _getter;
        private readonly IBinarySerializer _serializer;
        private readonly int _valuesCount;

        protected WrapperBinarySerializer(TKey index, Type ownerType, FieldInfo field, IBinarySerializer serializer, int valuesCount = 0)
        {
            _index = index;
            _serializer = serializer;
            _valuesCount = valuesCount;
            _getter = new Getter<object>(ownerType, field);
        }

        protected abstract void WriteKey(TKey index, BinaryDataWriter writer);

        public void Update(object obj, BinaryDataReader reader)
        {
            using (BinaryDataReader childReader = reader.ReadNode())
            {
                _serializer.Update(_getter.Get(obj), childReader);
            }
        }

        public void Serialize(object obj, BinaryDataWriter writer)
        {
            BinaryDataWriter childWriter = writer.TryWriteNode(sizeof(byte));
            _serializer.Serialize(_getter.Get(obj), childWriter);

            if (childWriter.Length <= 0)
                return;

            WriteKey(_index, writer);
            childWriter.PushNode();
        }

        public void Serialize(object obj, BinaryDataWriter writer, IBaseline baseline)
        {
            Serialize(obj, writer, (IBaseline<TKey>) baseline);
        }

        public void Serialize(object obj, BinaryDataWriter writer, IBaseline<TKey> baseline)
        {
            BinaryDataWriter childWriter = writer.TryWriteNode(sizeof(byte));
            Baseline<TChildKey> itemBaseline = baseline.GetOrCreateBaseline<Baseline<TChildKey>>(_index, _valuesCount, out bool isNew);
            _serializer.Serialize(_getter.Get(obj), childWriter, itemBaseline);

            if (childWriter.Length <= 0 && !isNew)
                return;

            WriteKey(_index, writer);
            childWriter.PushNode();
        }
    }

    public class ByteWrapperBinarySerializer<TChildKey> : WrapperBinarySerializer<byte, TChildKey>
        where TChildKey : unmanaged
    {
        public ByteWrapperBinarySerializer(byte index, Type ownerType, FieldInfo field, IBinarySerializer serializer, int valuesCount = 0) :
            base(index, ownerType, field, serializer, valuesCount)
        {
        }

        protected override void WriteKey(byte index, BinaryDataWriter writer)
        {
            writer.WriteByte(index);
        }
    }
}