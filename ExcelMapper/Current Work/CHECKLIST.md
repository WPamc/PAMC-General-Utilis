# Mapping Configuration Generator - Initial Implementation

## Aim and Metadata
- Aim: Build a WinForms utility to generate mapping strings for `DataTableFieldMapper` by linking hardcoded SQL source fields to Excel table headers.
- Creation date: 2026-05-11
- Last updated: 2026-05-12
- Scope target: `C:\code\proto\ExcelMapper`

## Phase 1: Data Modeling (Hardcoded Metadata)
- [x] 1. Create `Models\SqlSource.cs` to define the structure of a SQL data source.
  - Status: Done (2026-05-11)
  - Issue: Need a formal way to represent the two gatherers (Claims/Members).
  - Evidence: `Models\SqlSource.cs` created.
- [x] 2. Create `Data\HardcodedSourceRegistry.cs` to host the field lists for Claims and Members.
  - Status: Done (2026-05-11)
  - Issue: Decouple the metadata from the UI logic.
  - Evidence: `Data\HardcodedSourceRegistry.cs` created with all fields from `agents.md`.

## Phase 2: UI Foundation (Source Selection)
- [x] 3. Update `MainForm.cs` to include a `ComboBox` for Source Selection (Claims vs. Members).
  - Status: Done (2026-05-11)
  - Issue: Allow user to pick the starting dataset.
  - Evidence: Controls added to `MainForm.Designer.cs` and loaded in `MainForm.cs`.
- [x] 4. Add a `ListBox` (or `ListView`) to display "Available SQL Fields" based on the selection.
  - Status: Done (2026-05-11)
  - Issue: Visualize the source fields for mapping.
  - Evidence: `lstFields` added and populated.
- [x] 5. Implement selection logic to refresh the field list when the source changes.
  - Status: Done (2026-05-11)
  - Issue: Dynamic UI updates based on data source.
  - Evidence: `cmbSources_SelectedIndexChanged` implemented.

## Phase 3: Excel Discovery (Target Headers)
- [x] 6. Add "Select Excel File" button and folder/file browser.
  - Status: Done (2026-05-11)
  - Issue: Input path for the target template.
  - Evidence: `btnBrowse` and `txtExcelPath` implemented.
- [ ] 7. Implement NPOI logic to extract headers from the first sheet or selected table.
  - Status: Reopened (2026-05-12)
  - Issue: Identify mapping targets from the actual selected sheet/table or detected worksheet region.
  - Evidence: `LoadExcelHeaders` uses NPOI, but currently only reads `workbook.GetSheetAt(0)` and `sheet.GetRow(0)`; Phase 6 defines the remaining robust discovery work.

## Phase 4: Mapping & Generation
- [x] 8. Implement the mapping grid (SQL Field -> Excel Header).
  - Status: Done (2026-05-11)
  - Issue: Core mapping interface.
  - Evidence: `dgvMapping` implemented with double-click support from both lists.
- [x] 9. Implement the "Generate DSL" button to produce the semicolon-separated string.
  - Status: Done (2026-05-11)
  - Issue: Final output requirement.
  - Evidence: `btnGenerate_Click` produces the semicolon-separated mapping string.
- [x] 10. Add support for "Composite Expressions" in the mapping logic.
  - Status: Done (2026-05-11)
  - Issue: Handle {FIELD1} {FIELD2} requirement.
  - Evidence: Fields are wrapped in `{}` when added to the mapping grid.

## New Issues

- [x] NI1. Missing Resource Disposal for Excel Workbook
  - Status: Done (2026-05-11)
  - Severity: Low
  - Issue: `XSSFWorkbook` in `LoadExcelHeaders` is not wrapped in a `using` block, potentially causing memory leaks or file locks.
  - Evidence: `MainForm.cs` line 56 now uses a nested `using` block.
- [ ] NI2. Excel Header Discovery Does Not Match Template Structure
  - Status: Open (2026-05-12)
  - Severity: High
  - Issue: `LoadExcelHeaders` reads only the first row of the first sheet, but `Templates\Reporting2026-Export-Template.xlsx` stores report metadata in rows 1-3 and target headers on row 5.
  - Evidence: Template analysis found all 11 sheets frozen at `A6` / row 5, with row 5 containing headers; `MainForm.cs` currently calls `sheet.GetRow(0)`.
- [ ] NI3. Template Uses Worksheet Regions, Not Formal Excel Tables
  - Status: Open (2026-05-12)
  - Severity: High
  - Issue: NPOI `XSSFSheet.GetTables()` alone will not discover the template tables because the workbook has no `xl/tables` parts and each sheet has zero `tableParts`.
  - Evidence: `Reporting2026-Export-Template.xlsx` contains worksheet XML only for the report layouts; no formal Excel ListObject table definitions were found.
- [ ] NI4. No Helper Layer Exists for Excel Table/Header Detection
  - Status: Open (2026-05-12)
  - Severity: Medium
  - Issue: Excel discovery is embedded directly in `MainForm.LoadExcelHeaders`, making it hard to support formal tables, worksheet-region fallback, frozen-pane heuristics, merged-cell headings, and 2-3 row header flattening.
  - Evidence: Code search found only `LoadExcelHeaders`; no existing helper functions/classes for `GetTables`, `XSSFTable`, `DataFormatter`, merged-cell lookup, pane detection, or header-row detection.
