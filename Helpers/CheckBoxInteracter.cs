﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace eMonkey
{
    public class CheckboxInteractor
    {
        public static void ToggleCheckbox(CheckboxItemM checkbox)
        {
            if (checkbox.Element == null)
            {
                Console.WriteLine($"❌ Checkbox element is NULL: {checkbox.Name}");
                return;
            }

            if (checkbox.Element.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObject))
            {
                var togglePattern = (TogglePattern)togglePatternObject;
                togglePattern.Toggle(); // Clicks the checkbox
                Console.WriteLine($"✅ Toggled Checkbox: {checkbox.Name}");
            }
            else
            {
                Console.WriteLine($"❌ TogglePattern NOT SUPPORTED for: {checkbox.Name}");
            }
        }
    }
}
