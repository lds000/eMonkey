using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;

public class PatientVisitExtractor
{
    public static List<PatientVisit> GetPatientVisits(IntPtr chromeWindowHandle)
    {
        List<PatientVisit> patientVisits = new List<PatientVisit>();
        AutomationElement chromeWindow = AutomationElement.FromHandle(chromeWindowHandle);

        if (chromeWindow == null)
        {
            Console.WriteLine("❌ Unable to find Chrome Legacy Window.");
            return patientVisits;
        }

        // Get all DataItem elements (table cells)
        var dataItems = chromeWindow.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));

        // Get all Checkbox elements
        var checkboxes = chromeWindow.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox));

        Dictionary<int, Dictionary<int, string>> tableData = new Dictionary<int, Dictionary<int, string>>();
        Dictionary<int, AutomationElement> checkboxLookup = new Dictionary<int, AutomationElement>();

        // Extract DataItems
        foreach (AutomationElement element in dataItems)
        {
            string text = element.Current.Name?.Trim() ?? "";
            int row = -1, column = -1;

            if (element.TryGetCurrentPattern(GridItemPattern.Pattern, out object gridPatternObj))
            {
                var gridItemPattern = (GridItemPattern)gridPatternObj;
                row = gridItemPattern.Current.Row;
                column = gridItemPattern.Current.Column;
            }

            if (row >= 0 && column >= 0)
            {
                if (!tableData.ContainsKey(row))
                    tableData[row] = new Dictionary<int, string>();

                tableData[row][column] = text;
            }
        }

        // Improved Checkbox Matching: Find parent row for each checkbox
        foreach (AutomationElement checkbox in checkboxes)
        {
            AutomationElement parentRow = TreeWalker.RawViewWalker.GetParent(checkbox);

            if (parentRow != null && parentRow.TryGetCurrentPattern(GridItemPattern.Pattern, out object gridPatternObj))
            {
                var gridItemPattern = (GridItemPattern)gridPatternObj;
                int row = gridItemPattern.Current.Row;

                if (!checkboxLookup.ContainsKey(row))
                {
                    checkboxLookup[row] = checkbox;
                }
            }
        }

        // Debugging: List all found checkboxes
        Console.WriteLine($"✅ Found {checkboxes.Count} checkboxes in the UI.");

        foreach (AutomationElement checkbox in checkboxes)
        {
            string name = checkbox.Current.Name?.Trim() ?? "Unnamed Checkbox";
            string automationId = checkbox.Current.AutomationId ?? "No ID";
            Rect boundingRect = checkbox.Current.BoundingRectangle;
            string state = "Unknown";

            if (checkbox.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObj))
            {
                var togglePattern = (TogglePattern)togglePatternObj;
                state = togglePattern.Current.ToggleState == ToggleState.On ? "Checked" : "Unchecked";
            }

            Console.WriteLine($"Checkbox: {name} | ID: {automationId} | State: {state} | Bounds: {boundingRect}");
        }

        // Convert extracted data into PatientVisit objects
        foreach (var row in tableData.OrderBy(r => r.Key).Skip(1))
        {
            var columns = row.Value;
            int rowIndex = row.Key;

            // Improved: Assign checkbox element, fallback to nearest checkbox if null
            if (!checkboxLookup.TryGetValue(rowIndex, out var checkboxElement))
            {
                checkboxElement = checkboxes
                    .Cast<AutomationElement>()
                    .OrderBy(cb => Math.Abs(cb.Current.BoundingRectangle.Y - rowIndex * 30)) // Approximate row matching
                    .FirstOrDefault();
            }

            bool isChecked = false;
            if (checkboxElement != null && checkboxElement.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObj))
            {
                var togglePattern = (TogglePattern)togglePatternObj;
                isChecked = togglePattern.Current.ToggleState == ToggleState.On;
            }

            if (checkboxElement == null)
                Console.WriteLine($"⚠️ Warning: Null checkbox for row {rowIndex} (Potential Missing UI Element)");

            // Create the PatientVisit object
            patientVisits.Add(new PatientVisit
            {
                AppointmentTime = columns.TryGetValue(8, out string appointmentTime) ? appointmentTime : "",
                Provider = columns.TryGetValue(11, out string provider) ? provider : "",
                PatientName = columns.TryGetValue(9, out string patientName) ? patientName : "",
                Insurance = columns.TryGetValue(10, out string insurance) ? insurance : "",
                VisitReason = columns.TryGetValue(12, out string visitReason) ? visitReason : "",
                Gender = columns.TryGetValue(13, out string gender) ? gender : "",
                Age = columns.TryGetValue(14, out string age) ? age : "",
                VisitStatus = columns.TryGetValue(15, out string visitStatus) ? visitStatus : "",
                Duration = columns.TryGetValue(18, out string duration) ? duration : "",
                Room = columns.TryGetValue(20, out string room) ? room : "",
                CycleTime = columns.TryGetValue(21, out string cycleTime) ? cycleTime : "",
                IsChecked = isChecked,
                CheckboxElement = checkboxElement
            });
        }

        return patientVisits;
    }
}
