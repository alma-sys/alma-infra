using Alma.ExampleProject.Domain.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Alma.ExampleProject.DataAccess.MSSql.Mappings
{
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            BatchSize(15);
            Id("Id", map =>
            {
                map.Generator(Generators.Identity);
            });

            Property(x => x.PersonUId, map =>
            {
                map.NotNullable(true);
            });

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Length(100);
            });



            Property(x => x.FamilyName, map =>
            {
                map.Length(200);
            });

            Component(c => c.Email, cmap =>
            {
                cmap.Property(x => x.Address, map =>
                {
                    map.Column("Email");
                    map.NotNullable(true);
                    map.Length(254);
                });
            });

            Property(x => x.Address, map =>
            {
                map.Length(250);
            });

            Property(x => x.Telephone, map =>
            {
                map.Length(250);
            });

            Property(x => x.LastAccess, map =>
            {
                map.NotNullable(false);
            });

            Property(x => x.DomainUser, map =>
            {
                map.NotNullable(false);
                map.Length(50);
            });



            Bag(x => x.Roles, map =>
            {
                map.Table("UserRole");
                map.Key(key =>
                {
                    key.Column(nameof(User));
                    key.NotNullable(true);
                });
                map.Access(Accessor.Field);
                map.Inverse(false);
                map.Lazy(CollectionLazy.Extra);
            }, map =>
            {
                map.ManyToMany(p => p.Column("Role"));
            });

        }
    }
}
