using Alma.DataAccess.MongoMapping.Serializers;
using MongoDB.Bson.Serialization;

namespace Alma.DataAccess.MongoMapping
{
    public interface IRefMapper<TClass> where TClass : class
    {
        IRefMapper<TClass> SetElementName(string name);
        IRefMapper<TClass> SetIsRequired(bool required);
    }
    internal class RefMapper<TClass, TMember> : IRefMapper<TMember> where TClass : class where TMember : class
    {
        private BsonMemberMap base_member;

        public RefMapper(BsonMemberMap base_member)
        {
            this.base_member = base_member;
            this.base_member.SetSerializer(new DBRefSerializer<TMember>(base_member));
        }

        public IRefMapper<TMember> SetElementName(string name)
        {
            this.base_member.SetElementName(name);
            return this;
        }
        public IRefMapper<TMember> SetIsRequired(bool required)
        {
            this.base_member.SetIsRequired(required);
            return this;
        }

    }
}