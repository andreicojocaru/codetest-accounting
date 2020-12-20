using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.Domain;

namespace CodeTest.Accounting.Persistence
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity>
        where TEntity : IEntity
    {
        private readonly IDictionary<int, TEntity> _entities;

        public InMemoryRepository()
        {
            _entities = new Dictionary<int, TEntity>();
        }

        public Task<TEntity> GetAsync(int id)
        {
            if (_entities.ContainsKey(id))
            {
                return Task.FromResult(_entities[id]);
            }

            return Task.FromResult<TEntity>(default);
        }

        public Task<List<TEntity>> ListAllAsync()
        {
            return Task.FromResult(_entities.Values.ToList());
        }

        public Task<int> SetAsync(TEntity entity)
        {
            var last = _entities.Keys.LastOrDefault();
            var id = last + 1;

            entity.Id = id;
            _entities.Add(id, entity);

            return Task.FromResult(id);
        }
    }
}
