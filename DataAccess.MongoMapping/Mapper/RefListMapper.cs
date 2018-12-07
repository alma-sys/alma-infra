using Alma.Dados.MongoMapping.Serializers;
using MongoDB.Bson.Serialization;

namespace Alma.Dados.MongoMapping
{
    public interface IRefListMapper<TClass> where TClass : class
    {
        IRefListMapper<TClass> SetElementName(string name);
        IRefListMapper<TClass> SetIsRequired(bool required);
    }
    internal class RefListMapper<TClass, TMember> : IRefListMapper<TMember> where TClass : class where TMember : class
    {
        private BsonMemberMap base_member;

        public RefListMapper(BsonMemberMap base_member)
        {
            this.base_member = base_member;
            this.base_member.SetSerializer(new DBRefListSerializer<TMember>(base_member));
        }

        public IRefListMapper<TMember> SetElementName(string name)
        {
            this.base_member.SetElementName(name);
            return this;
        }
        public IRefListMapper<TMember> SetIsRequired(bool required)
        {
            this.base_member.SetIsRequired(required);
            return this;
        }

    }
}