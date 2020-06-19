using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BinarySerializer.Expressions
{
    public delegate object ObjectActivator();
    
    public static class Expressions
    {
        public static ObjectActivator InstantiateCreator(ConstructorInfo ctor)
        {
            ParameterInfo[] paramsInfo = ctor.GetParameters();
            Expression[] argsExp = new Expression[paramsInfo.Length];
            NewExpression newExp = Expression.New(ctor, argsExp);
            UnaryExpression conversion = Expression.Convert(newExp, typeof(object));
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), conversion);
            return (ObjectActivator) lambda.Compile();
        }
        
        public static Func<object, T> InstantiateGetter<T>(Type ownerType, FieldInfo field)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "obj");
            UnaryExpression newInstance = Expression.Convert(instance, ownerType);
            MemberExpression exp = Expression.Field(newInstance, field);
            UnaryExpression conversion = Expression.Convert(exp, typeof(T));
            return (Func<object, T>) Expression.Lambda(conversion, instance).Compile();
        }
        
        public static Action<object, T> InstantiateSetter<T>(Type ownerType, FieldInfo field)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object), "obj");
            ParameterExpression argument = Expression.Parameter(typeof(T), "arg");
            UnaryExpression newInstance = Expression.Convert(instance, ownerType);
            UnaryExpression newArgument = Expression.Convert(argument, field.FieldType);
            MemberExpression exp = Expression.Field(newInstance, field);
            BinaryExpression assignExp = Expression.Assign(exp, newArgument);
            return Expression.Lambda<Action<object, T>>(assignExp, instance, argument).Compile();
        }
    }
}