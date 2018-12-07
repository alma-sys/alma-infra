using Alma.Dominio;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PerfilMap : ClassMapping<Perfil>
    {
        public PerfilMap()
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
                map.Index("IX_PERFIL_NOME");
            });

            Property(x => x.Descricao, map =>
            {
                map.NotNullable(false);
                map.Length(150);
            });

            Property(x => x.Ativo, map =>
            {
                map.Column(c =>
                {
                    c.NotNullable(true);
                    c.Default($"1");
                });
            });

            Property(x => x.Privado, map =>
            {
                map.Column(c =>
                {
                    c.NotNullable(true);
                    c.Default($"1");
                });
            });



            Bag(x => x.Permissoes, map =>
            {
                map.Table("PerfilPermissao");
                map.Key(key =>
                {
                    key.Column(nameof(Perfil));
                    key.NotNullable(true);
                });
                map.Access(Accessor.Field);
                map.Inverse(false);
                map.Lazy(CollectionLazy.Extra);
            }, map =>
            {
                map.ManyToMany(p => p.Column("Permissao"));
            });
        }
    }
}
