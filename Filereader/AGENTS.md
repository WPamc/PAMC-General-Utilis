# AGENTS.md

## Repository Profile
- Project: `Filereader`, a small C# WinForms viewer.
- Runtime target: .NET Framework 4.7.2.
- Primary shell on this machine: PowerShell.
- Main project file: `Filereader.csproj`.
- Main form logic: `MainForm.cs`.
- Designer file: `MainForm.Designer.cs`.
- Test/sample file: `tests/CLMS_20260430_1.TXT`.
- Referenced format library: `..\..\pamc_c\Projects\PAMC\Libraries\PAMC.Formats\PAMC.Formats.csproj`.
- Krypton Toolkit source checkout: `C:\code\proto\Extended-Toolkit-master`.
- Use that checkout only as local reference source for Krypton behavior and examples; this app currently references Krypton through NuGet packages under `packages\`.
- Relevant Toolkit area for the multi-record viewer: `Source\Krypton Toolkit\Krypton.Toolkit.Suite.Extended.TreeGridView`, including `KryptonTreeGridView.cs`, `KryptonTreeGridNodeRow.cs`, and the TreeGridView examples.

## Current Work Source Of Truth
- Always read `Current Work/CHECKLIST.md` before changing behavior for the DataGrid multi-record viewer.
- Treat the checklist decisions as active requirements unless the user explicitly changes them.
- Keep checklist status aligned when completing, deferring, or discovering work.
- Do not implement deferred checklist items unless the user asks for that phase.

## Performance Rules
- Prefer targeted reads over broad scans.
- Use `rg` / `rg --files` first when searching.
- Avoid searching generated or dependency folders unless specifically needed:
  - `.vs/`
  - `bin/`
  - `obj/`
  - `packages/`
- When inspecting the format library, search only the relevant namespace or file first, for example:
  - `C:\code\pamc_c\Projects\PAMC\Libraries\PAMC.Formats\Medihelp\ERALayout.cs`
  - `C:\code\pamc_c\Projects\PAMC\Libraries\PAMC.Formats\Utilities.cs`
- Do not enumerate large network paths or external folders from this repo unless the user gives a specific path and reason.
- Use `multi_tool_use.parallel` for independent reads such as `Get-Content`, `rg`, and directory listings.

## Build And Verification
- Use `dotnet build` from `C:\code\proto\Filereader` as the first build check.
- The app builds to `bin\Debug\Filereader.exe`.
- For WinForms behavior changes, verify compile at minimum.
- If a change touches parsing, verify against `tests\CLMS_20260430_1.TXT` when practical.
- Do not run long GUI/manual workflows unless the user asks for them.

## WinForms Editing Rules
- Keep behavior in `MainForm.cs`.
- Edit `MainForm.Designer.cs` only for required control/layout changes.
- Preserve designer-generated structure and avoid formatting churn in `.Designer.cs`.
- Do not move file IO or parsing logic into `InitializeComponent()`.
- Existing controls should be reused before adding new controls.
- **DO NOT** use Krypton controls (e.g., `KryptonButton`, `KryptonLabel`, etc.) for any new or modified UI elements unless explicitly directed by the user. 
- **EXCEPTIONS:** 
  - `KryptonDataGridView` (or `KryptonTreeGridView`) for data display.
  - Existing Krypton controls within `ConfigWizardForm.cs` are permitted and should be maintained.

## Multi-Record Viewer Requirements
- The format list is managed via `formats.json` and the "Manage Formats" Wizard.
- Do not hardcode new formats in `MainForm.cs` or `cbFormat`.
- First implementation scope (Verified):
  - Implement `Medihelp ERA` via dynamic config.
  - For `Medihelp Pay Instruction`, show a clear not-implemented message or create a config.
- Use `FileHelpers.MultiRecordEngine` driven by `FormatConfig`.
- Display parsed public fields from the FileHelpers record objects, not raw pipe positions.
- Preserve record order because hierarchy is implied by file order.
- Master/Detail view:
  - Master view shows "Claims" (grouped by PK).
  - Detail view shows "Service Lines" for the selected Master record.
- Primary Key (PK) for grouping and filtering is defined in `FormatConfig.MasterColumns`.

## Known Local Context
- `btnView_Click` parses `Medihelp ERA` and binds it to `kryptonTreeGridView1`.
- `kryptonTreeGridView1` uses hierarchical nodes and lazy loading for record fields.
- `tests\CLMS_20260430_1.TXT` begins with an `H|...` header followed by `E|...` detail rows.

## Change Discipline
- only change source code when asked.
- keep code discussions succint and non technical.
- Keep edits scoped to the requested phase.
## Critical Constraints
- **PAMC.Formats:** DO NOT touch, modify, or refactor any code within the `PAMC.Formats` assembly/project. This code is external to the current remediation scope and must remain unchanged.
- Do not refactor unrelated project structure.
- Do not change `PAMC.Formats` unless the user explicitly asks.
- Do not delete generated output folders as part of normal work.
- If user changes are present, preserve them and work around them.
