using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Serilog;

namespace ExcelMapper.Services
{
    public static class ExcelSchemaService
    {
        private struct HeaderSpan
        {
            public int FirstCol { get; }
            public int LastCol { get; }
            public HeaderSpan(int firstCol, int lastCol)
            {
                FirstCol = firstCol;
                LastCol = lastCol;
            }
        }

        public static IReadOnlyList<string> DetectHeaders(ISheet sheet)
        {
            Log.Information("Starting header detection for sheet: {SheetName}", sheet.SheetName);
            var formatter = new DataFormatter();

            int headerEndRow = FindHeaderEndRow(sheet);
            Log.Debug("Heuristic identified header end row: {RowIndex}", headerEndRow);

            int firstHeaderRow = Math.Max(0, headerEndRow - 2);
            Log.Debug("Scanning header range from row {Start} to {End}", firstHeaderRow, headerEndRow);

            var span = FindHeaderSpan(sheet, headerEndRow, formatter);
            Log.Debug("Identified header column span: {FirstCol} to {LastCol}", span.FirstCol, span.LastCol);

            var headers = new List<string>();

            for (int col = span.FirstCol; col <= span.LastCol; col++)
            {
                var parts = new List<string>();

                for (int rowIndex = firstHeaderRow; rowIndex <= headerEndRow; rowIndex++)
                {
                    string text = GetCellTextWithMergedSupport(sheet, rowIndex, col, formatter);

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        parts.Add(text.Trim());
                    }
                }

                string header = string.Join(" / ", parts.Distinct());

                if (!string.IsNullOrWhiteSpace(header))
                {
                    Log.Verbose("Column {ColIndex} mapped to header: '{Header}'", col, header);
                    headers.Add(header);
                }
                else
                {
                    Log.Warning("Column {ColIndex} resulted in an empty header.", col);
                }
            }

            Log.Information("Detection complete. Found {Count} headers.", headers.Count);
            return headers;
        }

        private static int FindHeaderEndRow(ISheet sheet)
        {
            Log.Verbose("Scanning for header end row starting from sheet first row: {FirstRow}", sheet.FirstRowNum);
            // Simple heuristic:
            // Find the first non-empty row and treat it as the last header row.
            for (int rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);

                if (row == null)
                    continue;

                bool hasAnyText = false;
                foreach (var cell in row.Cells)
                {
                    if (cell != null && cell.CellType != CellType.Blank && !string.IsNullOrWhiteSpace(cell.ToString()))
                    {
                        hasAnyText = true;
                        break;
                    }
                }

                if (hasAnyText)
                {
                    Log.Verbose("Found non-empty row at {RowIndex}", rowIndex);
                    return rowIndex;
                }
            }

            Log.Warning("No non-empty rows found in sheet.");
            return 0;
        }

        private static HeaderSpan FindHeaderSpan(
            ISheet sheet,
            int headerEndRow,
            DataFormatter formatter)
        {
            IRow row = sheet.GetRow(headerEndRow);

            if (row == null)
            {
                Log.Warning("Header end row {RowIndex} is null.", headerEndRow);
                return new HeaderSpan(0, 0);
            }

            int firstCol = -1;
            int lastCol = -1;

            Log.Verbose("Calculating column span for row {RowIndex}", headerEndRow);

            for (int col = row.FirstCellNum; col < row.LastCellNum; col++)
            {
                string text = GetCellTextWithMergedSupport(sheet, headerEndRow, col, formatter);

                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (firstCol == -1)
                        firstCol = col;

                    lastCol = col;
                }
            }

            if (firstCol == -1)
            {
                Log.Warning("No non-empty columns found in header row {RowIndex}.", headerEndRow);
                return new HeaderSpan(0, 0);
            }

            return new HeaderSpan(firstCol, lastCol);
        }

        private static string GetCellTextWithMergedSupport(
            ISheet sheet,
            int rowIndex,
            int colIndex,
            DataFormatter formatter)
        {
            IRow row = sheet.GetRow(rowIndex);
            ICell cell = row != null ? row.GetCell(colIndex) : null;

            if (cell != null && cell.CellType != CellType.Blank)
            {
                return formatter.FormatCellValue(cell);
            }

            // If the cell is blank/null, check whether it belongs to a merged region.
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress region = sheet.GetMergedRegion(i);

                bool isInsideMergedRegion =
                    rowIndex >= region.FirstRow &&
                    rowIndex <= region.LastRow &&
                    colIndex >= region.FirstColumn &&
                    colIndex <= region.LastColumn;

                if (!isInsideMergedRegion)
                    continue;

                IRow firstRow = sheet.GetRow(region.FirstRow);
                ICell firstCell = firstRow != null ? firstRow.GetCell(region.FirstColumn) : null;

                if (firstCell != null)
                {
                    Log.Verbose("Cell ({Row},{Col}) redirected to merged parent ({PRow},{PCol}): '{Value}'", 
                        rowIndex, colIndex, region.FirstRow, region.FirstColumn, firstCell.ToString());
                    return formatter.FormatCellValue(firstCell);
                }
            }

            return string.Empty;
        }
    }
}
