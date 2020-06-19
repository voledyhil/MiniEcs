using System;
using System.Reflection;
using BinarySerializer.Data;
using BinarySerializer.Properties;
using BinarySerializer.Serializers.Baselines;

namespace BinarySerializer.Serializers
{
    public abstract class PropertyBinarySerializer<T, TWriter> : IBinarySerializer<byte>
        where TWriter : IPrimitiveWriter<T>
    {
        private readonly byte _index;
        private readonly Func<object, Property<T>> _getter;
        private readonly TWriter _writer;

        protected PropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, TWriter writer)
        {
            _index = index;
            _writer = writer;
            _getter = Expressions.Expressions.InstantiateGetter<Property<T>>(ownerType, field);
        }

        void IBinarySerializer.Serialize(object obj, BinaryDataWriter writer, IBaseline baseline)
        {
            Serialize(obj, writer, (IBaseline<byte>) baseline);
        }

        public void Update(object obj, BinaryDataReader reader)
        {
            _getter(obj).Update(_writer.Read(reader));
        }

        public void Serialize(object obj, BinaryDataWriter writer)
        {
            Property<T> property = _getter(obj);
            if (_writer.Equals(property.Value, default))
                return;

            writer.WriteByte(_index);
            _writer.Write(writer, property.Value);
        }

        public void Serialize(object obj, BinaryDataWriter writer, IBaseline<byte> baseline)
        {
            Property<T> property = _getter(obj);
            if (baseline[_index] == property.Version)
                return;

            baseline[_index] = property.Version;
            writer.WriteByte(_index);
            _writer.Write(writer, property.Value);
        }
    }

    public sealed class BoolPropertyBinarySerializer : PropertyBinarySerializer<bool, BoolWriter>
    {
        public BoolPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, BoolWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class BytePropertyBinarySerializer : PropertyBinarySerializer<byte, ByteWriter>
    {
        public BytePropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, ByteWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class SBytePropertyBinarySerializer : PropertyBinarySerializer<sbyte, SByteWriter>
    {
        public SBytePropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, SByteWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ShortPropertyBinarySerializer : PropertyBinarySerializer<short, ShortWriter>
    {
        public ShortPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, ShortWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class UShortPropertyBinarySerializer : PropertyBinarySerializer<ushort, UShortWriter>
    {
        public UShortPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, UShortWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class IntPropertyBinarySerializer : PropertyBinarySerializer<int, IntWriter>
    {
        public IntPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, IntWriter writer) : base(index,
            ownerType, field, writer)
        {
        }
    }

    public sealed class UIntPropertyBinarySerializer : PropertyBinarySerializer<uint, UIntWriter>
    {
        public UIntPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, UIntWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class LongPropertyBinarySerializer : PropertyBinarySerializer<long, LongWriter>
    {
        public LongPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, LongWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ULongPropertyBinarySerializer : PropertyBinarySerializer<ulong, ULongWriter>
    {
        public ULongPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, ULongWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class DoublePropertyBinarySerializer : PropertyBinarySerializer<double, DoubleWriter>
    {
        public DoublePropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, DoubleWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class CharPropertyBinarySerializer : PropertyBinarySerializer<char, CharWriter>
    {
        public CharPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, CharWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class FloatPropertyBinarySerializer : PropertyBinarySerializer<float, FloatWriter>
    {
        public FloatPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, FloatWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }

    public sealed class ShortFloatPropertyBinarySerializer : PropertyBinarySerializer<float, ShortFloatWriter>
    {
        public ShortFloatPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, ShortFloatWriter writer)
            : base(index, ownerType, field, writer)
        {
        }
    }

    public sealed class StringPropertyBinarySerializer : PropertyBinarySerializer<string, StringWriter>
    {
        public StringPropertyBinarySerializer(byte index, Type ownerType, FieldInfo field, StringWriter writer) : base(
            index, ownerType, field, writer)
        {
        }
    }
}