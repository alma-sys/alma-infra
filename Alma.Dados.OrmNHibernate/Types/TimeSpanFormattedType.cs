using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Alma.Dados.OrmNHibernate.Types
{
    public class TimeSpanFormattedType : IUserType
    {
        public const int MaxLength = 5;
        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var valueToGet = NHibernateUtil.String.NullSafeGet(rs, names[0]) as string;
            TimeSpan returnValue = TimeSpan.Zero;
            if (TimeSpan.TryParse(valueToGet, out returnValue))
                return returnValue;
            else
                return (TimeSpan?)null;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var typedValue = value as TimeSpan?;
            if (typedValue == null)
                NHibernateUtil.String.NullSafeSet(cmd, (string)null, index);
            else
                NHibernateUtil.String.NullSafeSet(cmd, typedValue.Value.ToString("hh:mm:ss"), index);
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return DeepCopy(cached);
        }

        public object Disassemble(object value)
        {
            return DeepCopy(value);
        }

        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { new SqlType(DbType.String) };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(TimeSpan); }
        }

        public bool IsMutable
        {
            get { return true; }
        }
    }
}
