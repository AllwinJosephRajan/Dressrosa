namespace Dressrosa.Infrastructures
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CurrentEnvironment => configuration["Environment"];

        public async Task<string> GetDBConnectionStringAsync()
        {
            string configValue = "";
            configValue = configuration.GetSection("ConnectionStrings")["Database"];
            return configValue;
        }
    }
}
