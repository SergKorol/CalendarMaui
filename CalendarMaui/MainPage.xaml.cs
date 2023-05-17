using CalendarMaui.Models;

namespace CalendarMaui;

public partial class MainPage : ContentPage
{
    public Dictionary<int, List<CalendarModel>> Dict { get; set;} = new Dictionary<int, List<CalendarModel>>();

    
    public MainPage()
    {
        InitializeComponent();
        calendar.SelectedDate = DateTime.Now;
    }
}