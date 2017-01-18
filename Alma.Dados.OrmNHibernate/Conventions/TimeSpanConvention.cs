
using Alma.Dados.OrmNHibernate.Types;
using NHibernate.Mapping.ByCode;
using System;
using System.Reflection;

namespace Alma.Dados.OrmNHibernate.Conventions
{
    class TimeSpanConvention
    {

        public static void BeforeMapProperty(
            IModelInspector modelInspector, PropertyPath member,
            IPropertyMapper map)
        {
            var prop = member.LocalMember as PropertyInfo;
            if (prop.PropertyType == typeof(TimeSpan) || prop.PropertyType == typeof(TimeSpan?))
            {
                map.Type<TimeSpanFormattedType>();
                map.Length(TimeSpanFormattedType.MaxLength);
            }
        }
    }
}
