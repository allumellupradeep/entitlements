namespace Domain
{
    public interface INeo4jService
    {
        Task<bool> RunQueryAsync( Auth auth);
        Task<string> RunQueryAsync(string query, object parameters);
            
    }
}