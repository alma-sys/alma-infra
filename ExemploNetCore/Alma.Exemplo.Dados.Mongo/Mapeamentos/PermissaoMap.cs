using Alma.Dados.MongoMapping;
using Alma.Dominio;
using MongoDB.Bson.Serialization;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PermissaoMap : ClassMapping<Permissao>
    {
        protected override void Map(BsonClassMap<Permissao> map)
        {
            map.SetCollection("Permissoes");

            map.MapMember(x => x.Chave)
                .SetIsRequired(true);

            map.MapMember(x => x.Descricao)
                .SetIsRequired(false);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapMember(x => x.Privado)
                .SetIsRequired(true);

        }
    }
}
