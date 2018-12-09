using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;
using System.Data.Common;

namespace Alma.DataAccess.OrmNHibernate.Types
{
    public class TimeSpanFormattedType : TimeSpanFormattedBaseType
    {
        public const int MaxLength = 15;

        protected override string Format => "dd\\.hh\\:mm\\:ss";
    }

    public class TimeSpanHourMinuteFormattedType : TimeSpanFormattedBaseType
    {
        protected override string Format => "hh\\:mm";
    }

    public class TimeSpanHourMinuteSecondFormattedType : TimeSpanFormattedBaseType
    {
        protected override string Format => "hh\\:mm\\:ss";
    }



    public abstract class TimeSpanFormattedBaseType : IUserType
    {

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

        protected abstract string Format
        {
            get;
        }


        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var valueToGet = NHibernateUtil.String.NullSafeGet(rs, names[0], session) as string;
            TimeSpan returnValue = TimeSpan.Zero;
            if (TimeSpan.TryParse(valueToGet, out returnValue))
                return returnValue;
            else
                return (TimeSpan?)null;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            var typedValue = value as TimeSpan?;
            if (typedValue == null)
                NHibernateUtil.String.NullSafeSet(cmd, (string)null, index, session);
            else
                NHibernateUtil.String.NullSafeSet(cmd, typedValue.Value.ToString(Format), index, session);
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
                return new[] { new SqlType(DbType.String, Format.Replace("\\", "").Length) };
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
