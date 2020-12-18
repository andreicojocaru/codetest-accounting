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

        public void Set(int id, TEntity entity)
        {
            var cacheKey = $"{typeof(TEntity).Name}_{id}";

            // note: this method allows overwriting entities
            // business requirements can be implemented at Application level instead
            _memoryCache.Set(cacheKey, entity);
        }
    }
}
