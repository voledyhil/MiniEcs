using System;
using System.Collections.Generic;

namespace BinarySerializer.Properties
{
    internal interface IProperty
    {
        
    }
    
    public class Property<T> : IProperty, IEquatable<Property<T>>
    {
        public int Version => _version;

        private int _version;
        public void Update(T value)
        {
            Value = value;
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _version++;
            }
        }

        public bool Equals(Property<T> other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            if (ReferenceEquals(this, other)) 
                return true;
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            return obj is Property<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(_value);
        }
    }
}