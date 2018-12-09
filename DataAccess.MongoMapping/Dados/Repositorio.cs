using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Alma.DataAccess.MongoMapping.Dados
{
    sealed class Repositorio<TEntity> : IQueryable<TEntity>, IRepository<TEntity> where TEntity : class
    {
        private IMongoDatabase db;
        private IMongoCollection<TEntity> collection;
        private BsonClassMap map;
        private BsonMemberMap idMember;

        public Repositorio(IMongoDatabase db)
        {
            this.db = db;
            this.collection = db.GetCollection<TEntity>(ClassMappingExtensions.GetCollectionForType<TEntity>());

            if (!BsonClassMap.IsClassMapRegistered(typeof(TEntity)))
                throw new InvalidOperationException("Cannot create repository of unmapped class: " + typeof(TEntity).FullName);


            this.map = BsonClassMap<TEntity>.GetRegisteredClassMaps().FirstOrDefault();
            this.idMember = map?.IdMemberMap;
        }

        public void Evict(TEntity instance)
        {
            throw new NotSupportedException();
        } //nao usado

        public void ClearCache()
        {
            throw new NotSupportedException();
        } //nao usado

        public TEntity Get(object primaryKey)
        {

            var filter = Builders<TEntity>.Filter.Eq(idMember?.MemberName, primaryKey);
            var entity = collection.Find(filter).FirstOrDefault();

            return entity;
        }

        public TEntity GetSessionless(object primaryKey)
        {
            return Get(primaryKey);
        }

        public IList<T> ExecuteNamedQuery<T>(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public T ExecuteNamedQueryScalar<T>(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public IList<TEntity> List<TProperty>(Expression<Func<TEntity, TProperty>> property, bool orderAsc = true)
        {
            if (orderAsc)
            {
                return this.OrderBy(property).ToList();
            }
            else
            {
                return this.OrderByDescending(property).ToList();
            }
        }



        #region Implementation of IEnumerable

        public IEnumerator<TEntity> GetEnumerator()
        {
            return collection.AsQueryable<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable

        public Expression Expression
        {
            get
            {
                return collection.AsQueryable<TEntity>().Expression;
            }
        }

        public Type ElementType
        {
            get
            {
                return collection.AsQueryable<TEntity>().ElementType;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return collection.AsQueryable<TEntity>().Provider;
            }
        }

        #endregion

        #region Implementation of Transactions


        //[Transacao]
        public void Create(TEntity instance)
        {
            collection.InsertOne(instance);
        }

        //[Transacao]
        public void Create(IList<TEntity> instances)
        {
            collection.InsertMany(instances);
        }

        public async Task CreateAsync(TEntity entidade)
        {
            await collection.InsertOneAsync(entidade);
        }

        public async Task CreateAsync(IList<TEntity> entidades)
        {
            await collection.InsertManyAsync(entidades);
        }


        //[Transacao]
        public void Save(TEntity instance)
        {
            if (instance == null)
                return;
            //if (entity.Id == ObjectId.Empty)
            //    throw new ArgumentException("Id must be available and in the database to perform an update.");
            object primaryKey = idMember.Getter.Invoke(instance);

            var filter = Builders<TEntity>.Filter.Eq(idMember.MemberName, primaryKey);
            var result = collection.ReplaceOne(filter, instance);
            if (result.IsModifiedCountAvailable && result.ModifiedCount != 1)
                throw new ApplicationException($"Atualizou {result.ModifiedCount} itens.");

        }

        //[Transacao]
        public void Save(IList<TEntity> instances)
        {
            foreach (var instance in instances)
            {
                Save(instance);
            }
        }


        public async Task SaveAsync(TEntity instance)
        {
            if (instance == null)
                return;
            //if (entity.Id == ObjectId.Empty)
            //    throw new ArgumentException("Id must be available and in the database to perform an update.");
            object primaryKey = idMember.Getter.Invoke(instance);

            var filter = Builders<TEntity>.Filter.Eq(idMember.MemberName, primaryKey);
            var result = await collection.ReplaceOneAsync(filter, instance);
            if (result.IsModifiedCountAvailable && result.ModifiedCount != 1)
                throw new ApplicationException($"Atualizou {result.ModifiedCount} itens.");
        }

        public async Task SaveAsync(IList<TEntity> instances)
        {
            var tasks = instances.Select(x => SaveAsync(x));
            await Task.WhenAll(tasks);
        }



        //[Transacao]
        public void Delete(TEntity instance)
        {
            if (instance == null)
                return;
            object primaryKey = idMember.Getter.Invoke(instance);

            var filter = Builders<TEntity>.Filter.Eq(idMember.MemberName, primaryKey);
            collection.DeleteOne(filter);
        }

        //[Transacao]
        public void Delete(IList<TEntity> instances)
        {
            if (instances == null || instances.Count == 0)
                return;

            var ids = instances.Select(x => idMember.Getter.Invoke(x)).ToArray();

            var filter = Builders<TEntity>.Filter.In(idMember.MemberName, ids);
            collection.DeleteMany(filter);
        }

        public async Task DeleteAsync(TEntity instance)
        {
            if (instance == null)
                return;
            object primaryKey = idMember.Getter.Invoke(instance);

            var filter = Builders<TEntity>.Filter.Eq(idMember.MemberName, primaryKey);
            await collection.DeleteOneAsync(filter);
        }

        public async Task DeleteAsync(IList<TEntity> instances)
        {
            if (instances == null || instances.Count == 0)
                return;

            var ids = instances.Select(x => idMember.Getter.Invoke(x)).ToArray();

            var filter = Builders<TEntity>.Filter.In(idMember.MemberName, ids);
            await collection.DeleteManyAsync(filter);
        }


        //[Transacao]
        public void MergeUpdate(TEntity instance)
        {
            Save(instance);
        }

        public async Task MergeUpdateAsync(TEntity instance)
        {
            await SaveAsync(instance);
        }


        #endregion

        public IDbTransaction GetTransaction()
        {
            return null;// GetSession().Transaction
        }


        public IList<TEntity> UnProxy(IList<TEntity> list)
        {
            throw new NotSupportedException();
        }

        public TEntity UnProxy(TEntity instance)
        {
            throw new NotSupportedException();
        }

        public void ExecuteProcedure(string procName, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public void EnableFilter(string name, object value)
        {
            throw new NotSupportedException();

        }

        public void DisableFilter(string name)
        {
            throw new NotSupportedException();

        }

        public IList<TEntity> ExecuteCustomSql(string sql, IDictionary<string, object> parameters = null)
        {
            //TODO: This should be supported.
            throw new NotSupportedException();
        }

        public T ExecuteCustomSqlScalar<T>(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public Task<IList<TEntity>> ExecuteCustomSqlAsync(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public Task<T> ExecuteCustomSqlScalarAsync<T>(string sql, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public Task ExecuteProcedureAsync(string procName, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public IList<TEntity> ExecuteNamedQuery(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }

        public Task<IList<TEntity>> ExecuteNamedQueryAsync(string queryName, IDictionary<string, object> parameters = null)
        {
            throw new NotSupportedException();
        }
    }
}
