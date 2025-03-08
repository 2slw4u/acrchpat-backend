using CoreService.Models.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace CoreService.Helpers.Cache
{
    public class UserParametersCache : IUserParametersCache
    {
        private readonly IMemoryCache _memoryCache;
        private MemoryCacheEntryOptions _cacheOptions;
        public UserParametersCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            this._cacheOptions = new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
        }
        public UserParametersCacheEntry? GetUserParametersFromCache(string userLogin)
        {
            _memoryCache.TryGetValue<UserParametersCacheEntry>(userLogin, out UserParametersCacheEntry? result);
            if (result == null)
            {
                Console.WriteLine("Entry in cache not found");
            }
            else
            {
                Console.WriteLine($"Found entry in cache: {result.Id}, banned: {result.IsBanned}, role: {result.Roles[0].Id} + {result.Roles[0].Name}");
                this.InsertUserParametersIntoCache(userLogin, result);
            }
            return result;
        }

        public void InsertUserParametersIntoCache(string userLogin, UserParametersCacheEntry userParameters)
        {
            _memoryCache.Set<UserParametersCacheEntry>(userLogin, userParameters, this._cacheOptions);
        }
    }
}
