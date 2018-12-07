﻿using Alma.Dominio;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PermissaoMap : ClassMapping<Permissao>
    {
        public PermissaoMap()
        {
            BatchSize(15);
            Id("Id", map =>
            {
                map.Generator(Generators.Identity);
            });

            Property(x => x.Nome, map =>
            {
                map.NotNullable(true);
                map.Length(30);
                map.Index("IX_PERMISSAO_NOME");
            });

            Property(x => x.Descricao, map =>
            {
                map.NotNullable(false);
                map.Length(150);
            });

            Property(x => x.Chave, map =>
            {
                map.NotNullable(true);
                map.Length(50);
                map.Index("IX_PERMISSAO_CHAVE");
            });

            Property(x => x.Privado, map =>
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
