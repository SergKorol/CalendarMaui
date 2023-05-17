using System.Windows.Input;
using CalendarMaui.Models;
using CalendarMaui.Observable;

namespace CalendarMaui.CustomControls;

public partial class CalendarView
{
    #region BindableProperty
    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
        nameof(SelectedDate), 
        typeof(DateTime), 
        typeof(CalendarView), 
        DateTime.Now, 
        BindingMode.TwoWay,
        propertyChanged: SelectedDatePropertyChanged);

    private static void SelectedDatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        var controls = (CalendarView)bindable;
        if (newvalue != null)
        {
            var newDate = (DateTime)newvalue;
            if (controls._bufferDate.Month == newDate.Month && controls._bufferDate.Year == newDate.Year)
            {
                var currentDate = controls.Weeks.FirstOrDefault(f => f.Value.FirstOrDefault(x => x.Date == newDate.Date) != null).Value.FirstOrDefault(f => f.Date == newDate.Date);
                if (currentDate != null)
                {
                   controls.Weeks.ToList().ForEach(x => x.Value.ToList().ForEach(y => y.IsCurrentDate = false));
                   currentDate.IsCurrentDate = true;
                }
            }
            else
            {
                controls.BindDates(newDate);
            }
        }
    }
    
    //
    public static readonly BindableProperty WeeksProperty =
        BindableProperty.Create(nameof(Weeks), typeof(ObservableDictionary<int, List<CalendarModel>>), typeof(CalendarView));
    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }
    public ObservableDictionary<int, List<CalendarModel>> Weeks
    {
        get => (ObservableDictionary<int, List<CalendarModel>>)GetValue(WeeksProperty);
        set => SetValue(WeeksProperty, value);
    }
    
    //
    public static readonly BindableProperty SelectedDateCommandProperty = BindableProperty.Create(
        nameof(SelectedDateCommand), 
        typeof(ICommand), 
        typeof(CalendarView));
    
    public ICommand SelectedDateCommand
    {
        get => (ICommand)GetValue(SelectedDateCommandProperty);
        set => SetValue(SelectedDateCommandProperty, value);
    }
    
    #endregion
    
    //
    private DateTime _bufferDate;
    public CalendarView()
    {
        InitializeComponent();
        BindDates(DateTime.Now);
        BindingContext = this;
    }

    public void BindDates(DateTime date)
    {
        SetWeeks(date);
        var choseDate = Weeks.SelectMany(x => x.Value).FirstOrDefault(f => f.Date.Date == date.Date);
        if (choseDate != null)
        {
            choseDate.IsCurrentDate = true;
            _bufferDate = choseDate.Date;
            SelectedDate = choseDate.Date;
        }
    }

    private void SetWeeks(DateTime date)
    {
        DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        int weekNumber = 1;
        if (Weeks is null)
        {
            Weeks = new ObservableDictionary<int, List<CalendarModel>>();
        }
        else
        {
            Weeks.Clear();
        }
        // Add days from previous month to first week
        for (int i = 0; i < (int)firstDayOfMonth.DayOfWeek; i++)
        {
            DateTime firstDate = firstDayOfMonth.AddDays(-((int)firstDayOfMonth.DayOfWeek - i));
            if (!Weeks.ContainsKey(weekNumber))
            {
                Weeks.Add(weekNumber, new List<CalendarModel>());
            }
            Weeks[weekNumber].Add(new CalendarModel { Date = firstDate });
        }
        
        // Add days from current month
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime dateInMonth = new DateTime(date.Year, date.Month, day);
            if (dateInMonth.DayOfWeek == DayOfWeek.Sunday && day != 1)
            {
                weekNumber++;
            }
            if (!Weeks.ContainsKey(weekNumber))
            {
                Weeks.Add(weekNumber, new List<CalendarModel>());
            }
            Weeks[weekNumber].Add(new CalendarModel { Date = dateInMonth });
        }
        
        // Add days from next month to last week
        DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, daysInMonth);
        for (int i = 1; i <= 6 - (int)lastDayOfMonth.DayOfWeek; i++)
        {
            DateTime lastDate = lastDayOfMonth.AddDays(i);
            if (!Weeks.ContainsKey(weekNumber))
            {
                Weeks.Add(weekNumber, new List<CalendarModel>());
            }
            Weeks[weekNumber].Add(new CalendarModel { Date = lastDate });
        }
    }

    #region Commands
    public ICommand CurrentDateCommand => new Command<CalendarModel>((currentDate) =>
    {
        _bufferDate = currentDate.Date;
        SelectedDate = currentDate.Date;
        SelectedDateCommand?.Execute(currentDate.Date);
    });

    public ICommand NextMonthCommand => new Command(() =>
    {
        _bufferDate = _bufferDate.AddMonths(1);
        BindDates(_bufferDate);
    });

    public ICommand PreviousMonthCommand => new Command(() =>
    {
        _bufferDate = _bufferDate.AddMonths(-1);
        BindDates(_bufferDate);
    });
    #endregion
}