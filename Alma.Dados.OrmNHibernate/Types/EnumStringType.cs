using System;
using System.Data;

namespace Alma.Dados.OrmNHibernate.Types
{
    /// <summary>
    /// Conversor de Enum para String
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EnumStringType<T> : NHibernate.Type.EnumStringType
    {
        public EnumStringType()
            : base(typeof(T))
        { }
        public override void Set(IDbCommand cmd, object value, int index)
        {
            IDataParameter par = (IDataParameter)cmd.Parameters[index];
            if (value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                par.Value = Convert.ToString((T)value);
            }
        }

        public override object Get(IDataReader rs, int index)
        {
            object code = rs[index];
            if (code == DBNull.Value || code == null)
            {
                return null;
            }
            else
            {
                return (T)(object)code.ToString();
            }
        }
    }
}
