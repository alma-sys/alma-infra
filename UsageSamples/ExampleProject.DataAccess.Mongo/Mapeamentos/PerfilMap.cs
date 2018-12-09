using Alma.DataAccess.MongoMapping;
using Alma.Domain;
using MongoDB.Bson.Serialization;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PerfilMap : ClassMapping<Role>
    {
        protected override void Map(BsonClassMap<Role> map)
        {
            map.SetCollection("Perfis");

            map.MapMember(x => x.Enabled)
                .SetIsRequired(true);

            map.MapMember(x => x.Description)
                .SetIsRequired(false);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapListAsRefs(x => x.AccessList, "_permissoes", cm => cm
                .SetIsRequired(true)
                .SetElementName("Permissoes")
            );

            map.MapMember(x => x.Private)
                .SetIsRequired(true);
        }
    }
}
