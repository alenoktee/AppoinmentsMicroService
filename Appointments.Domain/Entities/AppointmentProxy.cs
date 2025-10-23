namespace Appointments.Domain.Entities;

public class AppointmentProxy : Appointment
{
    private ICollection<Result> _results;
    private bool _resultsLoaded = false;

    private readonly Func<ICollection<Result>> _resultsLoader;

    public AppointmentProxy(Func<ICollection<Result>> resultsLoader)
    {
        _resultsLoader = resultsLoader;
    }

    public override ICollection<Result> Results
    {
        get
        {
            if (!_resultsLoaded)
            {
                _results = _resultsLoader();
                _resultsLoaded = true;
            }
            return _results;
        }
        set
        {
            _results = value;
            _resultsLoaded = true;
        }
    }


}
