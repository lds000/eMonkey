using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Automation;
using System;

namespace eMonkey
{
    /// <summary>
    /// ViewModel for managing patient visits.
    /// </summary>
    public class PatientVisitViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Command for handling checkbox interactions.
        /// </summary>
        public ICommand CheckBoxCommand
        {
            get;
        }

        ChromeWindowM _chromeWindow;

        /// <summary>
        /// Collection of patient visits.
        /// </summary>
        public ObservableCollection<PatientVisitM> PatientVisits
        {
            get; set;
        }

        private PatientVisitM _selectedVisit;

        /// <summary>
        /// Gets or sets the selected patient visit.
        /// </summary>
        public PatientVisitM SelectedVisit
        {
            get => _selectedVisit;
            set
            {
                _selectedVisit = value;
                OnPropertyChanged(nameof(SelectedVisit));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientVisitViewModel"/> class.
        /// </summary>
        public PatientVisitViewModel(ChromeWindowM chromeWindow)
        {
            CheckBoxCommand = new RelayCommand(OnCheckBoxChecked);
            PatientVisits = new ObservableCollection<PatientVisitM>();
            _chromeWindow = chromeWindow;
        }

        /// <summary>
        /// Handles the checkbox checked event.
        /// </summary>
        /// <param name="parameter">The patient visit associated with the checkbox.</param>
        private void OnCheckBoxChecked(object parameter)
        {
            var patientVisit = parameter as PatientVisitM;
            if (patientVisit != null)
            {
                // Custom behavior logic
            }
        }

        /// <summary>
        /// Loads patient visits using the specified window handle.
        /// </summary>
        /// <param name="chromeHandle">The handle of the window to extract patient visits from.</param>
        public void LoadPatientVisits(IntPtr chromeHandle)
        {
            PatientVisits.Clear();
            foreach (var visit in _chromeWindow.GetPatientVisits(OnCheckboxStatusChanged))
            {
                PatientVisits.Add(visit);
            }
        }

        /// <summary>
        /// Handles the checkbox status change event.
        /// </summary>
        /// <param name="visit">The patient visit associated with the checkbox.</param>
        /// <param name="isChecked">The new checkbox status.</param>
        private void OnCheckboxStatusChanged(PatientVisitM visit, bool isChecked)
        {
            if (visit != null)
            {
                visit.IsChecked = isChecked;
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

