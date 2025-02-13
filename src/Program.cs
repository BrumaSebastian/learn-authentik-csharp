using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Server.Configurations;
using Server.Models.Options;
using src.Services.HttpClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(nameof(AuthenticationOptions)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => {
    opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme(){
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. " +
            "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
            "\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>() // No specific scopes required
        }
    });
});

builder.Services.AddServerAuthentication();

builder.Services.AddHttpClient("AuthClient", c => {
    c.BaseAddress = new(builder.Configuration["AuthenticationOptions:BaseUri"] ?? throw new Exception("Auth client requires base address"));
    c.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
});

builder.Services.AddTransient<IAuthentikClient, AuthClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:5295", "http://localhost:9000", "http://localhost:4200") // Adjust these as needed
                .AllowCredentials()  // Allows cookies to be sent
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapPost("/retrieve-token", async (string code, [FromServices] IAuthentikClient authClient) =>
{
    return Results.Ok(await authClient.RetrieveToken(code));
})
.WithName("retrieve-token")
.WithOpenApi();

app.MapPost("/verify-token", async ([FromServices] IAuthentikClient authClient, HttpContext context) => { 
    var authorizationToken = context.Request.Headers.Authorization.ToString().Split()[^1];
    return Results.Ok(await authClient.ValidateToken(authorizationToken));
})
.WithName("verify-token")
// .RequireAuthorization()
.WithOpenApi();

app.MapGet("/test", () => { Console.WriteLine("vat di fuc"); return Task.CompletedTask; })
.WithName("test")
.RequireAuthorization()
.WithOpenApi();

app.Run();
