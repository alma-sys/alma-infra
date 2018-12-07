using Alma.Dados.MongoMapping.Serializers;
using MongoDB.Bson.Serialization;
using System;
using System.Linq.Expressions;

namespace Alma.Dados.MongoMapping
{
    public interface IComponentMapper<TClass> where TClass : class
    {
        IComponentMapper<TClass> MapMember<TMember>(Expression<Func<TClass, TMember>> property);

        IComponentMapper<TClass> SetElementName(string name);
        IComponentMapper<TClass> SetIsRequired(bool required);
    }
    class ComponentMapper<TRootClass, TClass> : IComponentMapper<TClass> where TRootClass : class where TClass : class
    {
        private BsonMemberMap base_member;

        public ComponentMapper(BsonMemberMap base_member)
        {
            this.base_member = base_member;
        }

        public IComponentMapper<TClass> MapMember<TMember>(Expression<Func<TClass, TMember>> property)
        {
            base_member.SetSerializer(new ComponentSerializer<TClass, TMember>(property));
            return this;
        }

        public IComponentMapper<TClass> SetElementName(string name)
        {
            this.base_member.SetElementName(name);
            return this;
        }
        public IComponentMapper<TClass> SetIsRequired(bool required)
        {
            this.base_member.SetIsRequired(required);
            return this;
        }
    }

   
}
