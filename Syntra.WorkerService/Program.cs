using Syntra.DependencyInjection.Extensions;
using Syntra.DependencyInjection.Registration;
using Syntra.WorkerService.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
// Do not call AddAuthorization() here: it registers AuthorizationPolicyCache, which requires
// EndpointDataSource — only available in the ASP.NET Core web host, not generic Worker/Console hosts.

builder.Services.UseAuditWriter<SampleAuditWriter>();
builder.Services.UseTransactionManager<SampleTransactionManager>();

builder.Services.AddSyntra(builder.Configuration, c => c
    .AddBehaviors(b => b
        .AddException()
        .AddLogging()
        .AddValidation()
        .AddIdempotency()
        .AddCaching()
        .AddPerformance()
        .AddRetry(ServiceLifetime.Singleton)
        .AddCircuitBreaker(ServiceLifetime.Singleton)
        .AddTimeout()
        .AddTransaction()
        .AddAudit())
    .ScanAssemblies(typeof(Program).Assembly));

builder.Services.AddHostedService<Syntra.WorkerService.SyntraDemoWorker>();

var host = builder.Build();
host.Run();
