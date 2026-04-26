
#PROJECT OVERVIEW

This solution is built using .NET 9 and integrates with a Neo4j graph database to manage and authorize user entitlements using an RBAC model.

#SETUP STEPS

1. Open the solution (.sln) in Visual Studio 2022 or later.

2. Create Neo4j database:
   Run: CREATE DATABASE MaintainEntitlements IF NOT EXISTS;

3. Update appsettings.json:
   {
     "Neo4j": {
       "Uri": "bolt://localhost:7687",
       "Username": "neo4j",
       "Password": "<your-password>",
       "Database": "MaintainEntitlements"
     }
   }

4. Run the application using IIS Express.

#On startup:
- Swagger UI will open
- Database will be seeded automatically using code-first approach

#API DETAILS

1. GET /test
   - Checks DB connection
   - Returns: Neo4j Connected

2. POST /authorize
   - Authorizes user based on request headers
   - Can be extended to JWT-based authorization

#SUMMARY

- .NET 9 + Neo4j integration
- Code-first data initialization
- RBAC-ready structure
