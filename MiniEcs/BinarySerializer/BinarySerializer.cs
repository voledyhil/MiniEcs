using System;
using System.Collections.Generic;
using System.Reflection;
using BinarySerializer.Data;
using BinarySerializer.Expressions;
using BinarySerializer.Properties;
using BinarySerializer.Serializers;
using BinarySerializer.Serializers.Baselines;

namespace BinarySerializer
{
    public static class BinarySerializer
    {
        private static readonly BoolWriter BoolWriter = new BoolWriter();
        private static readonly ByteWriter ByteWriter = new ByteWriter();
        private static readonly SByteWriter SbyteWriter = new SByteWriter();
        private static readonly ShortWriter ShortWriter = new ShortWriter();
        private static readonly UShortWriter UShortWriter = new UShortWriter();
        private static readonly IntWriter IntWriter = new IntWriter();
        private static readonly UIntWriter UIntWriter = new UIntWriter();
        private static readonly LongWriter LongWriter = new LongWriter();
        private static readonly ULongWriter UlongWriter = new ULongWriter();
        private static readonly DoubleWriter DoubleWriter = new DoubleWriter();
        private static readonly CharWriter CharWriter = new CharWriter();
        private static readonly FloatWriter FloatWriter = new FloatWriter();
        private static readonly ShortFloatWriter ShortFloatWriter = new ShortFloatWriter();
        private static readonly StringWriter StringWriter = new StringWriter();

        private static readonly IDictionary<Type, CompositeBinarySerializer> Serializers = new Dictionary<Type, CompositeBinarySerializer>();
        private static readonly IDictionary<Type, ObjectActivator> Creators = new Dictionary<Type, ObjectActivator>();

        public static byte[] Serialize(object obj)
        {
            using (BinaryDataWriter writer = new BinaryDataWriter())
            {
                GetSerializer(obj.GetType()).Serialize(obj, writer);
                return writer.GetData();
            }
        }

        public static byte[] Serialize(object obj, Baseline<byte> baseline)
        {
            CompositeBinarySerializer serializer = GetSerializer(obj.GetType());

            if (!baseline.HasValues)
                baseline.CreateValues(serializer.Count);

            using (BinaryDataWriter writer = new BinaryDataWriter())
            {
                serializer.Serialize(obj, writer, baseline);
                return writer.GetData();
            }
        }

        public static void Deserialize(object obj, byte[] data)
        {
            using (BinaryDataReader reader = new BinaryDataReader(data))
            {
                GetSerializer(obj.GetType()).Update(obj, reader);
            }
        }

        public static CompositeBinarySerializer GetSerializer(Type type)
        {
            return Serializers.TryGetValue(type, out CompositeBinarySerializer item) ? item : RegisterType(type);
        }

