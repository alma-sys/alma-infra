using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace Alma.Dados.OrmNHibernate.Mapper
{
    public abstract class GlobalFilterMapping
    {

        internal IDictionary<string, FilterDefinition> filters = new Dictionary<string, FilterDefinition>();

        protected void RegisterFilter<T>(string name) where T : IConvertible
        {
            filters.Add(
                name,
                new FilterDefinition(
                    name, null,
                    new Dictionary<string, IType>()
                    {
                        { name, MapearTipo(typeof(T)) }
                    },
                    false)
                );
        }

        private IType MapearTipo(Type type)
        {
            if (typeof(int) == type)
                return NHibernate.NHibernateUtil.Int32;
            else if (typeof(long) == type)
                return NHibernate.NHibernateUtil.Int64;
            else if (typeof(short) == type)
                return NHibernate.NHibernateUtil.Int16;
            else if (typeof(bool) == type)
                return NHibernate.NHibernateUtil.Boolean;
            else if (typeof(decimal) == type)
                return NHibernate.NHibernateUtil.Decimal;
            else if (typeof(double) == type)
                return NHibernate.NHibernateUtil.Double;
            else if (typeof(float) == type)
                return NHibernate.NHibernateUtil.Decimal;
            else if (typeof(string) == type)
                return NHibernate.NHibernateUtil.AnsiString;
            else
                return NHibernate.NHibernateUtil.GuessType(type);
        }
    }
}
