﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Common
{
    static class ReflectionExtensions
    {
        public static bool IsGenericTypeOf(this Type t, Type genericDefinition)
        {
            Type[] parameters = null;
            return IsGenericTypeOf(t, genericDefinition, out parameters);
        }

        public static bool IsGenericTypeOf(this Type t, Type genericDefinition, out Type[] genericParameters)
        {
            genericParameters = new Type[] { };
            if (!genericDefinition.IsGenericType)
            {
                return false;
            }

            var isMatch = t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
            if (!isMatch && t.BaseType != null)
            {
                isMatch = IsGenericTypeOf(t.BaseType, genericDefinition, out genericParameters);
            }
            if (!isMatch && genericDefinition.IsInterface && t.GetInterfaces().Any())
            {
                foreach (var i in t.GetInterfaces())
                {
                    if (i.IsGenericTypeOf(genericDefinition, out genericParameters))
                    {
                        isMatch = true;
                        break;
                    }
                }
            }

            if (isMatch && !genericParameters.Any())
            {
                genericParameters = t.GetGenericArguments();
            }
            return isMatch;
        }


        public static Type[] GetGenericInterfaces(this Type t, Type genericDefinition)
        {
            if (t == null || genericDefinition == null)
            {
                return new Type[] { };
            }
            var ifaces = t.GetInterfaces();
            var list = new List<Type>();

            var result =
                ifaces.Where(i => i.IsGenericTypeOf(genericDefinition)).ToArray();

            return result;
        }
    }
}
