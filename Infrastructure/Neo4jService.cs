namespace Infrastructure
{
    using Domain;
    using Microsoft.Extensions.Configuration;
    using Neo4j.Driver;

    public class Neo4jService : INeo4jService, IAsyncDisposable
    {
        private readonly IDriver _driver;
        private readonly IConfiguration config;
        private readonly string _databaseName;

        public Neo4jService(string uri, string username, string password, string database)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
            _databaseName = database;
        }

        private IAsyncSession CreateSession()
        {
            return _driver.AsyncSession(o => o.WithDatabase(_databaseName));
        }

        public async Task<string> RunQueryAsync(string query, object parameters = null)
        {
            await using var session = CreateSession();
            var result = await session.RunAsync(query, parameters);
            var record = await result.SingleAsync();

            return record["message"].As<string>();
        }

        public async Task<bool> RunQueryAsync(Auth auth)
        {
            var cypherQuery = @"
                        MATCH (t:Tenant {tenantId: $tenantId})
                        -[:OWNS_USERGROUP]->(g:UserGroup)
                        -[:OWNS]->(u:User {userName: $username}),
                        (g)-[:OWNS_ROLE]->(:Role)
                        -[:HAS_PERMISSION]->(p:Permission {action: $permission})
                        RETURN COUNT(p) > 0 AS isAllowed
                        ";
            await using var session = CreateSession();
            var result = await session.RunAsync(cypherQuery, new
            {
                auth.tenantId,
                auth.username,
                auth.permission
            });

            var record = await result.SingleAsync();
            bool isAllowed = record["isAllowed"].As<bool>();
            return isAllowed;
        }

        public async ValueTask DisposeAsync()
        {
            if (_driver != null)
                await _driver.DisposeAsync();
        }
    }
}
