using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BinarySerializer.Expressions
{
    public class Setter<T>
    {
        private readonly Action<object, T> _setter;
        public Setter(Type ownerType, FieldInfo field)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "obj");
            ParameterExpression argument = Expression.Parameter(typeof(T), "arg");
            UnaryExpression newInstance = Expression.Convert(instance, ownerType);
            UnaryExpression newArgument = Expression.Convert(argument, field.FieldType);
            MemberExpression exp = Expression.Field(newInstance, field);
            BinaryExpression assignExp = Expression.Assign(exp, newArgument);
            _setter = Expression.Lambda<Action<object, T>>(assignExp, instance, argument).Compile();
        }

        public void Set(object owner, T value)
        {
            _setter(owner, value);
        }
    }
}