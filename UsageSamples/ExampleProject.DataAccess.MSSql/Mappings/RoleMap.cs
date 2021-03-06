﻿using Alma.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Alma.ExampleProject.DataAccess.MSSql.Mappings
{
    public class RoleMap : ClassMapping<Role>
    {
        public RoleMap()
        {
            BatchSize(15);
            Id("Id", map =>
            {
                map.Generator(Generators.Identity);
            });

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Length(30);
                map.Index("IX_ROLE_NAME");
            });

            Property(x => x.Description, map =>
            {
                map.NotNullable(false);
                map.Length(150);
            });

            Property(x => x.Enabled, map =>
            {
                map.Column(c =>
                {
                    c.NotNullable(true);
                    c.Default($"1");
                });
            });

            Property(x => x.Private, map =>
            {
                map.Column(c =>
                {
                    c.NotNullable(true);
                    c.Default($"1");
                });
            });



            Bag(x => x.AccessList, map =>
            {
                map.Table("RoleAccess");
                map.Key(key =>
                {
                    key.Column(nameof(Role));
                    key.NotNullable(true);
                });
                map.Access(Accessor.Field);
                map.Inverse(false);
                map.Lazy(CollectionLazy.Extra);
            }, map =>
            {
                map.ManyToMany(p => p.Column("Access"));
            });
        }
    }
}
