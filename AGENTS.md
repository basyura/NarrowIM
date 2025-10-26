# Repository Guidelines

## Project Structure & Module Organization
- Solution: `NarrowIM.sln` at repo root.
- Source: `src/` (e.g., `src/NarrowIM.*`). UI or app entry: `src/NarrowIM.App/`.
- Tests: `tests/` (e.g., `tests/NarrowIM.*.Tests/`).
- Assets & configs: `assets/`, `docs/`, `.editorconfig`, `.gitignore`.
- Build outputs and IDE folders (`bin/`, `obj/`, `.vs/`) are ignored.

## Build, Test, and Development Commands
- Restore: `dotnet restore`
- Build: `dotnet build -c Debug|Release`
- Run app: `dotnet run --project src/NarrowIM.App`
- Test: `dotnet test -c Release`
- Format (respect `.editorconfig`): `dotnet format`
- Fast search: `rg "TODO|FIXME" -n` (optional).

## Coding Style & Naming Conventions
- C# 10+; 4-space indentation; UTF-8 files; file-scoped namespaces.
- Enable nullable (`<Nullable>enable</Nullable>`). Prefer `var` when type is obvious.
- Naming: PascalCase for types/methods; camelCase for locals/params; `_camelCase` for private fields; async methods end with `Async`.
- Keep classes small; favor composition; avoid static state. Follow analyzers and fix warnings before PR.

## Testing Guidelines
- Framework: xUnit (test projects end with `.Tests`).
- Name tests `MethodUnderTest_Condition_Expected()`; structure Arrange–Act–Assert.
- Aim for ≥ 80% coverage of core logic.
- Commands: `dotnet test --logger trx` or `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura` (if coverlet is configured).

## Commit & Pull Request Guidelines
- Use Conventional Commits: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:` (e.g., `feat(app): add message router`).
- One logical change per commit; keep diffs focused; no unrelated formatting.
- PRs must include: purpose/summary, linked issues, testing notes, and screenshots for UI changes.
- Ensure CI passes locally (`dotnet build`, `dotnet test`, `dotnet format`) before opening PR.

## Security & Configuration Tips
- Never commit secrets. Use environment variables or `dotnet user-secrets` for local dev.
- Provide safe defaults via `appsettings.Development.json.example` when needed.

## Agent-Specific Instructions
- Scope: This AGENTS.md applies to the entire repo.
- Keep patches minimal; do not change line endings or unrelated files.
- Prefer `apply_patch`; avoid mass refactors; update docs when behavior changes.
