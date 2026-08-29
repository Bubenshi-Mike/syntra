# Syntra

[![CI](https://github.com/Bubenshi-Mike/syntra/actions/workflows/ci.yml/badge.svg)](https://github.com/Bubenshi-Mike/syntra/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Syntra.svg?label=Syntra)](https://www.nuget.org/packages/Syntra)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Syntra is a **.NET 10** mediator-style library with **CQRS-friendly** requests, **railway-oriented** `Result` types, **pipeline behaviors**, **notifications**, and **streaming queries**. Optional **Roslyn analyzers** help keep registration and conventions consistent.

> **Target framework:** `net10.0`. **Repository:** [github.com/Bubenshi-Mike/syntra](https://github.com/Bubenshi-Mike/syntra).
>
> Pre-`v0.1.0`: not yet published to NuGet.org. The badges above will resolve once it is.

## Packages

| Package | Purpose |
|--------|---------|
| `Syntra.Abstractions` | Contracts: `IRequest`, `Result`, handlers, notifications, pipeline hooks. |
| `Syntra` | Mediator implementation, execution, options. |
| `Syntra.Behaviors` | Cross-cutting behaviors (validation, caching, retry, auth, audit, …). |
| `Syntra.DependencyInjection` | `AddSyntra`, Scrutor scanning, configuration binding. |
| `Syntra.Analyzers` | Roslyn analyzers (reference as analyzer; `PrivateAssets="all"`). |
| `Syntra.Diagnostics` | Opt-in tracing (`ActivitySource`) and metrics behaviors — enable with `.AddBehaviors(b => b.AddStandardPipeline().AddDiagnostics())`. |
| `Syntra.SourceGenerator` | Optional source generators. |

## Quick start

Reference the packages you need (typically **Abstractions**, **Syntra**, **Behaviors**, **DependencyInjection**):

Package versions auto-increment on every merge to `main` - Minor counts merges 0-9 within a
Major, then rolls into an extra Major increment and starts again at 0 (see
[CHANGELOG](CHANGELOG.md)). Because that Major bump is just to keep versions increasing as Minor
cycles, not a signal of an actual breaking change, a floating `Major.*` reference will stop
tracking new releases every 10 merges - check back periodically and bump the pin, or float on
`*` for the truly latest:

```xml
<ItemGroup>
  <PackageReference Include="Syntra.Abstractions" Version="3.*" />
  <PackageReference Include="Syntra" Version="3.*" />
  <PackageReference Include="Syntra.Behaviors" Version="3.*" />
  <PackageReference Include="Syntra.DependencyInjection" Version="3.*" />
  <PackageReference Include="Syntra.Analyzers" Version="3.*" PrivateAssets="all" />
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
(`Major.Minor-g{shortsha}`) so it's never mistaken for an official build. CI computes and passes
the real value explicitly - see `Directory.Build.props`.

Every merge to `main` also publishes the resulting version to NuGet.org automatically - see
`release.yml`.

## Roadmap

Before a `v1.0.0` release:

- [ ] See [CHANGELOG](CHANGELOG.md) `Unreleased` for architecture changes already landed

## License

MIT — see [LICENSE](LICENSE).