- [ ] NI5. Generated DSL Delimiter May Not Match Downstream Format
  - Status: Open (2026-05-12)
  - Severity: Medium
  - Issue: The stated target format is `ExcelCol=SQLCol;...`, but `btnGenerate_Click` joins mappings with `"; "`, adding spaces after semicolons.
  - Evidence: `MainForm.cs` uses `string.Join("; ", mappings)`.
- [ ] NI6. Empty Mapping Output Clipboard Handling Is Fragile
  - Status: Open (2026-05-12)
  - Severity: Low
  - Issue: Generating with no valid mapping rows still reports success and attempts to copy an empty output string to the clipboard.
  - Evidence: `btnGenerate_Click` always calls `Clipboard.SetText(txtOutput.Text)` after generation.
- [ ] NI7. No Automated Verification for Excel Discovery
  - Status: Open (2026-05-12)
  - Severity: Medium
  - Issue: Phase 6 will introduce heuristic table/header detection, but the repository currently has no test project or test harness to verify the 11-sheet template behavior.
  - Evidence: Repository file scan found only `ExcelMapper.csproj`; no test/spec project files were present.
- [ ] NI8. Hardcoded First-Sheet Logic Blocks Multi-Sheet Templates
  - Status: Open (2026-05-12)
  - Severity: High
  - Issue: `MainForm.LoadExcelHeaders` and `MainForm_Shown` are hardcoded to `workbook.GetSheetAt(0)`, making the other 10 sheets in the default template inaccessible.
  - Evidence: `MainForm.cs` lines 58 and 139.

## Phase 5: UX & Usability Enhancements
- [x] 11. Add instructional labels for "Double-click" actions to guide the user.
  - Status: Done (2026-05-11)
  - Issue: Discoverability of mapping actions.
  - Evidence: Labels now include "(Double-click to map/add)".
- [x] 12. Group UI controls into logical panels (Source, Target, Mapping) for better visual flow.
  - Status: Done (2026-05-11)
  - Issue: Screen looks like a collection of "empty boxes".
  - Evidence: Controls wrapped in numbered `GroupBox` containers.
- [x] 13. Implement a default "Select Source" prompt in the ComboBox.
  - Status: Done (2026-05-11)
  - Issue: List starts empty without guidance.
  - Evidence: "--- Select SQL Source ---" added to `HardcodedSourceRegistry` load logic.
- [x] 14. Add a status bar or feedback label for user actions.
  - Status: Done (2026-05-11)
  - Issue: Lack of feedback on file load or generation.
  - Evidence: `StatusStrip` added and updated in all major event handlers.
- [x] 15. Auto-load default template on `Form.Shown`.
  - Status: Done (2026-05-11)
  - Issue: Launching into an empty state requires manual browsing for the primary template.
  - Evidence: `MainForm_Shown` implemented and wired in `MainForm.Designer.cs`.

## Phase 6: Robust Excel Table and Header Discovery
- [x] 16. Add an Excel discovery helper/model layer.
  - Status: Done (2026-05-11)
  - Issue: `MainForm` needs structured results for sheet name, detected table/region name, header row range, column range, and flattened header labels.
  - Evidence: `Services\ExcelSchemaService.cs` implemented.
- [ ] 17. Detect formal Excel tables when present.
  - Status: Pending
  - Issue: Future templates may contain actual Excel ListObjects.
- [ ] 18. Add worksheet-region fallback for templates without formal tables.
  - Status: Pending
- [x] 19. Support 2-3 row headings and merged-cell labels.
  - Status: Done (2026-05-11)
  - Issue: Some templates may have grouped headings spanning multiple rows or merged cells.
  - Evidence: `ExcelSchemaService.DetectHeaders` handles merged support and 3-row flattening.
- [ ] 20. Expose sheet/table selection in the UI.
  - Status: Pending
  - Issue: The current UI loads only the first sheet, but the template contains 11 report sheets with distinct target headers.
  - Requirement: User should be able to select a sheet/detected table or region before mapping; changing selection refreshes target headers and clears or confirms existing mappings.
- [ ] 21. Verify header discovery against `Reporting2026-Export-Template.xlsx`.
  - Status: Pending
  - Issue: The detector needs concrete coverage for the default template.
  - Requirement: Build succeeds and discovery returns the expected row-5 headers for all 11 sheets, including wide sheets such as `IFRS17 Premiums` through column `AP`.

## Phase 7: Diagnostic Logging & Header Correction
- [x] 22. Initialize Serilog in `Program.cs` for file-based diagnostic logging.
  - Status: Done (2026-05-11)
  - Issue: Blind debugging of header discovery is inefficient.
  - Evidence: `Program.cs` configured to roll log files in `\logs`.
- [x] 23. Add comprehensive trace points to `ExcelSchemaService.cs`.
  - Status: Done (2026-05-11)
  - Issue: Need visibility into row scanning and merged cell redirection.
  - Evidence: `Log.Verbose/Debug` added to all discovery logic.
- [ ] 24. Analyze logs to identify "Wrong Headers" root cause.
  - Status: Pending
- [ ] 25. Fix identified heuristic defects and verify against template.
  - Status: Pending

## New Clashes

- [ ] NC1. Phase 3 Completion Status Conflicted With Template Reality
  - Status: Open (2026-05-12)
  - Issue: Phase 3 item 7 was marked Done, but the completed implementation only handles a first-sheet, first-row case.
  - Resolution Path: Item 7 has been reopened and Phase 6 is now the active requirement for robust template discovery.
