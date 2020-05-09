using System.Threading.Tasks;
using HomeNotify.API.Database;
using HomeNotify.API.Models;

namespace HomeNotify.API.Repositories
{
    public interface ITopicRepository : IRepository<Topic>
    {
        /// <summary>
        /// Get the first <see cref="Topic"/> with the given name.
        /// </summary>
        /// <param name="name">The topic name.</param>
        /// <returns>The <see cref="Topic"/>.</returns>
        Task<Topic> GetTopicByName(string name);

        /// <summary>
        /// Delete the first <see cref="Topic"/> with the given name.
        /// </summary>
        /// <param name="name">The topic name.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> DeleteTopicByName(string name);
    }
}