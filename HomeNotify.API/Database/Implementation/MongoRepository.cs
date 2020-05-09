using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HomeNotify.API.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace HomeNotify.API.Database.Implementation
{
    public abstract class MongoRepository<TModel> : IRepository<TModel> where TModel : ModelBase
    {
        protected IMongoDatabase db;

        protected IMongoCollection<TModel> Collection =>
            db.GetCollection<TModel>(((Collection) GetType().GetCustomAttribute(typeof(Collection))).Name);

        protected ILogger<MongoRepository<TModel>> logger;

        public MongoRepository(IMongoDatabase db, ILogger<MongoRepository<TModel>> logger)
        {
            this.db = db;
            this.logger = logger;
            
            EnsureIndexes();
        }

        /// <summary>
        /// Ensure that the required indexes have been defined.
        /// </summary>
        protected virtual void EnsureIndexes() {}

        public async Task<TModel> Get(Guid id)
        {
            return (await Collection.FindAsync(doc => doc.Id == id)).FirstOrDefault();
        }

        public async Task<IList<TModel>> GetAll()
        {
            return (await Collection.FindAsync(doc => true)).ToList();
        }

        public async Task<RepositoryResult> Insert(TModel obj)
        {
            try
            {
                await Collection.InsertOneAsync(obj);
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return RepositoryResult.Failure;
            }
        }

        public async Task<RepositoryResult> BulkInsert(IList<TModel> docs)
        {
            try
            {
                await Collection.InsertManyAsync(docs);
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return e.WriteConcernResult.DocumentsAffected > 0
                    ? RepositoryResult.PartialFailure
                    : RepositoryResult.Failure;
            }
            
        }

        public async Task<RepositoryResult> Update(TModel newDoc)
        {
            try
            {
                await Collection.ReplaceOneAsync(doc => doc.Id == newDoc.Id, newDoc);
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return RepositoryResult.Failure;
            }
        }

        public async Task<RepositoryResult> BulkUpdate(IList<TModel> newDocs)
        {
            try
            {
                foreach (var newDoc in newDocs)
                {
                    await Collection.ReplaceOneAsync(doc => doc.Id == newDoc.Id, newDoc);
                }
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return e.WriteConcernResult.DocumentsAffected > 0
                    ? RepositoryResult.PartialFailure
                    : RepositoryResult.Failure;
            }
        }

        public async Task<RepositoryResult> Delete(Guid id)
        {
            try
            {
                await Collection.DeleteOneAsync(doc => doc.Id == id);
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return RepositoryResult.Failure;
            }
        }

        public async Task<RepositoryResult> BulkDelete(IList<Guid> ids)
        {
            try
            {
                await Collection.DeleteManyAsync(doc => ids.Contains(doc.Id));
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return e.WriteConcernResult.DocumentsAffected > 0
                    ? RepositoryResult.PartialFailure
                    : RepositoryResult.Failure;
            }
        }
    }
}