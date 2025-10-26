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

## VS2022 Notes (Persistent)
- Target: VS2022 only. VSIX manifest uses `InstallationTarget [17.0,18.0)` and `ProductArchitecture: amd64`.
- Build: Use VS2022 MSBuild, not `dotnet build`.
  - Example: `"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" NarrowIM.sln -t:Restore,Rebuild -p:Configuration=Release`
- Encoding of logs: VS output may be UTF-16/CP932. If using Git Bash, convert when viewing.
  - PowerShell: `[Console]::OutputEncoding=[Text.UTF8Encoding]::new($false)`
  - Git Bash: `... 2>&1 | iconv -f CP932 -t UTF-8 | tee build.log`
- Resources: Keep `VSPackage.resx` string-only; do not embed icons/binary.
  - Set icon via VSIX manifest instead of `IconResourceID` to avoid VSSDK cto merge issues.
  - Do not add `System.Resources.Extensions` (can break cto merge).
- EnvDTE interop: Do not copy EnvDTE/CommandBars/stdole into the VSIX (`Private=False`).
  - Avoid `EnvDTE.WindowEvents` in VS2022 (can throw `MissingMethodException`).
  - Prefer VS Shell events (e.g., `IVsMonitorSelection`, `IVsRunningDocumentTableEvents`) for MRU/activation.
- ActivityLog: Path `C:\Users\<USER>\AppData\Roaming\Microsoft\VisualStudio\17.0_*\ActivityLog.xml` (UTF-16).
  - Use `iconv -f UTF-16 -t UTF-8 ActivityLog.xml | rg -n 'type="Error"' -C 4` to inspect errors.
