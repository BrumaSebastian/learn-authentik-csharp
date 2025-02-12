using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Server.Configurations;
using Server.Models.Options;

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
});

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

app.MapPost("/retrieve-token", async (string code, [FromServices]IHttpClientFactory clientFactory, [FromServices]IConfiguration configuration) => { 
    var authClient = clientFactory.CreateClient("AuthClient");
    var data = new StringContent(
        $"grant_type=authorization_code" +
        $"&code={code}" + 
        $"&client_id={configuration["AuthenticationOptions:ClientId"]}" +
        $"&client_secret={configuration["AuthenticationOptions:ClientSecret"]}" +
        $"&redirect_uri={configuration["AuthenticationOptions:CallbackPath"]}",
        Encoding.UTF8,
        "application/x-www-form-urlencoded"
    );

    var endpoint = configuration["AuthenticationOptions:JWTToken"];
    var response = await authClient.PostAsync("token/", data);
    var result = await response.Content.ReadAsStringAsync();

    return Results.Ok(result);
})
.WithName("retrieve-token")
.WithOpenApi();

app.MapPost("/verify-token", async ([FromServices]IHttpClientFactory clientFactory, [FromServices]IConfiguration configuration, HttpContext context) => { 
    var authClient = clientFactory.CreateClient("AuthClient");
    var s = context.Request.Headers.Authorization.ToString().Split()[^1];
    var data = new StringContent(
        $"token={context.Request.Headers.Authorization.ToString().Split()[^1]}&client_id={configuration["AuthenticationOptions:ClientId"]}&client_secret={configuration["AuthenticationOptions:ClientSecret"]}&scope=openid offline_access",
        Encoding.UTF8,
        "application/x-www-form-urlencoded"
    );
    var response = await authClient.PostAsync("introspect/", data);
    var result = await response.Content.ReadAsStringAsync();

    return Results.Ok(result);
})
.WithName("verify-token")
// .RequireAuthorization()
.WithOpenApi();

app.MapGet("/test", () => { Console.WriteLine("vat di fuc"); return Task.CompletedTask; })
.WithName("test")
.RequireAuthorization()
.WithOpenApi();

app.Run();
