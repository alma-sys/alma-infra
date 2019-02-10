using NHibernate.Engine;
using System;
using System.Data;
using System.Data.Common;

namespace Alma.DataAccess.OrmNHibernate.Types
{
    public class EnumCapsStringType<T> : NHibernate.Type.EnumStringType<T>
    {
        public EnumCapsStringType()
            : base()
        {
            var type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];
            if (!type.IsEnum)
                throw new InvalidOperationException("This type only supports enums and nullable enums");

        }

        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            IDataParameter par = cmd.Parameters[index];
            if (value == null || !Enum.IsDefined(typeof(T), value))
            {
                par.Value = DBNull.Value;
            }
            else
            {
                par.Value = ((T)value).ToString().ToUpper();
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
                return Enum.Parse(typeof(T), code.ToString(), true);
            }
        }
    }
}
