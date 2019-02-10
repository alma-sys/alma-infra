using Alma.DataAccess.MongoMapping;
using Alma.ExampleProject.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace Alma.ExampleProject.DataAccess.Mongo.Mappings
{
    public class UserMap : ClassMapping<User>
    {
        protected override void Map(BsonClassMap<User> map)
        {
            map.SetCollection("Users");

            map.MapMember(x => x.PersonUId)
                .SetIsRequired(true);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapMember(x => x.FamilyName)
                .SetIsRequired(false);


            map.MapAsComponent(x => x.Email, cm => cm
                .SetElementName("Email")
                .SetIsRequired(true)
                .MapMember(y => y.Address)
            );

            map.MapMember(x => x.Address)
                .SetIsRequired(false);
            map.MapMember(x => x.Telephone)
                .SetIsRequired(false);
            map.MapMember(x => x.LastAccess)
                .SetIsRequired(false);
            map.MapMember(x => x.DomainUser)
                .SetIsRequired(false);

            map.MapAsRef(x => x.Creator, cm => cm
                 .SetIsRequired(false));

            map.MapListAsRefs(x => x.Roles, "_roles", cm => cm
                  .SetIsRequired(false)
            );



        }
    }
}
