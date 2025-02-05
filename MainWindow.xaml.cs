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
            _viewModel.LoadPatientVisits(new IntPtr(66746)); // Example handle
        }
    }
}
