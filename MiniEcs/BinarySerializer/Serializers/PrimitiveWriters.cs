using System;
using BinarySerializer.Data;

namespace BinarySerializer.Serializers
{
    public interface IPrimitiveWriter<T>
    {
        int GetHashCode(T value);
        bool Equals(T a, T b);
        void Write(BinaryDataWriter writer, T value);
        T Read(BinaryDataReader reader);
    }
    
    public sealed class BoolWriter : IPrimitiveWriter<bool>
    {
        public int GetHashCode(bool value)
        {
            return Convert.ToInt32(value);
        }

        public bool Equals(bool a, bool b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, bool value)
        {
            writer.WriteBool(value);
        }

        public bool Read(BinaryDataReader reader)
        {
            return reader.ReadBool();
        }
    }

    public sealed class ByteWriter : IPrimitiveWriter<byte>
    {
        public int GetHashCode(byte value)
        {
            return value;
        }

        public bool Equals(byte a, byte b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, byte value)
        {
            writer.WriteByte(value);
        }

        public byte Read(BinaryDataReader reader)
        {
            return reader.ReadByte();
        }
    }

    public sealed class CharWriter : IPrimitiveWriter<char>
    {
        public int GetHashCode(char value)
        {
            return value.GetHashCode();
        }

        public bool Equals(char a, char b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, char value)
        {
            writer.WriteChar(value);
        }

        public char Read(BinaryDataReader reader)
        {
            return reader.ReadChar();
        }
    }

    public sealed class DoubleWriter : IPrimitiveWriter<double>
    {
        public int GetHashCode(double value)
        {
            return value.GetHashCode();
        }

        public bool Equals(double a, double b)
        {
            return Math.Abs(a - b) < 1e-6;
        }

        public void Write(BinaryDataWriter writer, double value)
        {
            writer.WriteDouble(value);
        }

        public double Read(BinaryDataReader reader)
        {
            return reader.ReadDouble();
        }
    }

    public sealed class FloatWriter : IPrimitiveWriter<float>
    {
        public int GetHashCode(float value)
        {
            return value.GetHashCode();
        }

        public bool Equals(float a, float b)
        {
            return Math.Abs(a - b) < 1e-6;
        }

        public void Write(BinaryDataWriter writer, float value)
        {
            writer.WriteFloat(value);
        }

        public float Read(BinaryDataReader reader)
        {
            return reader.ReadFloat();
        }
    }

    public sealed class IntWriter : IPrimitiveWriter<int>
    {
        public int GetHashCode(int value)
        {
            return value;
        }

        public bool Equals(int a, int b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, int value)
        {
            writer.WriteInt(value);
        }

        public int Read(BinaryDataReader reader)
        {
            return reader.ReadInt();
        }
    }

    public sealed class LongWriter : IPrimitiveWriter<long>
    {
        public int GetHashCode(long value)
        {
            return value.GetHashCode();
        }

        public bool Equals(long a, long b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, long value)
        {
            writer.WriteLong(value);
        }

        public long Read(BinaryDataReader reader)
        {
            return reader.ReadLong();
        }
    }

    public sealed class SByteWriter : IPrimitiveWriter<sbyte>
    {
        public int GetHashCode(sbyte value)
        {
            return value;
        }

        public bool Equals(sbyte a, sbyte b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, sbyte value)
        {
            writer.WriteSByte(value);
        }

        public sbyte Read(BinaryDataReader reader)
        {
            return reader.ReadSByte();
        }
    }

    public sealed class ShortWriter : IPrimitiveWriter<short>
    {
        public int GetHashCode(short value)
        {
            return value;
        }

        public bool Equals(short a, short b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, short value)
        {
            writer.WriteShort(value);
        }

        public short Read(BinaryDataReader reader)
        {
            return reader.ReadShort();
        }
    }

    public sealed class ShortFloatWriter : IPrimitiveWriter<float>
    {
        public int GetHashCode(float value)
        {
            return value.GetHashCode();
        }

        public bool Equals(float a, float b)
        {
            return Math.Abs(a - b) < 1e-6;
        }

        public void Write(BinaryDataWriter writer, float value)
        {
            writer.WriteShortFloat(value);
        }

        public float Read(BinaryDataReader reader)
        {
            return reader.ReadShortFloat();
        }

    }

    public sealed class StringWriter : IPrimitiveWriter<string>
    {
        public int GetHashCode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < value.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ value[i];
                    if (i == value.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }

        public bool Equals(string a, string b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, string value)
        {
            writer.WriteString(value);
        }

        public string Read(BinaryDataReader reader)
        {
            return reader.ReadString();
        }
    }

    public sealed class UIntWriter : IPrimitiveWriter<uint>
    {
        public int GetHashCode(uint value)
        {
            return value.GetHashCode();
        }

        public bool Equals(uint a, uint b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, uint value)
        {
            writer.WriteUInt(value);
        }

        public uint Read(BinaryDataReader reader)
        {
            return reader.ReadUInt();
        }
    }

    public sealed class ULongWriter : IPrimitiveWriter<ulong>
    {
        public int GetHashCode(ulong value)
        {
            return value.GetHashCode();
        }

        public bool Equals(ulong a, ulong b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, ulong value)
        {
            writer.WriteULong(value);
        }

        public ulong Read(BinaryDataReader reader)
        {
            return reader.ReadULong();
        }
    }

    public sealed class UShortWriter : IPrimitiveWriter<ushort>
    {
        public int GetHashCode(ushort value)
        {
            return value.GetHashCode();
        }

        public bool Equals(ushort a, ushort b)
        {
            return a == b;
        }

        public void Write(BinaryDataWriter writer, ushort value)
        {
            writer.WriteUShort(value);
        }

        public ushort Read(BinaryDataReader reader)
        {
            return reader.ReadUShort();
        }
    }
}