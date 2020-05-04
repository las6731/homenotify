using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace HomeNotify.API.Services.Implementation
{
    public class FirebaseMessageService : IMessageService
    {
        private readonly FirebaseMessaging firebaseMessaging;
        private readonly ILogger<FirebaseMessageService> logger;

        public FirebaseMessageService(ILogger<FirebaseMessageService> logger, FirebaseMessaging firebaseMessaging)
        {
            this.firebaseMessaging = firebaseMessaging;
            this.logger = logger;
        }
        
        public async Task<string> SendMessage(Message message)
        {
            var result = await firebaseMessaging.SendAsync(message);
            logger.LogInformation($"Message {result} sent successfully.");
            return result;
        }
    }
}