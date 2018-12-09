using Alma.DataAccess.MongoMapping;
using Alma.Domain;
using MongoDB.Bson.Serialization;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class PermissaoMap : ClassMapping<Access>
    {
        protected override void Map(BsonClassMap<Access> map)
        {
            map.SetCollection("Permissoes");

            map.MapMember(x => x.Key)
                .SetIsRequired(true);

            map.MapMember(x => x.Description)
                .SetIsRequired(false);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapMember(x => x.Private)
                .SetIsRequired(true);

        }
    }
}
