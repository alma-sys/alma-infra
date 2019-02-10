using Alma.DataAccess.MongoMapping;
using Alma.Domain;
using MongoDB.Bson.Serialization;

namespace Alma.ExampleProject.DataAccess.Mongo.Mappings
{
    public class RoleMap : ClassMapping<Role>
    {
        protected override void Map(BsonClassMap<Role> map)
        {
            map.SetCollection("Roles");

            map.MapMember(x => x.Enabled)
                .SetIsRequired(true);

            map.MapMember(x => x.Description)
                .SetIsRequired(false);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapListAsRefs(x => x.AccessList, "_authorizations", cm => cm
                .SetIsRequired(true)
                .SetElementName("AccessList")
            );

            map.MapMember(x => x.Private)
                .SetIsRequired(true);
        }
    }
}
