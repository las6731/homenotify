using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeNotify.API.Services.Implementation
{
    /// <summary>
    /// Service to 
    /// </summary>
    public class MemoryTopicsService : ITopicsService
    {
        private IList<string> topics;

        private ILogger<MemoryTopicsService> logger;

        private IMessageService messageService;

        public MemoryTopicsService(ILogger<MemoryTopicsService> logger, IMessageService messageService)
        {
            this.logger = logger;
            this.messageService = messageService;
            topics = new List<string>();
        }
        
        public Task<IList<string>> getTopics()
        {
            return Task.FromResult(topics);
        }

        public async Task<bool> ensureTopic(string topic)
        {
            if (!topics.Contains(topic))
            {
                topics.Add(topic);
                logger.LogInformation($"Topic {topic} added successfully.");
                
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
            }

            return true;
        }

        public async Task<bool> removeTopic(string topic)
        {
            var result = topics.Remove(topic);

            if (result)
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
            
            logger.LogInformation($"Topic {topic} " + (result ? "removed successfully." : "unable to be removed."));

            return result;
        }
    }
}