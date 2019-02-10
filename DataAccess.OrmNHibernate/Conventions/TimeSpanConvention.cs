
using Alma.DataAccess.OrmNHibernate.Types;
using NHibernate.Mapping.ByCode;
using System;
using System.Reflection;

namespace Alma.DataAccess.OrmNHibernate.Conventions
{
    class TimeSpanConvention
    {

        public static void BeforeMapProperty(
            IModelInspector modelInspector, PropertyPath member,
            IPropertyMapper map)
        {
            var prop = member.LocalMember as PropertyInfo;
            var field = member.LocalMember as FieldInfo;
            if (
                prop?.PropertyType == typeof(TimeSpan) ||
                prop?.PropertyType == typeof(TimeSpan?) ||
                field?.FieldType == typeof(TimeSpan) ||
                field?.FieldType == typeof(TimeSpan?)
                )
            {
                map.Type<TimeSpanFormattedType>();
                map.Length(TimeSpanFormattedType.MaxLength);
            }
        }
    }
}
