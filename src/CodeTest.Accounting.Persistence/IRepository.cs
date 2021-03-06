﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CodeTest.Accounting.Domain;

namespace CodeTest.Accounting.Persistence
{
    /// <summary>
    /// Generic repository, useful for this example.
    /// In a Enterprise application, each Service can implement their own Data Access layer and methods.
    /// </summary>
    /// <typeparam name="TEntity">The Entity type to store or retrieve from the Repository.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        // note: Int32 type Ids are picked just for easiness to work with while testing
        // normally, we should be using GUIDs or more complex data types
        Task<TEntity> GetAsync(int id);

        Task<List<TEntity>> ListAllAsync();

        Task<int> SetAsync(TEntity entity);
    }
}
