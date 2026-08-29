# Contributing to Syntra

Thanks for your interest in improving Syntra.

## Workflow

- All changes land through a pull request from a topic branch, reviewed before merge — direct pushes to `main` aren't accepted. This is currently enforced by convention only; GitHub branch protection isn't turned on yet (see [docs/BACKLOG.md](docs/BACKLOG.md#security)).
- Branch names follow `<type>/<short-description>`, e.g. `fix/retry-behavior-timeout`, `docs/readme-badges`.
- Keep PRs focused: one logical change per PR is easier to review than a bundle of unrelated fixes.
- **Stacked PRs** (a branch based on another still-open PR's branch, not on `main`): if the base branch merges into `main` before the stacked PR does, merging the stacked PR afterward merges it into that now-stale base branch — not `main`. GitHub will show it as "Merged," but the content never actually reaches `main`. This has happened twice already. Before merging a PR whose base isn't `main`, confirm that base is still current, or retarget the PR to `main` first.

## Building and testing locally

```bash
dotnet restore Syntra.slnx
dotnet build Syntra.slnx -c Release
dotnet test Syntra.slnx -c Release
dotnet pack Syntra.slnx -c Release -o ./artifacts
```

Before opening a PR, run `dotnet format` and make sure it reports no changes:

```bash
dotnet format Syntra.slnx --verify-no-changes
```

## Project layout and conventions

- **Layering** (enforced by `Syntra.Architecture.Tests`): `Syntra.Abstractions` has zero external dependencies; `Syntra` (core) references only Abstractions; `Syntra.Behaviors` may reference Abstractions + Core; `Syntra.DependencyInjection` may reference Core + Abstractions + Behaviors. Core must never reference DependencyInjection or Behaviors.
- **Style**: C# 14 / .NET 10, file-scoped namespaces, primary constructors for DI, one public type per file. Handlers and behaviors return `Result`/`Result<T>` — don't throw for business-rule failures.
- **Tests**: unit tests use NSubstitute and test one class in isolation; integration tests use a real `ServiceProvider` (don't mock the mediator); architecture tests use NetArchTest.Rules to enforce the layering rules above.

See `.cursor/rules/*.mdc` for the fuller convention reference used during development.

## Commit messages and changelog

Add an entry under the `Unreleased` section of [CHANGELOG.md](CHANGELOG.md) for any PR that changes public behavior.

## Reporting bugs and requesting features

Use the issue templates when opening a GitHub issue. For security vulnerabilities, see [SECURITY.md](SECURITY.md) instead of filing a public issue.
