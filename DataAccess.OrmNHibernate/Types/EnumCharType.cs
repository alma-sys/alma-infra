﻿using NHibernate.Engine;
using System;
using System.Data;
using System.Data.Common;

namespace Alma.DataAccess.OrmNHibernate.Types
{
    /// <summary>
    /// Conversor de Enum para VARCHAR(1)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EnumCharType<T> : NHibernate.Type.EnumStringType
    {
        public EnumCharType()

            : base(typeof(T))

        { }

        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            IDataParameter par = cmd.Parameters[index];
            if (value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                par.Value = Convert.ToChar((T)value);
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
                return (T)((object)(int)char.Parse(code.ToString()));
            }
        }
    }
}