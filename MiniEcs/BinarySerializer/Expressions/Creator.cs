using System.Linq.Expressions;
using System.Reflection;

namespace BinarySerializer.Expressions
{
    public delegate object ObjectActivator();
    
    public class Creator
    {
        private readonly ObjectActivator _activator;
        public Creator(ConstructorInfo ctor)
        {
            ParameterInfo[] paramsInfo = ctor.GetParameters();
            Expression[] argsExp = new Expression[paramsInfo.Length];
            NewExpression newExp = Expression.New(ctor, argsExp);
            UnaryExpression conversion = Expression.Convert(newExp, typeof(object));
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), conversion);
            _activator = (ObjectActivator) lambda.Compile();
        }

        public object Create()
        {
            return _activator();
        }
    }
}