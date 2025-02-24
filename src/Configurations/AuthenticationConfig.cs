using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Server.Models.Options;

namespace Server.Configurations;

public static class AuthenticationConfigs
{
    public static IServiceCollection AddServerAuthentication(this IServiceCollection services)
    {
        var authOptions = services.BuildServiceProvider()
        .GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        IdentityModelEventSource.ShowPII = true;

        services.AddAuthentication( app => {
            app.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            app.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.Authority = $"{authOptions.BaseUri}dev-authentik-app/";
            options.Audience = authOptions.ClientId;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
            };
        });

        services.AddAuthorization();

        return services;
    }
}