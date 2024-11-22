using Microsoft.Extensions.Options;
using Server.Models.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;

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
        } )
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:9000/application/o/angular-client/";
                options.Audience = "nE5oG8lPWZNPs7yxF8o46dRu8lTyeohKDVDBwupF";
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