using Domain;
using Infrastructure;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.GetSection("Neo4j").Get<Neo4jSettings>();
builder.Services.AddSingleton<INeo4jService>(provider =>
{
    return new Neo4jService(config.Uri, config.Username, config.Password, config.Database);
});
builder.Services.AddScoped<ProcessingServices>();
builder.Services.AddScoped<Neo4jInitializer>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDriver>(provider =>
{
    var config = builder.Configuration.GetSection("Neo4j").Get<Neo4jSettings>();
    return GraphDatabase.Driver(config.Uri, AuthTokens.Basic(config.Username, config.Password));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<Neo4jInitializer>();
    await initializer.InitializeAsync();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rbac");
        c.RoutePrefix = string.Empty; 
    });
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
