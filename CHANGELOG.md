# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). Version numbers
are `Major.Height.0`: `Major` is bumped by hand for real breaking milestones, `Height` is the git
commit count and auto-increments on every merge to `main` - it does **not** carry Semantic
Versioning's usual "new feature vs. fix" meaning. See [README](README.md#quick-start) for details.

## [Unreleased]

### Changed

- Package versioning is now automatic: `Major.Height.0`, where `Height` is the git commit count
  and increments on every merge to `main`, computed in CI (`Directory.Build.props`,
  `ci.yml`, `release.yml`). Starting `Major` is `2`.

## [Unreleased]

### Added

- Core mediator dispatch: `SendAsync` for commands/queries, `CreateStreamAsync` for streaming
  queries, `PublishAsync` for notifications with a choice of sequential, parallel-when-all, or
  parallel-no-wait publisher strategies.
- Railway-oriented `Result`/`Result<T>` pattern for handler outcomes, avoiding exceptions for
  business-rule failures.
- Pipeline behaviors (`Syntra.Behaviors`): validation (FluentValidation), caching, retry and
  circuit-breaker (Polly), timeout, authorization, audit, idempotency, transaction, logging,
  performance, and exception-handling.
- `AddSyntra()` fluent DI registration (`Syntra.DependencyInjection`), including Scrutor-based
  assembly scanning for handlers and configuration binding for mediator options.
- Roslyn analyzers (`Syntra.Analyzers`): missing-handler, duplicate-handler, missing
  `CancellationToken`, naming convention, behavior registration order, and prefer-`Result`-over-throw
  diagnostics.
- Incremental source generator (`Syntra.SourceGenerator`) for build-time handler registration.
- `Syntra.Diagnostics`: `ActivitySource`-based tracing and metrics behaviors — published but not
  yet wired into `AddSyntra()`'s standard pipeline or exercised by a sample.
- Console, Web API, and Worker Service samples demonstrating the above.

## [0.1.0] - 2026-04-11

### Added

- Initial NuGet packaging metadata and documentation.
- Published packages: `Syntra.Abstractions`, `Syntra`, `Syntra.Behaviors`, `Syntra.DependencyInjection`, `Syntra.Analyzers`, `Syntra.Diagnostics`, `Syntra.SourceGenerator` (see README for usage).
- NuGet package icon (`icon.png`) for gallery listing on nuget.org.

[0.1.0]: https://github.com/Bubenshi-Mike/syntra/releases/tag/v0.1.0
