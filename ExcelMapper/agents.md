# Agent Roles & Project Context

## Repository Profile
- Project: `ExcelMapper`, a WinForms utility for mapping SQL sources to Excel DSL.
- Runtime target: .NET Framework 4.8.1.
- Primary shell: PowerShell.
- Main project file: `ExcelMapper.csproj`.
- Main form logic: `MainForm.cs`.
- Designer file: `MainForm.Designer.cs`.

## Primary Objective: Mapping Configuration Generator
This application is a utility designed to generate the `mapping` string (e.g., `ExcelCol=SQLCol;...`) required by the `DataTableFieldMapper.Map` method. Its primary purpose is to bridge static SQL data sources with arbitrary Excel table layouts.

## Workflow
1.  **Source Discovery**: Identify available fields from static SQL gatherers.
2.  **Target Discovery**: Select an Excel file and extract column headers from a specific sheet/table using NPOI.
3.  **Mapping Interface**: Provide a UI to link SQL fields (including composite/concatenated fields) to Excel headers.
4.  **Output**: Generate the final mapping string for use in downstream ETL processes.

## Current Work Source Of Truth
- Always read `Current Work/CHECKLIST.md` before changing behavior.
- Treat the checklist decisions as active requirements unless the user explicitly changes them.
- Keep checklist status aligned when completing, deferring, or discovering work.

## Performance Rules
- Prefer targeted reads over broad scans.
- Use `grep_search` / `glob` first when searching.
- Avoid searching generated or dependency folders: `.vs/`, `bin/`, `obj/`, `packages/`.
- Do not enumerate large network paths or external folders unless specifically requested.

## Build And Verification
- Use `dotnet build` from `C:\code\proto\ExcelMapper` as the primary build check.
- The app builds to `bin\Debug\ExcelMapper.exe`.
- For WinForms behavior changes, verify compile at minimum.
- Do not run long GUI/manual workflows unless the user asks for them.

## WinForms Editing Rules
- Keep behavior in `MainForm.cs`.
- Edit `MainForm.Designer.cs` only for required control/layout changes.
- Preserve designer-generated structure and avoid formatting churn in `.Designer.cs`.
- Do not move file IO or complex logic into `InitializeComponent()`.
- Existing controls should be reused before adding new controls.
- Prefer standard WinForms controls over third-party toolkits unless already established in the project.

## Data Sources (SQL Inputs)

### 1. Claims Dataset (`UnderwriterClaimsGatherer`)
Available fields:
- `HPCODE`, `MEMBID`, `SUBSSN`, `CLAIMNO`, `TBLROWID`, `OPT`, `DATERECD`, `DATEPAID`, `STATUS`, `CLAIMTYPE`, `INPATDAYS`
- `FROMDATESVC`, `TODATESVC`, `BILLED`, `NET`, `DIAGCODE`, `DIAGDESC`
- `MAIN_FIRSTNM`, `MAIN_LASTNM`, `MAIN_PATID` (Subscriber Info)
- `DEP_FIRSTNM`, `DEP_LASTNM`, `DEP_SEX`, `DEP_RLSHIP` (Member Info)
- `RELATIONSHIP_DESCR`, `VENDORNM`, `BENEFIT_DESCR`, `EXTERNAL_ADJ_DESCR`
- `PROVIDER_LASTNAME`, `PROVIDER_FIRSTNAME`
- `REASON_ADJCODE`, `REASON_COMMENTS`, `REASON_DESCR`
- `UNDERWRITER_CLAIM_STATUS`

### 2. Members Dataset (`UnderwriterMembersGatherer`)
Available fields:
- `HPCODE`, `MEMBID`, `SUBSSN`, `FIRSTNM`, `LASTNM`, `SEX`, `BIRTH`
- `RLSHIP`, `DESCR` (Relationship), `HPNAME`, `OPT`, `EMPGROUP`
- `OPFROMDT`, `OPTHRUDT`, `HPFROMDT`, `HPTHRUDT`
- `BILLRELATION1` through `BILLRELATION10`

## Change Discipline
- Only change source code when explicitly asked.
- Keep code discussions succinct and focused on intent.
- Keep edits scoped to the requested phase.

## Critical Constraints
- **PAMC Libraries**: DO NOT touch, modify, or refactor any code within the `PAMC.Utilities.*` or `PAMC.DateTimeUtils` projects. This code is external to the current scope.
- Do not refactor unrelated project structure.
- Do not delete generated output folders as part of normal work.
- If user changes are present, preserve them and work around them.
