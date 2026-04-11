using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syntra.Abstractions.Mediator;
using Syntra.ConsoleApp.Demo;
using Syntra.ConsoleApp.Infrastructure;
using Syntra.DependencyInjection.Extensions;
using Syntra.DependencyInjection.Registration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
// Generic host has no EndpointDataSource; AddAuthorization() pulls AuthorizationPolicyCache and fails at Build().

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

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();
var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Syntra.Console");

logger.LogInformation("=== Syntra console sample — command / query / notification / stream ===");

var idem = Guid.NewGuid();
logger.LogInformation("-- Idempotent command (same key twice; second hits idempotency cache)");
var r1 = await mediator.SendAsync(new LogMessageCommand(idem, "first logical send")).ConfigureAwait(false);
var r2 = await mediator.SendAsync(new LogMessageCommand(idem, "replay same key")).ConfigureAwait(false);
logger.LogInformation("First: {Ok1}, Second (cached path): {Ok2}", r1.IsSuccess, r2.IsSuccess);

logger.LogInformation("-- Cached query (run twice; second should often be faster via distributed cache)");
_ = await mediator.SendAsync(new GetInventoryQuery("SKU-42")).ConfigureAwait(false);
_ = await mediator.SendAsync(new GetInventoryQuery("SKU-42")).ConfigureAwait(false);

logger.LogInformation("-- Notification fan-out (two handlers)");
await mediator.PublishAsync(new PingNotification("Program")).ConfigureAwait(false);

logger.LogInformation("-- Retry pipeline (handler fails until attempt aligns with retry policy)");
var echo = await mediator.SendAsync(new FlakyEchoQuery("hello")).ConfigureAwait(false);
logger.LogInformation("Echo result: {Ok} {Value}", echo.IsSuccess, echo.IsSuccess ? echo.Value : echo.Error?.Message);

logger.LogInformation("-- Streaming query (async enumerable)");
await foreach (var n in mediator.CreateStreamAsync(new CountdownStreamQuery(5, 1)).ConfigureAwait(false))
    logger.LogInformation("  stream -> {N}", n);

logger.LogInformation("=== Done ===");
