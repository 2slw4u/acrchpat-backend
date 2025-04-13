using CoreService.Models.Cache;

namespace CoreService.Helpers.Cache
{
    public interface IUserParametersCache
    {
        public UserParametersCacheEntry? GetUserParametersFromCache(string userLogin);
        public void InsertUserParametersIntoCache(string userLogin, UserParametersCacheEntry userParameters);
    }
}
