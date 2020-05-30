using System;
using System.Text;

namespace BinarySerializer.Data
{
    public class BinaryDataWriter : BinaryBuffer
    {
        private BinaryDataWriter _parentWriter;
        private readonly int _startPos;
        
        public BinaryDataWriter()
        {
        }

        private BinaryDataWriter(BinaryDataWriter parentWriter, int offset) : base(parentWriter.Buffer, 0, 0)
        {
            _parentWriter = parentWriter;
            _startPos = _parentWriter.InnerLen + offset + sizeof(ushort);
            
            InnerLen = _startPos;
            Buffer.EnsureBufferSize(InnerLen);
        }
        
        public void WriteBool(bool value)
        {
            WriteByte(Convert.ToByte(value));
        }

        public void WriteByte(byte value)
        {
            Buffer.EnsureBufferSize(InnerLen + 1);
            Buffer[InnerLen] = value;
            IncLength(1);
        }
        

        public void WriteShort(short value)
        {
            Buffer.EnsureBufferSize(InnerLen + 2);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(2);
        }

        public void WriteSByte(sbyte value)
        {
            Buffer.EnsureBufferSize(InnerLen + 1);
            Buffer[InnerLen] = unchecked((byte) value);
            IncLength(1);
        }

        public void WriteChar(char value)
        {
            Buffer.EnsureBufferSize(InnerLen + 2);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(2);
        }
        
        public void WriteUShort(ushort value)
        {
            Buffer.EnsureBufferSize(InnerLen + 2);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(2);
        }
        
        public void WriteDouble(double value)
        {
            Buffer.EnsureBufferSize(InnerLen + 8);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(8);
        }

        public void WriteInt(int value)
        {
            Buffer.EnsureBufferSize(InnerLen + 4);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(4);
        }

        public void WriteUInt(uint value)
        {
            Buffer.EnsureBufferSize(InnerLen + 4);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(4);
        }

        public void WriteFloat(float value)
        {
            Buffer.EnsureBufferSize(InnerLen + 4);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(4);
        }

        public void WriteLong(long value)
        {
            Buffer.EnsureBufferSize(InnerLen + 8);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(8);
        }

        public void WriteULong(ulong value)
        {
            Buffer.EnsureBufferSize(InnerLen + 8);
            FastBitConverter.Write(Buffer.Data, InnerLen, value);
            IncLength(8);
        }

        public void WriteShortFloat(float value)
        {
            WriteShort((short) (value * 256));
        }

        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteByte(0);
                return;
            }

            if (value.Length > byte.MaxValue)
                value = value.Substring(0, byte.MaxValue);

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteByte((byte) bytes.Length);
            Buffer.EnsureBufferSize(InnerLen + bytes.Length);
            Write(bytes);
        }

        public void CopyFrom(byte[] src, int srcOffset, int count)
        {
            Buffer.EnsureBufferSize(InnerLen + count);
            Buffer.CopyFrom(src, srcOffset, InnerLen, count);
            IncLength(count);
        }


        public BinaryDataWriter TryWriteNode(int offset)
        {
            return new BinaryDataWriter(this, offset);
        }

        public byte[] GetData()
        {
            byte[] bytes = new byte[Length];
            if (Length > 0)
                Buffer.CopyTo(bytes, _startPos, 0, Length);
            return bytes;
        }

        public void PushNode()
        {
            if (_parentWriter == null)
                return;
            
            if (Length > ushort.MaxValue)
                throw new ArgumentException($"node len '{Length}'. maximum len {ushort.MaxValue}");

            _parentWriter.WriteUShort(Convert.ToUInt16(Length));
            _parentWriter.IncLength(Length);
            _parentWriter = null;
        }

        public override void Dispose()
        {
            PushNode();

            base.Dispose();
        }
    }
}