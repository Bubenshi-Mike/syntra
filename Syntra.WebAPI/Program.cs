using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Syntra.DependencyInjection.Extensions;
using Syntra.DependencyInjection.Registration;
using Scalar.AspNetCore;
using Syntra.WebAPI.Api;
using Syntra.WebAPI.Application;
using Syntra.WebAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<OrderStore>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "syntra.sample";
        o.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Orders.Cancel", p => p.RequireAuthenticatedUser());
});

builder.Services.UseAuditWriter<SampleAuditWriter>();
builder.Services.UseTransactionManager<SampleTransactionManager>();

builder.Services.AddSyntra(builder.Configuration, c => c
    .AddBehaviors(b => b.AddStandardPipeline().AddDiagnostics())
    .ScanAssemblies(typeof(Program).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Syntra sample API"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost(
        "/api/auth/login",
        async (HttpContext http) =>
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "demo-user"),
                new Claim(ClaimTypes.NameIdentifier, "demo-user-id"),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await http.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity))
                .ConfigureAwait(false);
            return Results.Ok(new
            {
                message = "Cookie session created. Authenticated requests can cancel orders.",
            });
        })
    .WithTags("Auth");

app.MapGet(
        "/api/auth/logout",
        async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return Results.Ok(new { message = "Signed out." });
        })
    .WithTags("Auth");

app.MapControllers();
app.MapOrdersMinimalApi();

app.Run();
