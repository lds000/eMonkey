using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Automation;
using System;

namespace eMonkey
{
    public class PatientVisitViewModel : INotifyPropertyChanged
    {
        public ICommand CheckBoxCommand
        {
            get;
        }
        public ObservableCollection<PatientVisit> PatientVisits
        {
            get; set;
        }

        private PatientVisit _selectedVisit;
        public PatientVisit SelectedVisit
        {
            get => _selectedVisit;
            set
            {
                _selectedVisit = value;
                OnPropertyChanged(nameof(SelectedVisit));
            }
        }

        public PatientVisitViewModel()
        {
            CheckBoxCommand = new RelayCommand(OnCheckBoxChecked);
            PatientVisits = new ObservableCollection<PatientVisit>();
        }

        private void OnCheckBoxChecked(object parameter)
        {
            var patientVisit = parameter as PatientVisit;
            if (patientVisit != null)
            {
                // Custom behavior logic
            }
        }

        public void LoadPatientVisits(IntPtr chromeHandle)
        {
            PatientVisits.Clear();
            foreach (var visit in PatientVisitExtractor.GetPatientVisits(chromeHandle, OnCheckboxStatusChanged))
            {
                PatientVisits.Add(visit);
            }
        }

        private void OnCheckboxStatusChanged(PatientVisit visit, bool isChecked)
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
