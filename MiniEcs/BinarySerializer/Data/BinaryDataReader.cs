using System;

namespace BinarySerializer.Data
{
    public class BinaryDataReader : BinaryBuffer
    {
        public int Position { get; private set; }

        private int _innerPosition;
        public BinaryDataReader(byte[] source, int offset = 0) : base(source)
        {
            _innerPosition = Position = offset;
            
            if (source != null)
                InnerLen = Length = source.Length;
        }

        private BinaryDataReader(BufferData bufferData, int innerPosition, int length, int innerLen) : base(bufferData, length, innerLen)
        {
            _innerPosition = innerPosition;
        }

        public bool ReadBool()
        {
            return Convert.ToBoolean(ReadByte());
        }

        public byte ReadByte()
        {
            if (!(InnerLen - _innerPosition >= 1))
                throw new BufferException("Trying to read past the buffer size");
            byte retval = Buffer[_innerPosition];
            _innerPosition++;
            Position++;
            return retval;
        }
        
        public char ReadChar()
        {
            if (!(InnerLen - _innerPosition >= 2))
                throw new BufferException("Trying to read past the buffer size");
            char retval = Buffer.ToChar(_innerPosition);
            _innerPosition += 2;
            Position += 2;
            return retval;    
        }

        public short ReadShort()
        {
            if (!(InnerLen - _innerPosition >= 2))
                throw new BufferException("Trying to read past the buffer size");
            short retval = Buffer.ToInt16(_innerPosition);
            _innerPosition += 2;
            Position += 2;
            return retval;
        }

        public sbyte ReadSByte()
        {
            if (!(InnerLen - _innerPosition >= 1))
                throw new BufferException("Trying to read past the buffer size");
            sbyte retval = unchecked((sbyte)Buffer[_innerPosition]);
            _innerPosition++;
            Position++;
            return retval;
        }

        public ushort ReadUShort()
        {
            if (!(InnerLen - _innerPosition >= 2))
                throw new BufferException("Trying to read past the buffer size");
            ushort retval = Buffer.ToUInt16(_innerPosition);
            _innerPosition += 2;
            Position += 2;
            return retval;
        }

        public double ReadDouble()
        {
            if (!(InnerLen - _innerPosition >= 8))
                throw new BufferException("Trying to read past the buffer size");
            double retval = Buffer.ToDouble(_innerPosition);
            _innerPosition += 8;
            Position += 8;
            return retval;
        }

        public int ReadInt()
        {
            return (int) ReadUInt();
        }

        public uint ReadUInt()
        {
            if (!(InnerLen - _innerPosition >= 4))
                throw new BufferException("Trying to read past the buffer size");
            uint retval = Buffer.ToUInt32(_innerPosition);
            _innerPosition += 4;
            Position += 4;
            return retval;
        }
        
        
        public long ReadLong()
        {
            return (long) ReadULong();
        }

        public ulong ReadULong()
        {
            if (!(InnerLen - _innerPosition >= 8))
                throw new BufferException("Trying to read past the buffer size");
            ulong retval = Buffer.ToUInt64(_innerPosition);
            _innerPosition += 8;
            Position += 8;
            return retval;
        }

        public float ReadShortFloat()
        {
            if (!(InnerLen - _innerPosition >= 2))
                throw new BufferException("Trying to read past the buffer size");
            short retval = Buffer.ToInt16(_innerPosition);
            _innerPosition += 2;
            Position += 2;
            return retval / 256f;
        }

        public float ReadFloat()
        {
            if (!(InnerLen - _innerPosition >= 4))
                throw new BufferException("Trying to read past the buffer size");
            float retval = Buffer.ToSingle(_innerPosition);
            _innerPosition += 4;
            Position += 4;
            return retval;
        }

        public string ReadString()
        {
            byte byteLen = ReadByte();
            if (byteLen <= 0)
                return null;

            if (InnerLen - _innerPosition < byteLen)
                throw new BufferException("Trying to read past the buffer size");

            string retval = Buffer.ToString(_innerPosition, byteLen);
            _innerPosition += byteLen;
            Position += byteLen;
            return retval;
        }


        public void CopyTo(byte[] dst, int srcOffset, int dstOffset, int count)
        {
            Buffer.CopyTo(dst, srcOffset, dstOffset, count);
        }

        public byte[] GetRemainingBytes()
        {
            int len = Length - Position;
            byte[] outgoingData = new byte[len];
            Buffer.CopyTo(outgoingData, _innerPosition, 0, len);
            Position = Length;
            return outgoingData;
        }

        public BinaryDataReader ReadNode()
        {
            ushort byteLen = ReadUShort();            
            if (byteLen == 0)
                return new BinaryDataReader(Buffer, _innerPosition, 0, InnerLen);

            if (InnerLen - _innerPosition < byteLen)
                throw new BufferException("Trying to read past the buffer size");

            int pos = _innerPosition;
            
            _innerPosition += byteLen;
            Position += byteLen;
            
            return new BinaryDataReader(Buffer, pos, byteLen, InnerLen);
        }

        public override void Dispose()
        {
            base.Dispose();

            _innerPosition = 0;
            Position = 0;
        }
    }
}