using System.Collections.Generic;
using System.Threading.Tasks;
using HomeNotify.API.Models;

namespace HomeNotify.API.Services
{
    public interface ITopicsService
    {
        /// <summary>
        /// Get the list of notification topics that have been defined.
        /// </summary>
        /// <returns>The list of topics.</returns>
        Task<IList<string>> getTopics();

        /// <summary>
        /// Add a new notification topic, if not already defined.
        /// </summary>
        /// <param name="topic">The notification topic.</param>
        /// <returns>A boolean indicating if the topic exists or has been added.</returns>
        Task<bool> ensureTopic(string topic);

        /// <summary>
        /// Remove a notification topic, if defined.
        /// </summary>
        /// <param name="topic">The notification topic.</param>
        /// <returns>A boolean indicating if the topic has been removed.</returns>
        Task<bool> removeTopic(string topic);
    }
}