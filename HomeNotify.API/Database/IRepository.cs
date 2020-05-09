using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeNotify.API.Database
{
    /// <summary>
    /// A generic repository.
    /// </summary>
    /// <typeparam name="T">The type stored in the repository.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets an object from the repository.
        /// </summary>
        /// <param name="id">The id of the desired object.</param>
        /// <returns>The object.</returns>
        Task<T> Get(Guid id);

        /// <summary>
        /// Gets all objects from the repository.
        /// </summary>
        /// <returns>A list containing all objects in the repository.</returns>
        Task<IList<T>> GetAll();

        /// <summary>
        /// Inserts an object into the repository.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> Insert(T obj);

        /// <summary>
        /// Inserts multiple objects into the repository.
        /// </summary>
        /// <param name="docs">A list of objects to insert.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> BulkInsert(IList<T> docs);

        /// <summary>
        /// Updates an object in the repository.
        /// </summary>
        /// <param name="newDocs">The object to update.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> Update(T newDocs);

        /// <summary>
        /// Updates multiple objects in the repository.
        /// </summary>
        /// <param name="newObjs">A list of objects to update.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> BulkUpdate(IList<T> newObjs);

        /// <summary>
        /// Deletes an object from the repository.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> Delete(Guid id);

        /// <summary>
        /// Deletes multiple objects from the repository.
        /// </summary>
        /// <param name="ids">A list of ids to delete.</param>
        /// <returns>The result.</returns>
        Task<RepositoryResult> BulkDelete(IList<Guid> ids);
    }
}