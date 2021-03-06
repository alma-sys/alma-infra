﻿using NHibernate.Type;

namespace Alma.DataAccess.OrmNHibernate.Types
{
    /// <summary>
    /// Conversor de enumeração para tipos inteiro.
    /// </summary>

    public sealed class EnumInt32Type<T> : PersistentEnumType
    {
        public EnumInt32Type() : base(typeof(T)) { }
    }
}
