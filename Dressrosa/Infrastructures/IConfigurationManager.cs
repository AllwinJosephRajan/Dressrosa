namespace Dressrosa.Infrastructures
{
    public interface IConfigurationManager
    {
        string CurrentEnvironment { get; }
        Task<string> GetDBConnectionStringAsync();
    }
}
