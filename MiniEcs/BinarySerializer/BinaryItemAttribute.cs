using System;

namespace BinarySerializer
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BinaryItemAttribute : Attribute
    {
        public bool IsShort { get; }

        public BinaryItemAttribute()
        {
        }

        public BinaryItemAttribute(bool isShort)
        {
            IsShort = isShort;
        }
    }
}