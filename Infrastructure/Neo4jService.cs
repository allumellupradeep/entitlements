namespace Infrastructure
{
    using Domain;
    using Neo4j.Driver;

    public class Neo4jService : INeo4jService, IAsyncDisposable
    {
        private readonly IDriver _driver;
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

        public async Task<(bool isAuthorized, bool isActiveUser)> RunQueryAsync(Auth auth)
        {
            var cypherQuery = @"OPTIONAL MATCH (t:Tenant {tenantId: $tenantId})
                                OPTIONAL MATCH (t)-[:OWNS_USERGROUP]->(g:UserGroup)-[:OWNS]->(u:User {userName: $username})
                                OPTIONAL MATCH (g)-[:OWNS_ROLE]->(r:Role)-[:HAS_PERMISSION]->(p:Permission {action: $permission})

                                WITH u, COUNT(DISTINCT p) AS permCount

                                RETURN 
                                    (permCount > 0) AS isAllowed,
                                    (u IS NOT NULL AND u.status = 'Active') AS isActiveUser
                                LIMIT 1
                                ";
            await using var session = CreateSession();
            var result = await session.RunAsync(cypherQuery, new
            {
                auth.tenantId,
                auth.username,
                auth.permission
            });

            var record = await result.SingleAsync();

            return (record["isAllowed"].As<bool>(), record["isActiveUser"].As<bool>());
        }

        public async ValueTask DisposeAsync()
        {
            if (_driver != null)
                await _driver.DisposeAsync();
        }
    }
}
