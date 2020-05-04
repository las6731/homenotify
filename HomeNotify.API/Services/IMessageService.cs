using System.Threading.Tasks;
using FirebaseAdmin.Messaging;

namespace HomeNotify.API.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string id of the message.</returns>
        Task<string> SendMessage(Message message);
    }
}