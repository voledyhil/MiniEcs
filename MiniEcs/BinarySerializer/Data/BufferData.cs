using System;
using System.Text;

namespace BinarySerializer.Data
{
    public class BufferData
    {
        public byte[] Data => _data;
        
        public byte this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }

        private const int AllocSize = 256;
        private byte[] _data;

        public void EnsureBufferSize(int newLen)
        {
            if (_data == null)
                _data = new byte[AllocSize];
            
            if (newLen > _data.Length)
                Array.Resize(ref _data, AllocSize + newLen);
        }

        public void CopyFrom(byte[] src, int srcOffset, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, _data, dstOffset, count);
        }

        public void CopyTo(byte[] dst, int srcOffset, int dstOffset, int count)
        {
            Buffer.BlockCopy(_data, srcOffset, dst, dstOffset, count);
        }

        public char ToChar(int position)
        {
            return BitConverter.ToChar(_data, position);
        }

        public ushort ToUInt16(int position)
        {
            return BitConverter.ToUInt16(_data, position);
        }
        
        public short ToInt16(int position)
        {
            return BitConverter.ToInt16(_data, position);
        }
        
        public double ToDouble(int position)
        {
            return BitConverter.ToDouble(_data, position);
        }
        
        public float ToSingle(int position)
        {
            return BitConverter.ToSingle(_data, position);
        }

        public uint ToUInt32(int position)
        {
            return BitConverter.ToUInt32(_data, position);
        }

        public ulong ToUInt64(int position)
        {
            return BitConverter.ToUInt64(_data, position);
        }

        public string ToString(int position, int len)
        {
            return Encoding.UTF8.GetString(_data, position, len);
        }
    }
}