using System;

namespace BinarySerializer.Data
{
    public abstract class BinaryBuffer : IDisposable
    {
        protected BufferData Buffer;

        public int Length { get; protected set; }

        protected int InnerLen;

        protected BinaryBuffer()
        {
            Buffer = new BufferData();
        }
        
        protected BinaryBuffer(BufferData bufferData, int length, int innerLen)
        {
            Buffer = bufferData;
            Length = length;
            InnerLen = innerLen;
        }
        
        protected BinaryBuffer(byte[] source)
        {
            Buffer = new BufferData();
            
            if (source == null) 
                return;
            
            Write(source);
        }
        
        protected void Write(byte[] source)
        {
            Buffer.EnsureBufferSize(InnerLen + source.Length);
            Buffer.CopyFrom(source, 0, InnerLen, source.Length);
            IncLength(source.Length);
        }
        

        protected void IncLength(int amount)
        {
            Length += amount;
            InnerLen += amount;
        }

        public virtual void Dispose()
        {            
            Length = 0;
            InnerLen = 0;
            Buffer = null;
        }
    }
}