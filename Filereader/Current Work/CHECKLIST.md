# Filereader High DPI Stabilization

## Aim and Metadata
- Aim: Make Filereader crisp and layout-stable on high-DPI and mixed-DPI Windows displays.
- Creation date: 2026-05-12
- Last updated: 2026-05-12
- Scope target: `App.config`, `MainForm.cs`, `MainForm.Designer.cs`, `LogSettingsForm.cs`, `Toastr.Winforms/Toast.cs`, `Toastr.Winforms/Toast.Designer.cs`, `Properties/Settings.settings`

## Completed

- [x] 1. Analyze Current DPI Posture
  - Status: Done (2026-05-12)
  - Issue: Need to classify Filereader before changing DPI behavior.
  - Evidence: Project targets .NET Framework 4.7.2 in `Filereader.csproj`; startup uses classic `Application.EnableVisualStyles()` and `Application.Run(new MainForm())` in `Program.cs`; no manifest or app.config DPI declaration was found.

## Next Phase

- [x] 2. Choose One DPI Configuration Path
  - Status: Done (2026-05-12)
  - Issue: Filereader currently has no explicit DPI awareness declaration, so Windows likely bitmap-scales the app on high-DPI displays.
  - Evidence: App.config updated with System.Windows.Forms.ApplicationConfigurationSection (DpiAwareness=PerMonitorV2).
  - Plan: For .NET Framework 4.7.2, prefer `App.config` with `System.Windows.Forms.ApplicationConfigurationSection` and `DpiAwareness=PerMonitorV2`.

- [x] 3. Resolve MainForm AutoScaleMode Conflict
  - Status: Done (2026-05-12)
  - Issue: `MainForm.Designer.cs` declares `AutoScaleMode.Font`, but the `MainForm` constructor changes it to `AutoScaleMode.Dpi` after `InitializeComponent()`.
  - Evidence: MainForm.cs constructor override commented out to match MainForm.Designer.cs.
  - Plan: Pick one strategy and keep it consistent. For this business/data-grid WinForms app, start by keeping `Font` unless DPI-mode testing proves `Dpi` is safer.

## Subsequent Phases

- [ ] 4. Modernize MainForm Top Layout
  - Status: Pending
  - Issue: The top controls are manually positioned with fixed sizes and may crowd or clip after DPI awareness is enabled.
  - Evidence: `MainForm.Designer.cs` top-row controls around lines ~128-192 use fixed `Location` and `Size`.
  - Plan: Replace fragile fixed positioning with layout containers, docking, autosizing, or wider responsive constraints while preserving existing behavior.

- [ ] 5. Stabilize Master Filter Bar Layout
  - Status: Pending
  - Issue: The runtime-created filter bar uses fixed height, fixed button widths, fixed spacers, and left-docked controls.
  - Evidence: `MainForm.cs` lines ~440-488.
  - Plan: Use a DPI-safe layout pattern so `Filter`, `Search`, `Clear`, and `Columns...` remain readable at 125%-200%.

- [ ] 6. Rework Log Settings Dialog Layout
  - Status: Pending
  - Issue: `LogSettingsForm` has the most obvious DPI clipping risk because it uses fixed form size, fixed table height, absolute row heights, and manually positioned path controls.
  - Evidence: `LogSettingsForm.cs` lines ~28, ~45, and ~60-66.
  - Plan: Convert the dialog to a fully docked/table-based layout with autosizing where practical.

- [ ] 7. Review Saved Form State Defaults
  - Status: Pending
  - Issue: Default saved `FormSize` is much smaller than the designer client size and can make startup cramped under high scaling.
  - Evidence: `Properties/Settings.settings` default `FormSize` is `800, 450`; `MainForm.Designer.cs` client size is `1432, 729`.
  - Plan: Decide whether to reset defaults, clamp restored size, or add a minimum size.

- [ ] 8. Review Toast DPI Behavior
  - Status: Pending
  - Issue: Toast UI uses fixed pixel layout, fixed image sizes, and hardcoded 10px placement offsets.
  - Evidence: `Toastr.Winforms/Toast.Designer.cs` lines ~31-66 and `Toastr.Winforms/Toast.cs` lines ~94-145.
  - Plan: Scale placement offsets and review icon/image quality for high-DPI displays.

## Validation

- [ ] 9. Build Filereader
  - Status: Pending
  - Issue: Any DPI/layout edits must still compile.
  - Evidence: Project instruction requires `dotnet build Filereader.csproj` from `C:\code\proto\Filereader`.

- [ ] 10. Run DPI Visual Test Matrix
  - Status: Pending
  - Issue: DPI awareness can make the app crisp while exposing layout regressions.
  - Evidence: Required screens include main form, Log Settings, Manage Formats, master/detail grids, filter bar, and toast notifications.
  - Plan: Test launch at 100%, 125%, 150%, and 200%; if mixed-DPI monitors are available, test launching on each monitor and moving the main form before opening dialogs.

## New Issues

- [ ] NI1. Krypton Control DPI Compatibility Unknown
  - Status: Open (2026-05-12)
  - Severity: Medium
  - Issue: Filereader uses Krypton controls in the wizard and grids; their PerMonitorV2 behavior should be verified with the referenced package versions.
  - Evidence: `packages.config` references `Krypton.Toolkit` 105.26.4.110 and `Krypton.Toolkit.Suite.Extended.DataGridView` 105.26.1.19.

## New Clashes

- [ ] NC1. Runtime AutoScale Override Conflicts With Designer State
  - Status: Open (2026-05-12)
  - Issue: The designer and constructor disagree about scaling mode, making future designer edits and runtime behavior harder to predict.
  - Evidence: `MainForm.Designer.cs` uses `AutoScaleMode.Font`; `MainForm.cs` changes the form to `AutoScaleMode.Dpi` after initialization.
