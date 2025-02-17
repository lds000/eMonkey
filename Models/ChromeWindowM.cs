using eMonkey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using Condition = System.Windows.Automation.Condition;

/// <summary>
/// Represents a Chrome window and provides methods to interact with its elements.
/// </summary>
public class ChromeWindowM
{
    private readonly AutomationElement _chromeWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromeWindowM"/> class.
    /// </summary>
    /// <param name="windowHandle">The handle of the Chrome window.</param>
    public ChromeWindowM(IntPtr windowHandle)
    {
        var handle = WindowHelper.GetWindowHandle("eCW (");
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle.");
        }
        _chromeWindow = AutomationElement.FromHandle(windowHandle);

        // Add event handlers for various automation events
        AddAutomationEventHandlers();
    }

    /// <summary>
    /// Gets all child elements of the Chrome window.
    /// </summary>
    /// <returns>A collection of child automation elements.</returns>
    public AutomationElementCollection GetAllChildElements()
    {
        return _chromeWindow.FindAll(TreeScope.Descendants, Condition.TrueCondition);
    }

    /// <summary>
    /// Gets all data items in the Chrome window.
    /// </summary>
    /// <returns>A collection of data item automation elements.</returns>
    public AutomationElementCollection GetDataItems()
    {
        return _chromeWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
    }

    /// <summary>
    /// Gets all checkboxes in the Chrome window.
    /// </summary>
    /// <returns>A collection of checkbox automation elements.</returns>
    public AutomationElementCollection GetCheckboxes()
    {
        return _chromeWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox));
    }

    /// <summary>
    /// Gets the patient visits from the specified window handle.
    /// </summary>
    /// <param name="checkboxStatusChangedCallback">The callback to invoke when a checkbox status changes.</param>
    /// <returns>A list of patient visits.</returns>
    public List<PatientVisitM> GetPatientVisits(Action<PatientVisitM, bool> checkboxStatusChangedCallback)
    {
        List<PatientVisitM> patientVisits = new List<PatientVisitM>();

        var allChildren = GetAllChildElements();

        foreach (AutomationElement child in allChildren)
        {
            string childInfo = $"Name: {child.Current.Name}, " +
                               $"ControlType: {child.Current.ControlType.ProgrammaticName}, " +
                               $"AutomationId: {child.Current.AutomationId}";

            if (child.TryGetCurrentPattern(InvokePattern.Pattern, out object invokePatternObj))
            {
                childInfo += ", Supports InvokePattern";
            }

            try
            {
                if (child.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePatternObj))
                {
                    var valuePattern = (ValuePattern)valuePatternObj;
                    childInfo += $", Value: {valuePattern.Current.Value}";
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }

            if (child.TryGetCurrentPattern(SelectionPattern.Pattern, out object selectionPatternObj))
            {
                childInfo += ", Supports SelectionPattern";
            }

            if (child.TryGetCurrentPattern(ScrollPattern.Pattern, out object scrollPatternObj))
            {
                childInfo += ", Supports ScrollPattern";
            }

            Console.WriteLine(childInfo);
        }

        var dataItems = GetDataItems();
        var checkboxes = GetCheckboxes();

        Dictionary<int, Dictionary<int, string>> tableData = new Dictionary<int, Dictionary<int, string>>();
        Dictionary<int, AutomationElement> checkboxLookup = new Dictionary<int, AutomationElement>();

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

        foreach (var row in tableData.OrderBy(r => r.Key).Skip(1))
        {
            var columns = row.Value;
            int rowIndex = row.Key;

            if (!checkboxLookup.TryGetValue(rowIndex, out var checkboxElement))
            {
                checkboxElement = checkboxes
                    .Cast<AutomationElement>()
                    .OrderBy(cb => Math.Abs(cb.Current.BoundingRectangle.Y - rowIndex * 30))
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

            var patientVisit = new PatientVisitM
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
            };

            patientVisits.Add(patientVisit);

            if (checkboxElement != null)
            {
                AddPropertyChangedEventHandler(checkboxElement, TogglePattern.ToggleStateProperty, (sender, e) =>
                {
                    if (e.Property == TogglePattern.ToggleStateProperty)
                    {
                        var togglePattern = (TogglePattern)checkboxElement.GetCurrentPattern(TogglePattern.Pattern);
                        bool newIsChecked = togglePattern.Current.ToggleState == ToggleState.On;
                        checkboxStatusChangedCallback(patientVisit, newIsChecked);
                    }
                });
            }
        }

        return patientVisits;
    }

    /// <summary>
    /// Adds event handlers for various automation events.
    /// </summary>
    private void AddAutomationEventHandlers()
    {
        Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, _chromeWindow, TreeScope.Subtree, OnWindowOpened);
        Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, _chromeWindow, TreeScope.Subtree, OnWindowClosed);
        Automation.AddStructureChangedEventHandler(_chromeWindow, TreeScope.Subtree, OnStructureChanged);
        Automation.AddAutomationPropertyChangedEventHandler(_chromeWindow, TreeScope.Subtree, OnPropertyChanged, AutomationElementIdentifiers.NameProperty);
    }

    /// <summary>
    /// Event handler for window opened event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnWindowOpened(object sender, AutomationEventArgs e)
    {
        var element = sender as AutomationElement;
        if (element != null)
        {
            Console.WriteLine($"New window opened: {element.Current.Name}");
            // Handle the new window (popup) here
        }
    }

    /// <summary>
    /// Event handler for window closed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnWindowClosed(object sender, AutomationEventArgs e)
    {
        var element = sender as AutomationElement;
        if (element != null)
        {
            Console.WriteLine($"Window closed: {element.Current.Name}");
        }
    }

    /// <summary>
    /// Event handler for structure changed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnStructureChanged(object sender, StructureChangedEventArgs e)
    {
        var element = sender as AutomationElement;
        if (element != null)
        {
            Console.WriteLine($"Structure changed: {element.Current.Name}");
        }
    }

    /// <summary>
    /// Event handler for property changed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        var element = sender as AutomationElement;
        if (element != null)
        {
            Console.WriteLine($"Property changed: {element.Current.Name}, Property: {e.Property.ProgrammaticName}, Old Value: {e.OldValue}, New Value: {e.NewValue}");
        }
    }

    /// <summary>
    /// Adds an event handler for the window opened event.
    /// </summary>
    /// <param name="eventHandler">The event handler to add.</param>
    public void AddWindowOpenedEventHandler(AutomationEventHandler eventHandler)
    {
        Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, _chromeWindow, TreeScope.Subtree, eventHandler);
    }

    /// <summary>
    /// Adds an event handler for the property changed event on a specific element.
    /// </summary>
    /// <param name="element">The automation element to monitor.</param>
    /// <param name="property">The property to monitor.</param>
    /// <param name="eventHandler">The event handler to add.</param>
    public void AddPropertyChangedEventHandler(AutomationElement element, AutomationProperty property, AutomationPropertyChangedEventHandler eventHandler)
    {
        Automation.AddAutomationPropertyChangedEventHandler(element, TreeScope.Element, eventHandler, property);
    }
}
