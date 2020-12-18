using Microsoft.Extensions.Caching.Memory;

namespace CodeTest.Accounting.Persistence
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity>
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public TEntity Get(int id)
        {
            var cacheKey = $"{typeof(TEntity).Name}_{id}";

            if (_memoryCache.TryGetValue(cacheKey, out var entity))
            {
                if (entity is TEntity validEntity)
                {
                    return validEntity;
                }

                // invalid cache entry, not Customer object
                _memoryCache.Remove(cacheKey);
            }

            return default;
        }

        public int Set(TEntity entity)
        {
            var id = GetNextId();
            var cacheKey = $"{typeof(TEntity).Name}_{id}";

            // note: this method allows overwriting entities
            // business requirements can be implemented at Application level instead
            _memoryCache.Set(cacheKey, entity);

            // we need to increment the IDs to next added item will receive a correct Id
            IncrementId();

            return id;
        }

        private int GetNextId()
        {
            // note: this would be a simple Count if we were using an List or a database system
            // i.e. Entity Framework over SQL can automatically generate Identity ids

            // MemoryCache doesn't have a count, so we will fake it
            // a simple List could also do, but MemoryCache is nice because we use it to store Entities too
            var cacheKey = $"{typeof(TEntity).Name}_idx";

            if (_memoryCache.TryGetValue(cacheKey, out var index))
            {
                if (index is int number)
                {
                    return number + 1;
                }

                // invalid cache entry, not Int32 type
                _memoryCache.Remove(cacheKey);
            }


            // start from 1
            return 1;
        }

        private void IncrementId()
        {
            var cacheKey = $"{typeof(TEntity).Name}_idx";
            var nextId = GetNextId();

            _memoryCache.Set(cacheKey, nextId);
        }
    }
}
