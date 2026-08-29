# Maintenance Backlog

Working list of what to look at during the next maintenance pass. Written at the end of the
first big roadmap push (PRs #1-#19). Nothing here is urgent enough to block using the library —
it's the "next time we're in here" list. Update this file as items get resolved or new ones
surface; don't let it go stale.

## Quick win first: PR #4 never actually reached `main`

Same root cause as PR #7 earlier in this project's history. `ci/coverage-report-and-threshold`
(PR #4) was based on `ci/coverage-collection`, and that base branch had already been merged into
`main` separately (as PR #2) *before* PR #4 merged into it. Merging PR #4 therefore merged it
into the now-stale `ci/coverage-collection` branch, not `main` — GitHub shows it as "Merged," but
`main` never got the ReportGenerator human-readable coverage summary step it added.

**Fix:** same pattern as the PR #7 → PR #18 recovery — rebuild the branch directly on current
`main` and open a fresh PR.

**Process note (see also the Documentation section below):** this is the second time a stacked
PR quietly failed to reach `main` because its base branch merged out from under it first. Worth
a standing habit: before merging any PR whose base isn't `main`, confirm that base is still
current (or retarget the PR to `main` first).

## Security

- **`main` has no branch protection at all** (confirmed via the GitHub API — a protection check
  returns "Branch not protected," 404). The "every change goes through a PR" workflow is
  currently pure convention, not enforced — nothing technically stops a direct push, and nothing
  requires CI to pass before a merge button is clickable. Worth enabling: require a pull request
  before merging, require the CI status check to pass, and (now that `release.yml` auto-publishes
  to nuget.org on every merge to `main`) this is the one guardrail that actually prevents a red
  build from shipping to the public feed.
- **Dependabot vulnerability alerts are disabled** (confirmed via the API). Enable under repo
  Settings → Code security → Dependabot alerts.
- **Dependabot security updates are disabled.** Enable once alerts are on — with Central Package
  Management already in place (`Directory.Packages.props`), a security bump is a one-file diff.
- **No `.github/dependabot.yml`.** No automated PRs for routine NuGet version bumps at all
  (security or otherwise). Worth adding a minimal weekly-cadence config for the `nuget` package
  ecosystem now that CPM makes those PRs trivial to review.
- Secret scanning is **enabled**, and push protection is **enabled** — good, no action needed.
  Secret scanning validity checks are disabled (a GitHub Advanced Security-tier feature); low
  priority, revisit if that tier becomes available.
- **CodeQL / code scanning is not configured.** The original roadmap deferred this deliberately
  until the project has real external usage — still a reasonable call, but worth re-checking
  each maintenance pass rather than forgetting about it indefinitely.
- **Known, currently unresolved vulnerability:** `Microsoft.OpenApi` 2.0.0, high severity
  (`GHSA-v5pm-xwqc-g5wc`), transitive via `Syntra.WebAPI`. First surfaced when the report-only CI
  audit step landed and is *still present as of this writing* — it was never actually fixed, only
  made visible. `Syntra.WebAPI` is a sample, not a published package, which is why it hasn't been
  urgent, but it should still get a compatible-version bump or an explicit override.
- CI's `dotnet list package --vulnerable`/`--outdated` steps are `continue-on-error` (report-only,
  non-blocking) by design, since the project was young when they were added. Reasonable to
  revisit now: should a high/critical-severity finding hard-fail CI going forward?
- `NUGET_API_KEY` is the one credential that can push to the public package feed on every merge
  to `main` — confirm it's still valid and consider a rotation cadence.

## CI / Quality Gates

- Resurrect PR #4's content (see above) and actually get a coverage threshold gate in place —
  the original plan always intended this as a follow-up once a real baseline existed.
- Re-check whether the `Syntra.Behaviors`/`Syntra.DependencyInjection` coverage-instrumentation
  gap (documented back in PR #2) is actually resolved now that PR #15 narrowed the
  `FrameworkReference`. Was theorized as the likely fix; never explicitly re-verified against a
  real coverage report.
- `Syntra.Analyzers.Tests` and `Syntra.SourceGenerator.Tests` both pull in an old
  `Microsoft.CodeAnalysis.Testing`-family package that targets pre-net10.0 reference assemblies
  and an older `Microsoft.CodeAnalysis.Common` (NU1701/NU1608 warnings on every restore). Not
  breaking anything today; watch for a newer release of those testing packages with native
  net10.0 support.
- `arch/behaviors-frameworkreference-audit`'s runtime behavior (401 unauthenticated, cache/auth
  wiring) is currently verified **manually only** against a running WebAPI instance. Worth an
  actual automated `WebApplicationFactory`-based integration test so a future change can't
  silently regress it without CI noticing.

## Architecture & Analyzers

- `PipelineBehaviorOrderAnalyzer` (SYN020) has a documented blind spot: it never fires against a
  single-expression-bodied configure lambda (`c => c.X().Y()`) — which is exactly the shape the
  README's own example uses. Currently just documented via a test
  (`Wrong_order_as_a_single_expression_chain_silently_reports_nothing`); the analyzer itself was
  never fixed.
- `SYN104` (`CommandDoesNotImplementICommand`) in `Syntra.SourceGenerator` is unreachable through
  any real usage — the generator matches the interface by metadata name only, so the compiler's
  own generic constraint already prevents the case it's checking for. Decide: fix the matching
  logic to use real interface identity, or drop the diagnostic.
- `Syntra.SourceGenerator`'s `SYN101`-`SYN103` substantially duplicate `SYN001`-`SYN003` in
  `Syntra.Analyzers`. Worth a scope decision on this package: keep both, or have the generator
  defer to the analyzers and only emit code.

## Release & Versioning

- The new `Major.Minor.0` scheme (Minor = PR-merge count mod 10, Major auto-bumps every 10
  merges to stay monotonic) is new as of this maintenance pass — watch it through at least one
  real Major rollover to confirm the arithmetic and the published version both land where
  expected.
- Because that Major bump isn't a real breaking-change signal, a floating `Major.*` package
  reference silently stops tracking new releases roughly every 10 merges. This is documented in
  the README/CHANGELOG, but there's no automated nudge for consumers (or for this repo's own
  samples) to notice when it happens — worth keeping an eye on.
- Confirm the NuGet badge in the README actually resolves now that packages are published (it
  was expected to 404 pre-publish; that should no longer be true).
- Decide the real criteria for an eventual `v1.0.0` — the README's Roadmap section currently just
  points at the CHANGELOG's `Unreleased` section rather than a concrete checklist.

## Documentation

- Add an explicit callout to `CONTRIBUTING.md` about the stacked-PR trap described at the top of
  this file — it's bitten this project twice already and is exactly the kind of thing a new
  contributor (or future self) would repeat without a written warning.
- Keep this file itself part of the routine: review and prune it at the start of every
  maintenance pass, not just add to it.
