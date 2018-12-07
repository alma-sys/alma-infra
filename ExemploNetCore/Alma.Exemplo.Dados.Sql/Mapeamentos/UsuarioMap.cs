using Alma.Exemplo.Dominio.Entidades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class UsuarioMap : ClassMapping<Usuario>
    {
        public UsuarioMap()
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

        

            Property(x => x.Sobrenome, map =>
            {
                map.Length(200);
            });

            Component(c => c.Email, cmap =>
            {
                cmap.Property(x => x.Endereco, map =>
                {
                    map.Column("Email");
                    map.NotNullable(true);
                    map.Length(254);
                });
            });

            Property(x => x.Endereco, map =>
            {
                map.Length(250);
            });

            Property(x => x.Telefone, map =>
            {
                map.Length(250);
            });

            Property(x => x.UltimoAcesso, map =>
            {
                map.NotNullable(false);
            });

            Property(x => x.DomainUser, map =>
            {
                map.NotNullable(false);
                map.Length(50);
            });



            Bag(x => x.Perfis, map =>
            {
                map.Table("UsuarioPerfil");
                map.Key(key =>
                {
                    key.Column(nameof(Usuario));
                    key.NotNullable(true);
                });
                map.Access(Accessor.Field);
                map.Inverse(false);
                map.Lazy(CollectionLazy.Extra);
            }, map =>
            {
                map.ManyToMany(p => p.Column("Perfil"));
            });

        }
    }
}
