using Alma.DataAccess.MongoMapping;
using Alma.Domain;
using MongoDB.Bson.Serialization;

namespace Alma.ExampleProject.DataAccess.Mongo.Mappings
{
    public class AuthorizationMap : ClassMapping<Access>
    {
        protected override void Map(BsonClassMap<Access> map)
        {
            map.SetCollection("Authorizations");

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
