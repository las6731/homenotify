using System;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using HomeNotify.API.Models;
using HomeNotify.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeNotify.API.Controllers
{
    [ApiController]
    [Route("notification")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> logger;
        private readonly IMessageService messageService;
        private readonly ITopicsService topicsService;

        public NotificationController(ILogger<NotificationController> logger, IMessageService messageService, ITopicsService topicsService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.topicsService = topicsService;
        }
        
        [HttpPost("token/{token}")]
        public async Task<string> SendMessageByToken([FromRoute]string token, [FromBody]NotificationMessage message)
        {
            logger.LogInformation($"Token: {token}");
            logger.LogInformation($"Message: {JsonConvert.SerializeObject(message)}");
            
            return await messageService.SendMessage(new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = message.Title,
                    Body = message.Body
                }
            });
        }

        [HttpPost("topic/{topic}")]
        public async Task<string> SendMessageByTopic([FromRoute] string topic, [FromBody] NotificationMessage message)
        {
            logger.LogInformation($"Topic: {topic}");
            logger.LogInformation($"Message: {JsonConvert.SerializeObject(message)}");

            await topicsService.ensureTopic(topic);
            return await messageService.SendMessage(new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Title = message.Title,
                    Body = message.Body
                }
            });
        }
    }
}