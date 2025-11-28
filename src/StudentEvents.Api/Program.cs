using Microsoft.EntityFrameworkCore;
using StudentEvents.Infrastructure.Data;
using StudentEvents.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// configure Swagger with JWT bearer support
builder.Services.AddSwaggerWithJwt();

var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentEventsDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddHttpClient();

// moved JWT setup into extension method
builder.Services.AddJwtAuthentication(configuration);

// moved infrastructure registrations into extension method
builder.Services.AddInfrastructureServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<StudentEvents.Api.Configuration.DatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();