        public static CompositeBinarySerializer RegisterType(Type ownerType)
        {
            if (Serializers.TryGetValue(ownerType, out CompositeBinarySerializer item))
                return item;
            
            FieldInfo[] fields =
                ownerType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<IBinarySerializer> serializers = new List<IBinarySerializer>(fields.Length);
            for (byte index = 0; index < fields.Length; index++)
            {
                FieldInfo field = fields[index];
                BinaryItemAttribute attr =
                    (BinaryItemAttribute) field.GetCustomAttribute(typeof(BinaryItemAttribute), true);
                if (attr == null)
                    continue;

                Type fieldType = field.FieldType;

                if (fieldType.IsPrimitive)
                {
                    if (fieldType == typeof(bool))
                        serializers.Add(new BoolBinarySerializer(index, ownerType, field, BoolWriter));
                    else if (fieldType == typeof(byte))
                        serializers.Add(new ByteBinarySerializer(index, ownerType, field, ByteWriter));
                    else if (fieldType == typeof(sbyte))
                        serializers.Add(new SByteBinarySerializer(index, ownerType, field, SbyteWriter));
                    else if (fieldType == typeof(short))
                        serializers.Add(new ShortBinarySerializer(index, ownerType, field, ShortWriter));
                    else if (fieldType == typeof(ushort))
                        serializers.Add(new UShortBinarySerializer(index, ownerType, field, UShortWriter));
                    else if (fieldType == typeof(int))
                        serializers.Add(new IntBinarySerializer(index, ownerType, field, IntWriter));
                    else if (fieldType == typeof(uint))
                        serializers.Add(new UIntBinarySerializer(index, ownerType, field, UIntWriter));
                    else if (fieldType == typeof(long))
                        serializers.Add(new LongBinarySerializer(index, ownerType, field, LongWriter));
                    else if (fieldType == typeof(ulong))
                        serializers.Add(new ULongBinarySerializer(index, ownerType, field, UlongWriter));
                    else if (fieldType == typeof(double))
                        serializers.Add(new DoubleBinarySerializer(index, ownerType, field, DoubleWriter));
                    else if (fieldType == typeof(char))
                        serializers.Add(new CharBinarySerializer(index, ownerType, field, CharWriter));
                    else if (fieldType == typeof(float))
                        serializers.Add(attr.IsShort
                            ? (IBinarySerializer) new ShortFloatBinarySerializer(index, ownerType, field,
                                ShortFloatWriter)
                            : new FloatBinarySerializer(index, ownerType, field, FloatWriter));
                    else throw new ArgumentException();
                }
                else if (fieldType.IsEnum)
                {
                    Type underlyingType = Enum.GetUnderlyingType(fieldType);
                    if (underlyingType == typeof(byte))
                        serializers.Add(new ByteEnumBinarySerializer(index, ownerType, field));
                    else if (underlyingType == typeof(int))
                        serializers.Add(new IntEnumBinarySerializer(index, ownerType, field));
                    else throw new ArgumentException("Not supported enum underlying type: " + underlyingType.Name);
                }
                else if (fieldType == typeof(string))
                {
                    serializers.Add(new StringBinarySerializer(index, ownerType, field, StringWriter));
                }
                else if (typeof(IProperty).IsAssignableFrom(fieldType))
                {
                    if (fieldType == typeof(Property<bool>))
                        serializers.Add(new BoolPropertyBinarySerializer(index, ownerType, field, BoolWriter));
                    else if (fieldType == typeof(Property<byte>))
                        serializers.Add(new BytePropertyBinarySerializer(index, ownerType, field, ByteWriter));
                    else if (fieldType == typeof(Property<sbyte>))
                        serializers.Add(new SBytePropertyBinarySerializer(index, ownerType, field, SbyteWriter));
                    else if (fieldType == typeof(Property<short>))
                        serializers.Add(new ShortPropertyBinarySerializer(index, ownerType, field, ShortWriter));
                    else if (fieldType == typeof(Property<ushort>))
                        serializers.Add(new UShortPropertyBinarySerializer(index, ownerType, field, UShortWriter));
                    else if (fieldType == typeof(Property<int>))
                        serializers.Add(new IntPropertyBinarySerializer(index, ownerType, field, IntWriter));
                    else if (fieldType == typeof(Property<uint>))
                        serializers.Add(new UIntPropertyBinarySerializer(index, ownerType, field, UIntWriter));
                    else if (fieldType == typeof(Property<long>))
                        serializers.Add(new LongPropertyBinarySerializer(index, ownerType, field, LongWriter));
                    else if (fieldType == typeof(Property<ulong>))
                        serializers.Add(new ULongPropertyBinarySerializer(index, ownerType, field, UlongWriter));
                    else if (fieldType == typeof(Property<double>))
                        serializers.Add(new DoublePropertyBinarySerializer(index, ownerType, field, DoubleWriter));
                    else if (fieldType == typeof(Property<char>))
                        serializers.Add(new CharPropertyBinarySerializer(index, ownerType, field, CharWriter));
                    else if (fieldType == typeof(Property<string>))
                        serializers.Add(new StringPropertyBinarySerializer(index, ownerType, field, StringWriter));
                    else if (fieldType == typeof(Property<float>))
                        serializers.Add(attr.IsShort
                            ? (IBinarySerializer) new ShortFloatPropertyBinarySerializer(index, ownerType, field,
                                ShortFloatWriter)
                            : new FloatPropertyBinarySerializer(index, ownerType, field, FloatWriter));
                    else throw new ArgumentException();
                }
                else if (fieldType.IsClass)
                {
                    if (Serializers.TryGetValue(fieldType, out CompositeBinarySerializer ser))
                    {
                        serializers.Add(new ByteWrapperBinarySerializer<byte>(index, ownerType, field, ser, ser.Count));
                    }
                    else if (typeof(IBinaryObjectCollection).IsAssignableFrom(fieldType))
                    {
                        Type valueType = fieldType.GenericTypeArguments[0];
                        CompositeBinarySerializer valueSer = GetSerializer(valueType);

                        if (!Creators.TryGetValue(valueType, out ObjectActivator itemCreator))
                        {
                            itemCreator = Expressions.Expressions.InstantiateCreator(valueType.GetConstructor(new Type[] { }));
                            Creators.Add(valueType, itemCreator);
                        }

                        if (typeof(IBinaryObjectCollection<byte>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<byte>(index, ownerType, field,
                                new DictionaryByteKeyBinarySerializer(itemCreator, valueSer, ByteWriter)));
                        else if (typeof(IBinaryObjectCollection<short>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<short>(index, ownerType, field,
                                new DictionaryShortKeyBinarySerializer(itemCreator, valueSer, ShortWriter)));
                        else if (typeof(IBinaryObjectCollection<ushort>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<ushort>(index, ownerType, field,
                                new DictionaryUShortKeyBinarySerializer(itemCreator, valueSer, UShortWriter)));
                        else if (typeof(IBinaryObjectCollection<int>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<int>(index, ownerType, field,
                                new DictionaryIntKeyBinarySerializer(itemCreator, valueSer, IntWriter)));
                        else if (typeof(IBinaryObjectCollection<uint>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<uint>(index, ownerType, field,
                                new DictionaryUIntKeyBinarySerializer(itemCreator, valueSer, UIntWriter)));
                        else if (typeof(IBinaryObjectCollection<long>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<long>(index, ownerType, field,
                                new DictionaryLongKeyBinarySerializer(itemCreator, valueSer, LongWriter)));
                        else if (typeof(IBinaryObjectCollection<ulong>).IsAssignableFrom(fieldType))
                            serializers.Add(new ByteWrapperBinarySerializer<ulong>(index, ownerType, field,
                                new DictionaryULongKeyBinarySerializer(itemCreator, valueSer, UlongWriter)));
                        else throw new ArgumentException();
                    }
                    else
                    {
                        ser = RegisterType(fieldType);
                        serializers.Add(new ByteWrapperBinarySerializer<byte>(index, ownerType, field, ser, ser.Count));
                    }
                }
                else throw new ArgumentException();
            }

            CompositeBinarySerializer serializer = new CompositeBinarySerializer(serializers);
            Serializers.Add(ownerType, new CompositeBinarySerializer(serializers));
            return serializer;
        }
    }
}