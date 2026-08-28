# Syntra

Syntra is a **.NET 10** mediator-style library with **CQRS-friendly** requests, **railway-oriented** `Result` types, **pipeline behaviors**, **notifications**, and **streaming queries**. Optional **Roslyn analyzers** help keep registration and conventions consistent.

> **Target framework:** `net10.0`. **Repository:** [github.com/Bubenshi-Mike/syntra](https://github.com/Bubenshi-Mike/syntra).

## Packages

| Package | Purpose |
|--------|---------|
| `Syntra.Abstractions` | Contracts: `IRequest`, `Result`, handlers, notifications, pipeline hooks. |
| `Syntra` | Mediator implementation, execution, options. |
| `Syntra.Behaviors` | Cross-cutting behaviors (validation, caching, retry, auth, audit, …). |
| `Syntra.DependencyInjection` | `AddSyntra`, Scrutor scanning, configuration binding. |
| `Syntra.Analyzers` | Roslyn analyzers (reference as analyzer; `PrivateAssets="all"`). |
| `Syntra.Diagnostics` | Optional diagnostics integration. |
| `Syntra.SourceGenerator` | Optional source generators. |

## Quick start

Reference the packages you need (typically **Abstractions**, **Syntra**, **Behaviors**, **DependencyInjection**):

Package versions auto-increment on every merge to `main` (`Major.Height.0` - see
[CHANGELOG](CHANGELOG.md)), so pin to the current major line with a floating version instead of
a specific number that will quickly go stale:

```xml
<ItemGroup>
  <PackageReference Include="Syntra.Abstractions" Version="2.*" />
  <PackageReference Include="Syntra" Version="2.*" />
  <PackageReference Include="Syntra.Behaviors" Version="2.*" />
  <PackageReference Include="Syntra.DependencyInjection" Version="2.*" />
  <PackageReference Include="Syntra.Analyzers" Version="2.*" PrivateAssets="all" />
</ItemGroup>
```

```csharp
using Syntra.DependencyInjection.Extensions;
using Syntra.DependencyInjection.Registration;

builder.Services.AddSyntra(builder.Configuration, c => c
    .AddBehaviors(b => b.AddStandardPipeline())
    .ScanAssemblies(typeof(Program).Assembly));
```

See the `samples/` folder in this repository for **Web API**, **Console**, and **Worker** examples.

## Building & packing locally

```bash
dotnet test -c Release
dotnet pack -c Release -o ./artifacts
```

A local pack without an explicit `-p:Version=` gets a version derived from your current commit
(`Major.Height.0-g{shortsha}`) so it's never mistaken for an official build. CI computes and
passes the real value explicitly - see `Directory.Build.props`.

## License

MIT — see [LICENSE](LICENSE).
