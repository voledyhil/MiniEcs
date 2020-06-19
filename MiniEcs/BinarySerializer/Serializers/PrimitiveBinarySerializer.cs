using System;
using System.Reflection;
using BinarySerializer.Data;
using BinarySerializer.Serializers.Baselines;

namespace BinarySerializer.Serializers
{
    public abstract class PrimitiveBinarySerializer<T, TWriter> : IBinarySerializer<byte>
        where TWriter : IPrimitiveWriter<T>
    {
        private readonly byte _index;
        private readonly TWriter _writer;
        private readonly Func<object, T> _getter;
        private readonly Action<object, T> _setter;

        protected PrimitiveBinarySerializer(byte index, Type ownerType, FieldInfo field, TWriter writer)
        {
            _index = index;
            _writer = writer;
            _getter = Expressions.Expressions.InstantiateGetter<T>(ownerType, field);
            _setter = Expressions.Expressions.InstantiateSetter<T>(ownerType, field);
        }

        void IBinarySerializer.Serialize(object obj, BinaryDataWriter writer, IBaseline baseline)
        {
            Serialize(obj, writer, (IBaseline<byte>) baseline);
        }

        public void Serialize(object obj, BinaryDataWriter writer)
        {
            T value = _getter(obj);
            if (_writer.Equals(value, default))
                return;

            writer.WriteByte(_index);
            _writer.Write(writer, value);
        }

        public void Update(object obj, BinaryDataReader reader)
        {
            _setter(obj, _writer.Read(reader));
        }

        public void Serialize(object obj, BinaryDataWriter writer, IBaseline<byte> baseline)
        {
            T value = _getter(obj);
            int hash = _writer.GetHashCode(value);
            int baseHash = baseline[_index];
            if (baseHash == 0 && Equals(value, default) || baseHash == hash)
                return;
            baseline[_index] = hash;
            writer.WriteByte(_index);
            _writer.Write(writer, value);
        }
    }


    public sealed class BoolBinarySerializer : PrimitiveBinarySerializer<bool, BoolWriter>
    {
        public BoolBinarySerializer(byte index, Type ownerType, FieldInfo field, BoolWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ByteBinarySerializer : PrimitiveBinarySerializer<byte, ByteWriter>
    {
        public ByteBinarySerializer(byte index, Type ownerType, FieldInfo field, ByteWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class SByteBinarySerializer : PrimitiveBinarySerializer<sbyte, SByteWriter>
    {
        public SByteBinarySerializer(byte index, Type ownerType, FieldInfo field, SByteWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ShortBinarySerializer : PrimitiveBinarySerializer<short, ShortWriter>
    {
        public ShortBinarySerializer(byte index, Type ownerType, FieldInfo field, ShortWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class UShortBinarySerializer : PrimitiveBinarySerializer<ushort, UShortWriter>
    {
        public UShortBinarySerializer(byte index, Type ownerType, FieldInfo field, UShortWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class IntBinarySerializer : PrimitiveBinarySerializer<int, IntWriter>
    {
        public IntBinarySerializer(byte index, Type ownerType, FieldInfo field, IntWriter writer) : base(index,
            ownerType, field, writer)
        {
        }
    }

    public sealed class UIntBinarySerializer : PrimitiveBinarySerializer<uint, UIntWriter>
    {
        public UIntBinarySerializer(byte index, Type ownerType, FieldInfo field, UIntWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class LongBinarySerializer : PrimitiveBinarySerializer<long, LongWriter>
    {
        public LongBinarySerializer(byte index, Type ownerType, FieldInfo field, LongWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ULongBinarySerializer : PrimitiveBinarySerializer<ulong, ULongWriter>
    {
        public ULongBinarySerializer(byte index, Type ownerType, FieldInfo field, ULongWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class DoubleBinarySerializer : PrimitiveBinarySerializer<double, DoubleWriter>
    {
        public DoubleBinarySerializer(byte index, Type ownerType, FieldInfo field, DoubleWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class CharBinarySerializer : PrimitiveBinarySerializer<char, CharWriter>
    {
        public CharBinarySerializer(byte index, Type ownerType, FieldInfo field, CharWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class FloatBinarySerializer : PrimitiveBinarySerializer<float, FloatWriter>
    {
        public FloatBinarySerializer(byte index, Type ownerType, FieldInfo field, FloatWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ShortFloatBinarySerializer : PrimitiveBinarySerializer<float, ShortFloatWriter>
    {
        public ShortFloatBinarySerializer(byte index, Type ownerType, FieldInfo field, ShortFloatWriter writer)
            : base(index, ownerType, field, writer)
        {
        }
    }

    public sealed class StringBinarySerializer : PrimitiveBinarySerializer<string, StringWriter>
    {
        public StringBinarySerializer(byte index, Type ownerType, FieldInfo field, StringWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }
}