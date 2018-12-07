using Alma.Dados.MongoMapping;
using Alma.Dados.MongoMapping.Generators;
using Alma.Dominio;
using MongoDB.Bson.Serialization;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    class EntidadeMap : ClassMapping<Entidade<long>>
    {
        protected override void Map(BsonClassMap<Entidade<long>> map)
        {
            map.MapIdMember(x => x.Id)
                .SetIdGenerator(new Int64IdGenerator())
                .SetElementName("_id")
                ;
            map.SetIsRootClass(true);
        }
    }
}
