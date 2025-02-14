﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Automation;
using System;

namespace eMonkey
{
    public class PatientVisitViewModel : INotifyPropertyChanged
    {
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

        public ICommand ToggleCheckboxCommand
        {
            get;
        }

        public PatientVisitViewModel()
        {
            PatientVisits = new ObservableCollection<PatientVisit>();
           // ToggleCheckboxCommand = new RelayCommand(ToggleCheckbox, CanToggleCheckbox);
        }

        private bool CanToggleCheckbox(object parameter)
        {
            return parameter is PatientVisit visit && visit.CheckboxElement != null;
        }

        private void ToggleCheckbox(object parameter)
        {
            if (parameter is PatientVisit visit && visit.CheckboxElement != null)
            {
                if (visit.CheckboxElement.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObj))
                {
                    var togglePattern = (TogglePattern)togglePatternObj;
                    togglePattern.Toggle();
                    visit.IsChecked = !visit.IsChecked;  // Update ViewModel state
                }
            }
        }

        public void LoadPatientVisits(IntPtr chromeHandle)
        {
            PatientVisits.Clear();
            foreach (var visit in PatientVisitExtractor.GetPatientVisits(chromeHandle))
            {
                PatientVisits.Add(visit);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
