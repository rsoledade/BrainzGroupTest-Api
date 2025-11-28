using Microsoft.EntityFrameworkCore;
using StudentEvents.Infrastructure.Data;
using StudentEvents.Application.Services;
using StudentEvents.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentEventsDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddHttpClient();

var jwtSection = configuration.GetSection("JwtSettings");
var jwtKey = jwtSection.GetValue<string>("Key") ?? "please-change-this-secret-key";
var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "StudentEventsApi";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "StudentEventsApiUsers";
var jwtExpireMinutes = jwtSection.GetValue<int?>("ExpireMinutes") ?? 60;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        // allow small clock skew
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});


builder.Services.AddAuthorization();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IGraphSyncService, GraphSyncService>();

builder.Services.AddSingleton<IGraphClientFactory, GraphClientFactory>();

builder.Services.AddHostedService<GraphSyncBackgroundService>();

var app = builder.Build();

// ensure database is created and migrations applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentEventsDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
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