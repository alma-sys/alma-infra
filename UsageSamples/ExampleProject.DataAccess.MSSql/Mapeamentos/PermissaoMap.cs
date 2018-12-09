using Alma.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PermissaoMap : ClassMapping<Access>
    {
        public PermissaoMap()
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
                map.Index("IX_PERMISSAO_NOME");
            });

            Property(x => x.Description, map =>
            {
                map.NotNullable(false);
                map.Length(150);
            });

            Property(x => x.Key, map =>
            {
                map.NotNullable(true);
                map.Length(50);
                map.Index("IX_PERMISSAO_CHAVE");
            });

            Property(x => x.Private, map =>
            {
                map.Column(c =>
                {
                    c.NotNullable(true);
                    c.Default($"1");
                });
            });

        }
    }
}
