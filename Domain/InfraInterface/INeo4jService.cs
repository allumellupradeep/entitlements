namespace Domain
{
    public interface INeo4jService
    {
        Task<(bool isAuthorized, bool isActiveUser)> RunQueryAsync( Auth auth);
        Task<string> RunQueryAsync(string query, object parameters);
            
    }
}