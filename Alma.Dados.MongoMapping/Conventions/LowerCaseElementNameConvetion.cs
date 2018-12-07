using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Alma.Dados.MongoMapping.Conventions
{
    class LowerCaseElementNameConvetion : IMemberMapConvention
    {
        public string Name => nameof(LowerCaseElementNameConvetion);
        public void Apply(BsonMemberMap memberMap)
        {
            memberMap.SetElementName(memberMap.ElementName.ToLower());
        }
    }
}
