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

## [0.1.0] - 2026-04-11

### Added

- Initial NuGet packaging metadata and documentation.
- Published packages: `Syntra.Abstractions`, `Syntra`, `Syntra.Behaviors`, `Syntra.DependencyInjection`, `Syntra.Analyzers`, `Syntra.Diagnostics`, `Syntra.SourceGenerator` (see README for usage).
- NuGet package icon (`icon.png`) for gallery listing on nuget.org.

[0.1.0]: https://github.com/Bubenshi-Mike/syntra/releases/tag/v0.1.0
