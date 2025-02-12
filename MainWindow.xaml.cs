using System;
using System.Windows;

namespace eMonkey
{
    public partial class MainWindow : Window
    {
        private readonly PatientVisitViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new PatientVisitViewModel();
            DataContext = _viewModel;

            //get handle from a window with Title starting with "eCW"
            var handle = WindowHelper.GetWindowHandle("eCW");


            _viewModel.LoadPatientVisits(new IntPtr(handle)); // Example handle
        }
    }
}
