using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using HomeNotify.API.Database;
using HomeNotify.API.Models;
using HomeNotify.API.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeNotify.API.Services.Implementation
{
    /// <summary>
    /// Service to access <see cref="Topic"/>.
    /// </summary>
    public class TopicsService : ITopicsService
    {
        private readonly ILogger<TopicsService> logger;

        private readonly ITopicRepository repository;

        private readonly IMessageService messageService;

        public TopicsService(ILogger<TopicsService> logger, ITopicRepository repository, IMessageService messageService)
        {
            this.logger = logger;
            this.repository = repository;
            this.messageService = messageService;
        }
        
        public async Task<IList<string>> getTopics()
        {
            return (await repository.GetAll()).Select(topic => topic.Name).ToList();
        }

        public async Task<bool> ensureTopic(string topic)
        {
            var existingTopic = await repository.GetTopicByName(topic);

            if (existingTopic != null) return true;
            
            var result = await repository.Insert(new Topic()
            {
                Name = topic
            });

            if (result == RepositoryResult.Failure) return false;
            
            var data = new List<string>
            {
                {topic}
            };
            await messageService.SendMessage(new Message
            {
                Topic = "topics",
                Data = new Dictionary<string, string>
                {
                    {"topics", JsonConvert.SerializeObject(data)}
                }
            });

            return true;
        }

        public async Task<bool> removeTopic(string topic)
        {
            var result = await repository.DeleteTopicByName(topic);

            if (result == RepositoryResult.Success)
            {
                await messageService.SendMessage(new Message
                {
                    Topic = "topics",
                    Data = new Dictionary<string, string>
                    {
                        {"unsubscribeTopic", topic}
                    }
                });
            }
            
            logger.LogInformation($"Topic {topic} " + (result.IsSuccess() ? "removed successfully." : "unable to be removed."));

            return result.IsSuccess();
        }
    }
}