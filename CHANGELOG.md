# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Removed

- `Syntra.Abstractions.Streams.IStreamQuery<T>`, a duplicate of the canonical
  `Syntra.Abstractions.Requests.IStreamQuery<T>` (the one the mediator's dispatch path and
  every sample actually use). It had zero consumers in this repo and doubled the public API
  surface for one concept. If you implemented it directly, switch to
  `Syntra.Abstractions.Requests.IStreamQuery<T>` (same shape - drop-in).

## [0.1.0] - 2026-04-11

### Added

- Initial NuGet packaging metadata and documentation.
- Published packages: `Syntra.Abstractions`, `Syntra`, `Syntra.Behaviors`, `Syntra.DependencyInjection`, `Syntra.Analyzers`, `Syntra.Diagnostics`, `Syntra.SourceGenerator` (see README for usage).
- NuGet package icon (`icon.png`) for gallery listing on nuget.org.

[0.1.0]: https://github.com/Bubenshi-Mike/syntra/releases/tag/v0.1.0
