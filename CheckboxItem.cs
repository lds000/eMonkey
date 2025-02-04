using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;

public class CheckboxItem
{
    public string Name
    {
        get; set;
    }
    public string AutomationId
    {
        get; set;
    }
    public string State
    {
        get; set;
    }
    public Rect BoundingRectangle
    {
        get; set;
    }
    public AutomationElement Element
    {
        get; set;
    }

    public override string ToString()
    {
        return $"☑️ Checkbox: {Name}\n" +
               $"   🆔 Automation ID: {AutomationId}\n" +
               $"   📍 Bounding Rectangle: (X: {BoundingRectangle.X}, Y: {BoundingRectangle.Y}, W: {BoundingRectangle.Width}, H: {BoundingRectangle.Height})\n" +
               $"   ✅ State: {State}\n";
    }
}

