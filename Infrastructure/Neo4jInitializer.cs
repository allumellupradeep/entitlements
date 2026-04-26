    using Neo4j.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    namespace Infrastructure
    {
        public class Neo4jInitializer
        {
            private readonly IDriver _driver;
            private readonly string _database;

            public Neo4jInitializer(IDriver driver, IConfiguration config)
            {
                _driver = driver;
                _database = config["Neo4j:Database"];
            }

            public async Task InitializeAsync()
            {
                await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

                await session.ExecuteWriteAsync(async tx =>
                {
                    var cypherStatements = new[]
                    {
                        // Tenant
                        "MERGE (t2:Tenant {tenantId: 'T2'}) SET t2.name = 'OCBC Bank'",

                        // Users
                        "MERGE (u1:User {userId: 'U1'}) SET u1.userName = 'smith@ocbc.com', u1.status = 'Active'",
                        "MERGE (u2:User {userId: 'U2'}) SET u2.userName = 'chentan@ocbc.com', u2.status = 'Active'",
                        "MERGE (u5:User {userId: 'U5'}) SET u5.userName = 'john@ocbc.com', u5.status = 'Active'",

                        // Parties
                        "MERGE (p1:Party {partyId: 'P1'}) SET p1.name = 'Smith', p1.type = 'individual'",
                        "MERGE (p2:Party {partyId: 'P2'}) SET p2.name = 'Chen Tan', p2.type = 'individual'",
                        "MERGE (p5:Party {partyId: 'P5'}) SET p5.name = 'John Singleton', p5.type = 'individual'",

                        // Roles
                        "MERGE (r1:Role {roleId: 'R1'}) SET r1.name = 'CustomerServiceRep'",
                        "MERGE (r2:Role {roleId: 'R2'}) SET r2.name = 'PaymentOfficer'",
                        "MERGE (r3:Role {roleId: 'R3'}) SET r3.name = 'SalesRep'",

                        // Permissions
                        "MERGE (perm1:Permission {permId: 'PERM1'}) SET perm1.action = 'READ_CUSTOMER'",
                        "MERGE (perm2:Permission {permId: 'PERM2'}) SET perm2.action = 'INITIATE_PAYMENT'",
                        "MERGE (perm3:Permission {permId: 'PERM3'}) SET perm3.action = 'CALL_CUSTOMER'",

                        // UserGroups
                        "MERGE (g1:UserGroup {name: 'salesGroup'})",
                        "MERGE (g2:UserGroup {name: 'paymentsGroup'})",
                        "MERGE (g3:UserGroup {name: 'servicesGroup'})",

                        // Relationships
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (u1:User {userId: 'U1'}) MERGE (t2)-[:HAS_IDENTITY]->(u1)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (u2:User {userId: 'U2'}) MERGE (t2)-[:HAS_IDENTITY]->(u2)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (u5:User {userId: 'U5'}) MERGE (t2)-[:HAS_IDENTITY]->(u5)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (p1:Party {partyId: 'P1'}) MERGE (t2)-[:OWNS]->(p1)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (p2:Party {partyId: 'P2'}) MERGE (t2)-[:OWNS]->(p2)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (p5:Party {partyId: 'P5'}) MERGE (t2)-[:OWNS]->(p5)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (r1:Role {roleId: 'R1'}) MERGE (t2)-[:OWNS]->(r1)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (r2:Role {roleId: 'R2'}) MERGE (t2)-[:OWNS]->(r2)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (r3:Role {roleId: 'R3'}) MERGE (t2)-[:OWNS]->(r3)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (g1:UserGroup {name: 'salesGroup'}) MERGE (t2)-[:OWNS_USERGROUP]->(g1)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (g2:UserGroup {name: 'paymentsGroup'}) MERGE (t2)-[:OWNS_USERGROUP]->(g2)",
                        "MATCH (t2:Tenant {tenantId: 'T2'}), (g3:UserGroup {name: 'servicesGroup'}) MERGE (t2)-[:OWNS_USERGROUP]->(g3)",

                        "MATCH (r1:Role {roleId: 'R1'}), (perm1:Permission {permId: 'PERM1'}) MERGE (r1)-[:HAS_PERMISSION]->(perm1)",
                        "MATCH (r2:Role {roleId: 'R2'}), (perm2:Permission {permId: 'PERM2'}) MERGE (r2)-[:HAS_PERMISSION]->(perm2)",
                        "MATCH (r3:Role {roleId: 'R3'}), (perm3:Permission {permId: 'PERM3'}) MERGE (r3)-[:HAS_PERMISSION]->(perm3)",

                        "MATCH (g1:UserGroup {name: 'salesGroup'}), (u5:User {userId: 'U5'}) MERGE (g1)-[:OWNS]->(u5)",
                        "MATCH (g1:UserGroup {name: 'salesGroup'}), (r3:Role {roleId: 'R3'}) MERGE (g1)-[:OWNS_ROLE]->(r3)",
                        "MATCH (g2:UserGroup {name: 'paymentsGroup'}), (u2:User {userId: 'U2'}) MERGE (g2)-[:OWNS]->(u2)",
                        "MATCH (g2:UserGroup {name: 'paymentsGroup'}), (u5:User {userId: 'U5'}) MERGE (g2)-[:OWNS]->(u5)",
                        "MATCH (g2:UserGroup {name: 'paymentsGroup'}), (r2:Role {roleId: 'R2'}) MERGE (g2)-[:OWNS_ROLE]->(r2)",
                        "MATCH (g3:UserGroup {name: 'servicesGroup'}), (u1:User {userId: 'U1'}) MERGE (g3)-[:OWNS]->(u1)",
                        "MATCH (g3:UserGroup {name: 'servicesGroup'}), (r1:Role {roleId: 'R1'}) MERGE (g3)-[:OWNS_ROLE]->(r1)"

                };

                    foreach (var statement in cypherStatements)
                    {
                        await tx.RunAsync(statement);
                    }
                });
            }
        }
    }
