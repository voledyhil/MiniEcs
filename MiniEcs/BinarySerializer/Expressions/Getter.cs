using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BinarySerializer.Expressions
{
    public class Getter<T>
    {
        private readonly Func<object, T> _getter;
        public Getter(Type ownerType, FieldInfo field)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "obj");
            UnaryExpression newInstance = Expression.Convert(instance, ownerType);
            MemberExpression exp = Expression.Field(newInstance, field);
            UnaryExpression conversion = Expression.Convert(exp, typeof(T));
            _getter = (Func<object, T>) Expression.Lambda(conversion, instance).Compile();
        }

        public T Get(object owner)
        {
            return _getter(owner);
        }
    }
}