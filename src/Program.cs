using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Server.Configurations;
using Server.Models;
using Server.Models.Options;
using Server.Services;

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
});
builder.Services.AddServerAuthentication();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:5295", "http://localhost:9000") // Adjust these as needed
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


app.MapGet("/test", () => { Console.WriteLine("vat di fuc"); return Task.CompletedTask; })
.WithName("test")
.RequireAuthorization()
.WithOpenApi();

app.Run();
