using System;

namespace BinarySerializer.Data
{
    public sealed class BufferException : Exception
    {
        public BufferException(string message) : base(message)
        {
        }
    }
}