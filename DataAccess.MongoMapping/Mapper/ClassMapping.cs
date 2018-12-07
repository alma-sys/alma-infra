using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Alma.Dados.MongoMapping
{
    public abstract class ClassMapping<T> where T : class
    {

        public ClassMapping()
        {
            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                this.Map(cm);
            });
        }

        protected abstract void Map(BsonClassMap<T> map);
    }

    public static class ClassMappingExtensions
    {
        public static void MapAsComponent<TClass, TMember>(this BsonClassMap<TClass> map, Expression<Func<TClass, TMember>> property, Action<IComponentMapper<TMember>> mapping) where TClass : class where TMember : class
        {
            var base_member = map.MapMember(property);
            mapping(new ComponentMapper<TClass, TMember>(base_member));
        }

        public static void MapListAsRefs<TClass, TMember>(this BsonClassMap<TClass> map, Expression<Func<TClass, TMember>> property, Action<IRefListMapper<TMember>> mapping) where TClass : class where TMember : class
        {
            var base_member = map.MapMember(property);
            mapping(new RefListMapper<TClass, TMember>(base_member));
        }

        public static void MapListAsRefs<TClass, TMember>(this BsonClassMap<TClass> map, Expression<Func<TClass, TMember>> property, string acessor, Action<IRefListMapper<TMember>> mapping) where TClass : class where TMember : class
        {
            var base_member = map.MapField(acessor);
            base_member.SetElementName(ReflectionTools.GetPropertyInfo(property).Name);
            mapping(new RefListMapper<TClass, TMember>(base_member));
        }

        public static void MapAsRef<TClass, TMember>(this BsonClassMap<TClass> map, Expression<Func<TClass, TMember>> property, Action<IRefMapper<TMember>> mapping) where TClass : class where TMember : class
        {
            var base_member = map.MapMember(property);
            mapping(new RefMapper<TClass, TMember>(base_member));
        }

        public static void MapAsRef<TClass, TMember>(this BsonClassMap<TClass> map, Expression<Func<TClass, TMember>> property, string acessor, Action<IRefMapper<TMember>> mapping) where TClass : class where TMember : class
        {
            var base_member = map.MapField(acessor);
            base_member.SetElementName(ReflectionTools.GetPropertyInfo(property).Name);
            mapping(new RefMapper<TClass, TMember>(base_member));
        }

        public static void SetCollection<TClass>(this BsonClassMap<TClass> map, string name)
        {
            Collections.Add(typeof(TClass), name);
        }

        private static IDictionary<Type, string> Collections = new Dictionary<Type, string>();
        internal static string GetCollectionForType<TClass>()
        {
            return GetCollectionForType(typeof(TClass));
        }

        internal static string GetCollectionForType(Type type)
        {
            if (Collections.ContainsKey(type))
                return Collections[type];
            else
                return type.Name;
        }

    }
}
