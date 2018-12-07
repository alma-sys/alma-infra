using NHibernate.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Alma.Dados.OrmNHibernate.Types
{
    /// <summary>
    /// Conversor de Enum para String
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EnumStringType<T> : NHibernate.Type.EnumStringType
    {
        private readonly Dictionary<T, object> attributes;
        private readonly Dictionary<object, T> values;

        public EnumStringType()
            : base(typeof(T))
        {
            var type = typeof(T);
            if (type.IsGenericType)
                type = type.GetGenericArguments()[0];
            if (!type.IsEnum)
                throw new InvalidOperationException("This type only supports enums and nullable enums");

            this.attributes = type.GetEnumValues()
                .Cast<T>()
                .ToDictionary(key => key, value => ToAmbientValue(value));

            this.values = type.GetEnumValues()
                .Cast<T>()
                .ToDictionary(key => ToAmbientValue(key), value => value);
        }

        private static object ToAmbientValue(T enumeration)
        {
            var attributes = (AmbientValueAttribute[])enumeration
                .GetType().GetTypeInfo()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(typeof(AmbientValueAttribute), false);

            return (attributes.Length > 0)
                ? attributes[0].Value
                : enumeration.ToString();
        }
        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            IDataParameter par = cmd.Parameters[index];
            if (value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                T vl = (T)value;
                par.Value = attributes[vl];
            }
        }

        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            object code = rs[index];
            if (code == DBNull.Value || code == null)
            {
                return null;
            }
            else
            {
                if (values.ContainsKey(code))
                    return values[code];
                else
                    throw new InvalidOperationException($"Invalid data from the storage: {code}");
            }
        }
    }
}
