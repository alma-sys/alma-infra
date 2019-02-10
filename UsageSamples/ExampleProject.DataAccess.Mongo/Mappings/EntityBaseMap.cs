using Alma.DataAccess.MongoMapping;
using Alma.DataAccess.MongoMapping.Generators;
using Alma.Domain;
using MongoDB.Bson.Serialization;

namespace Alma.ExampleProject.DataAccess.Mongo.Mappings
{
    class EntityBaseMap : ClassMapping<Entity<long>>
    {
        protected override void Map(BsonClassMap<Entity<long>> map)
        {
            map.MapIdMember(x => x.Id)
                .SetIdGenerator(new Int64IdGenerator())
                .SetElementName("_id")
                ;
            map.SetIsRootClass(true);
        }
    }
}
