using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eMonkey
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr chromeLegacyHandle = new IntPtr(66746);  // Replace with your handle

        public MainWindow()
        {
            InitializeComponent();
            LoadPatientVisits();
        }


        private void LoadPatientVisits()
        {
            List<PatientVisit> patientVisits = PatientVisitExtractor.GetPatientVisits(chromeLegacyHandle);
            PatientVisitList.ItemsSource = patientVisits;
        }


        private void ToggleCheckbox(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).DataContext is PatientVisit visit && visit.CheckboxElement != null)
            {
                if (visit.CheckboxElement.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObj))
                {
                    var togglePattern = (TogglePattern)togglePatternObj;
                    togglePattern.Toggle();
                }
            }
        }
    }
}

