using System.ComponentModel;
using System.Windows.Automation;

public class PatientVisit : INotifyPropertyChanged
{
    private bool _isChecked;

    public string AppointmentTime
    {
        get; set;
    }
    public string Provider
    {
        get; set;
    }
    public string PatientName
    {
        get; set;
    }
    public string Insurance
    {
        get; set;
    }
    public string VisitReason
    {
        get; set;
    }
    public string Gender
    {
        get; set;
    }
    public string Age
    {
        get; set;
    }
    public string VisitStatus
    {
        get; set;
    }
    public string Duration
    {
        get; set;
    }
    public string Room
    {
        get; set;
    }
    public string CycleTime
    {
        get; set;
    }
    public AutomationElement CheckboxElement
    {
        get; set;
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            OnPropertyChanged(nameof(IsChecked));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
