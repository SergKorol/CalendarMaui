namespace CalendarMaui.Models;

public class CalendarModel : PropertyChangedModel
{
    public DateTime Date { get; set; }
    private bool _isCurrentDate;

    public bool IsCurrentDate
    {
            get => _isCurrentDate;
            set => SetField(ref _isCurrentDate, value);
    }
}