using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;

public class TableDataRow
{
    public int RowIndex
    {
        get; set;
    }
    public Dictionary<int, string> Columns { get; set; } = new Dictionary<int, string>();

    public override string ToString()
    {
        return $"Row {RowIndex}: " + string.Join(" | ", Columns.OrderBy(c => c.Key).Select(c => c.Value));
    }
}

public class DataTableExtractor
{
    public static List<TableDataRow> ExtractTableData(IntPtr chromeWindowHandle)
    {
        List<TableDataRow> tableData = new List<TableDataRow>();
        AutomationElement chromeWindow = AutomationElement.FromHandle(chromeWindowHandle);

        if (chromeWindow == null)
        {
            Console.WriteLine("❌ Unable to find Chrome Legacy Window.");
            return tableData;
        }

        // Get all DataItem elements
        var dataItems = chromeWindow.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));

        Dictionary<int, TableDataRow> rowLookup = new Dictionary<int, TableDataRow>();

        foreach (AutomationElement element in dataItems)
        {
            string text = element.Current.Name?.Trim() ?? "";
            int row = -1, column = -1;

            // Extract row and column index if available
            if (element.TryGetCurrentPattern(GridItemPattern.Pattern, out object gridPatternObj))
            {
                var gridItemPattern = (GridItemPattern)gridPatternObj;
                row = gridItemPattern.Current.Row;
                column = gridItemPattern.Current.Column;
            }

            // Only add meaningful data (avoid empty rows)
            if (row >= 0 && column >= 0 && !string.IsNullOrEmpty(text))
            {
                if (!rowLookup.ContainsKey(row))
                {
                    rowLookup[row] = new TableDataRow { RowIndex = row };
                }

                rowLookup[row].Columns[column] = text;
            }
        }

        tableData = rowLookup.Values.OrderBy(r => r.RowIndex).ToList();
        return tableData;
    }
}
