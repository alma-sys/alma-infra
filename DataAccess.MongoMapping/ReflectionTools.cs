using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Alma.DataAccess.MongoMapping
{
    internal static class ReflectionTools
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        internal static TClass CreateInstance<TClass>()
        {
            return (TClass)CreateInstance(typeof(TClass));
        }

        internal static object CreateInstance(Type type)
        {
            var ctype_ctor = type
                      .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null
                      );

            if (ctype_ctor == null)
                throw new MissingMethodException($"No parameterless constructor defined for {type.FullName}.");
            //needs parameterless constructor, can be internal
            var obj = ctype_ctor.Invoke(new object[] { });

            return obj;
        }
    }
}
