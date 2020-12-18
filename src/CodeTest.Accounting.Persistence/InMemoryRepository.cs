using System.Collections.Generic;
using System.Linq;
using CodeTest.Accounting.Contracts;

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

        public TEntity Get(int id)
        {
            if (_entities.ContainsKey(id))
            {
                return _entities[id];
            }

            return default;
        }

        public IList<TEntity> ListAll()
        {
            return _entities.Values.ToList();
        }

        public int Set(TEntity entity)
        {
            var last = _entities.Keys.LastOrDefault();
            var id = last + 1;

            entity.Id = id;
            _entities.Add(id, entity);

            return id;
        }
    }
}
