using System.Threading.Tasks;
using HomeNotify.API.Database;
using HomeNotify.API.Database.Implementation;
using HomeNotify.API.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace HomeNotify.API.Repositories.Implementation
{
    [Collection("Topics")]
    public class MongoTopicRepository : MongoRepository<Topic>, ITopicRepository
    {
        public MongoTopicRepository(IMongoDatabase db, ILogger<MongoTopicRepository> logger) : base(db, logger) {}

        protected override void EnsureIndexes()
        {
            var indexKeys = Builders<Topic>.IndexKeys.Ascending(_ => _.Name);
            var indexModel = new CreateIndexModel<Topic>(indexKeys);
            Collection.Indexes.CreateOneAsync(indexModel);
        }
        
        public async Task<Topic> GetTopicByName(string name)
        {
            return (await Collection.FindAsync(topic => topic.Name == name)).FirstOrDefault();
        }

        public async Task<RepositoryResult> DeleteTopicByName(string name)
        {
            try
            {
                await Collection.DeleteOneAsync(doc => doc.Name == name);
                return RepositoryResult.Success;
            }
            catch (MongoWriteConcernException e)
            {
                logger.LogError(e, e.WriteConcernResult.LastErrorMessage);
                return RepositoryResult.Failure;
            }
        }
    }
}