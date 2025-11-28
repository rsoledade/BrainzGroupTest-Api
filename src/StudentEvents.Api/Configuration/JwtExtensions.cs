using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace StudentEvents.Api.Configuration
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");
            var jwtKey = jwtSection.GetValue<string>("Key") ?? "please-change-this-secret-key";
            var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "StudentEventsApi";
            var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "StudentEventsApiUsers";
            var expireMinutes = jwtSection.GetValue<int?>("ExpireMinutes") ?? 120;

            // Ensure key bytes are derived the same way as TokenService (hash to 256-bit if shorter)
            byte[] keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            if (keyBytes.Length < 32)
            {
                keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(jwtKey));
            }

            services.AddAuthentication(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    // give some clock skew but less than expire time
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
