# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- `Syntra.Behaviors` no longer takes a `FrameworkReference` on `Microsoft.AspNetCore.App`.
  It now declares the specific packages it actually needs
  (`Microsoft.AspNetCore.Authorization`, `Microsoft.AspNetCore.Http.Abstractions`,
  `Microsoft.Extensions.Caching.Abstractions`) instead of implicitly requiring the full
  ASP.NET Core shared framework at runtime for every consumer, including non-web hosts.
  Verified end-to-end: `AuthorizationBehavior` correctly blocks unauthenticated requests
  (401) and passes authenticated ones through to the handler in the WebAPI sample; the
  ConsoleApp and WorkerService samples no longer reference the `Microsoft.AspNetCore.App`
  shared framework at all (confirmed via `runtimeconfig.json`).

## [0.1.0] - 2026-04-11

### Added

- Initial NuGet packaging metadata and documentation.
- Published packages: `Syntra.Abstractions`, `Syntra`, `Syntra.Behaviors`, `Syntra.DependencyInjection`, `Syntra.Analyzers`, `Syntra.Diagnostics`, `Syntra.SourceGenerator` (see README for usage).
- NuGet package icon (`icon.png`) for gallery listing on nuget.org.

[0.1.0]: https://github.com/Bubenshi-Mike/syntra/releases/tag/v0.1.0
