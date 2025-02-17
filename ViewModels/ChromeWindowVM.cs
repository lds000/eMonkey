using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace eMonkey
{
    public class ChromeWindowVM : INotifyPropertyChanged
    {
        private ChromeWindowM _chromeWindow;
        private PatientVisitM _selectedVisit;

        public ChromeWindowVM(IntPtr windowHandle)
        {
            _chromeWindow = new ChromeWindowM(windowHandle);
            PatientVisits = new ObservableCollection<PatientVisitM>();
            LoadPatientVisits();
        }

        public ObservableCollection<PatientVisitM> PatientVisits
        {
            get; set;
        }

        public PatientVisitM SelectedVisit
        {
            get => _selectedVisit;
            set
            {
                _selectedVisit = value;
                OnPropertyChanged(nameof(SelectedVisit));
            }
        }

        public ICommand CheckBoxCommand => new RelayCommand(OnCheckBoxChecked);

        private void OnCheckBoxChecked(object parameter)
        {
            if (parameter is PatientVisitM patientVisit)
            {
                // Custom behavior logic
            }
        }

        private void LoadPatientVisits()
        {
            PatientVisits.Clear();
            foreach (var visit in _chromeWindow.GetPatientVisits(OnCheckboxStatusChanged))
            {
                PatientVisits.Add(visit);
            }
        }

        private void OnCheckboxStatusChanged(PatientVisitM visit, bool isChecked)
        {
            if (visit != null)
            {
                visit.IsChecked = isChecked;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
