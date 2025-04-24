namespace CoreService.Helpers
{
    public static class ConfigurationHelper
    {
        public static IConfiguration _config;
        public static void Initialize(IConfiguration config)
        {
            _config = config;
        }
    }
}
