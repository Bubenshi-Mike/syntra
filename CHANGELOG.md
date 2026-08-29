# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). Version numbers
are `Major.Minor.0`: `Minor` auto-increments once per PR merged into `main` and cycles 0-9;
`Major` starts at a hand-picked base (bumped for real breaking milestones) but also gets an
automatic +1 every time `Minor` wraps back to 0, purely to keep every published version strictly
higher than the last - it does **not** carry Semantic Versioning's usual "new feature vs. fix vs.
breaking change" meaning. A floating `Major.*` package reference will stop tracking new releases
whenever that automatic bump happens, roughly every 10 merges. See
[README](README.md#quick-start) for details.

## [Unreleased]

### Changed

- Package versioning is now `Major.Minor.0`, where `Minor` is the count of PRs merged into
  `main` (not raw commit count, which grows much faster) and cycles 0-9, rolling into an
  automatic `Major` increment every 10 merges to keep versions strictly increasing - computed in
  `Directory.Build.props`, `ci.yml`, `release.yml`. Starting `Major` base is `2`.
- Every merge to `main` that actually touches a packable project's source, shared build/version
  metadata, or the embedded README/LICENSE/icon now publishes the resulting version to
  NuGet.org automatically (`release.yml`) - not just an explicitly pushed version tag. A
  docs-only, CI-only, sample-only, or test-only merge is skipped so the published version
  doesn't churn for changes nobody consuming the packages would ever see.
- NuGet package icon replaced with the new Syntra logo.

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
