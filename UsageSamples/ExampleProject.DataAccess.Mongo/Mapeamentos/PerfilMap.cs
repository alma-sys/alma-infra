using Alma.Dados.MongoMapping;
using Alma.Dominio;
using MongoDB.Bson.Serialization;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PerfilMap : ClassMapping<Perfil>
    {
        protected override void Map(BsonClassMap<Perfil> map)
        {
            map.SetCollection("Perfis");

            map.MapMember(x => x.Ativo)
                .SetIsRequired(true);

            map.MapMember(x => x.Descricao)
                .SetIsRequired(false);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapListAsRefs(x => x.Permissoes, "_permissoes", cm => cm
                .SetIsRequired(true)
                .SetElementName("Permissoes")
            );

            map.MapMember(x => x.Privado)
                .SetIsRequired(true);
        }
    }
}
