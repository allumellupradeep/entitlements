
namespace Domain
{
    public class ProcessingServices
    {
        private readonly INeo4jService _neo4jService;

        public ProcessingServices(INeo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<(bool isAuthorized, bool isActiveUser)> Authorize(string subject, string permissionName, string username)
        {
            var auth = new Auth
            {
                tenantId = subject,
                permission = permissionName,
                username = username
            };
            var result = await _neo4jService.RunQueryAsync(auth);
            return result;
        }

        public async Task<string> TestConnection(string str)
        {
            return await _neo4jService.RunQueryAsync(str, null);
        }
    }
}