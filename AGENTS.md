# AGENTS.md

## Workspace Profile
- Root: `C:\code\proto`.
- This folder is a multi-project workspace, not a single Git repository.
- Primary shell on this machine: PowerShell.
- Prefer targeted project work over broad workspace changes.

## Projects
- `Filereader`: .NET Framework 4.7.2 WinForms parser/viewer.
  - Main project: `Filereader\Filereader.csproj`.
  - Main form logic: `Filereader\MainForm.cs`.
  - Project-specific instructions: `Filereader\AGENTS.md`.
- `ExcelMapper`: .NET Framework 4.8.1 WinForms utility for generating Excel mapping DSL.
  - Main project: `ExcelMapper\ExcelMapper.csproj`.
  - Main form logic: `ExcelMapper\MainForm.cs`.
  - Project-specific instructions: `ExcelMapper\agents.md`.
- `Portal`: Angular/Clarity sample application.
  - Package file: `Portal\package.json`.
  - Angular config: `Portal\angular.json`.
  - Main source: `Portal\src`.
- `Filereader\Toastr.Winforms`: local toast notification library used by `Filereader`.
- `Toastr.Winforms`: separate upstream/demo toast project.
- `Extended-Toolkit-master`: large local Krypton Toolkit source checkout. Treat as reference/vendor code unless explicitly asked to modify it.

## Instruction Precedence
- Read this root file first for workspace-level boundaries.
- When working inside `Filereader`, also read `Filereader\AGENTS.md` and follow it.
- When working inside `ExcelMapper`, also read `ExcelMapper\agents.md` and follow it.
- Project-specific instructions override this root file for that project.

## Build And Verification
- Build `Filereader` from `C:\code\proto\Filereader` with:
  - `dotnet build Filereader.csproj`
- Build `ExcelMapper` from `C:\code\proto\ExcelMapper` with:
  - `dotnet build`
- Build the standalone toast project from `C:\code\proto\Toastr.Winforms` with:
  - `dotnet build Toastr.Winforms\Toastr.Winforms.csproj`
- Build `Portal` from `C:\code\proto\Portal` with:
  - `npx ng build`
- `Portal` currently has no `npm run build` script.
- Do not run long GUI/manual workflows unless the user asks for them.

## Search And Performance
- Use `rg` / `rg --files` first when available.
- Avoid broad recursive scans through generated, dependency, and vendor folders:
  - `.vs\`
  - `bin\`
  - `obj\`
  - `packages\`
  - `node_modules\`
  - `dist\`
  - `artifacts\`
  - `Extended-Toolkit-master\`
- Use `multi_tool_use.parallel` for independent reads such as file listings, `Get-Content`, and targeted searches.

## External And Vendor Boundaries
- Do not modify code under `C:\code\pamc_c` unless the user explicitly asks.
- Do not modify `PAMC.Formats`, `PAMC.Utilities.*`, or `PAMC.DateTimeUtils` as part of normal work in this workspace.
- Do not modify `Extended-Toolkit-master` unless the task is specifically about the Krypton Toolkit source checkout.
- Treat generated output folders as build artifacts. Do not delete or mass-edit them during normal analysis or implementation.

## Editing Discipline
- Keep edits scoped to the requested project and behavior.
- Preserve user changes and unrelated local modifications.
- Prefer existing project patterns over new abstractions.
- For WinForms projects, keep behavior in the form code-behind and edit `.Designer.cs` files only for necessary UI/layout changes.
- Do not move file IO, parsing, or business logic into `InitializeComponent()`.
- Use standard WinForms controls unless the project already establishes a third-party control for the same purpose or project instructions say otherwise.

## Current Work Checklists
- `Filereader` and `ExcelMapper` may have `Current Work\CHECKLIST.md` files.
- Read the relevant checklist before changing behavior governed by that project.
- Keep checklist status aligned only when the user asks for checklist-governed implementation or reconciliation.

## Known Local Coupling
- `Filereader` references `PAMC.Formats` from `C:\code\pamc_c`.
- `ExcelMapper` references `PAMC.Utilities.*` from `C:\code\pamc_c`.
- `ExcelMapper` also references many NuGet packages through `..\Filereader\packages`.
- These projects build on this machine, but the workspace is not fully portable without those sibling/local dependencies.
