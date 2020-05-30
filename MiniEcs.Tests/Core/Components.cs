using System;
using BinarySerializer;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    public class ComponentA : IEcsComponent, IEquatable<ComponentA>
    {
        [BinaryItem] public int Value;

        public bool Equals(ComponentA other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ComponentA) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public class ComponentB : IEcsComponent, IEquatable<ComponentB>
    {
        [BinaryItem] public int Value;

        public bool Equals(ComponentB other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ComponentB) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public class ComponentC : IEcsComponent, IEquatable<ComponentC>
    {
        [BinaryItem] public int Value;

        public bool Equals(ComponentC other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ComponentC) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public class ComponentD : IEcsComponent, IEquatable<ComponentD>
    {
        [BinaryItem] public int Value;

        public bool Equals(ComponentD other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ComponentD) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}