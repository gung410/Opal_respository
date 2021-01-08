using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Calendar
{
    public class CalendarSectionViewModel : BaseViewModel
    {
        private readonly ICalendarBackendService _calendarBackendService;

        private int _count;
        private int _totalEventToDay;
        private int _totalEventThisWeek;
        private int _totalEventThisMonth;
        private bool _isShowCalendarSection;
        private string _thisMonth;
        private string _thisDay;

        public CalendarSectionViewModel()
        {
            _calendarBackendService = CreateRestClientFor<ICalendarBackendService>(GlobalSettings.BackendServiceCalendar);
        }

        public ICommand OpenFullCalendarCommand => new Command(async () =>
        {
            await this.NavigationService.NavigateToAsync<CalendarViewModel>();
        });

        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                _count = value;
                RaisePropertyChanged(() => Count);
            }
        }

        public string ThisMonth
        {
            get
            {
                return _thisMonth;
            }

            set
            {
                _thisMonth = value;
                RaisePropertyChanged(() => ThisMonth);
            }
        }

        public string ThisDay
        {
            get
            {
                return _thisDay;
            }

            set
            {
                _thisDay = value;
                RaisePropertyChanged(() => ThisDay);
            }
        }

        public int TotalEventToDay
        {
            get
            {
                return _totalEventToDay;
            }

            set
            {
                _totalEventToDay = value;
                RaisePropertyChanged(() => TotalEventToDay);
            }
        }

        public int TotalEventThisWeek
        {
            get
            {
                return _totalEventThisWeek;
            }

            set
            {
                _totalEventThisWeek = value;
                RaisePropertyChanged(() => TotalEventThisWeek);
            }
        }

        public int TotalEventThisMonth
        {
            get
            {
                return _totalEventThisMonth;
            }

            set
            {
                _totalEventThisMonth = value;
                RaisePropertyChanged(() => TotalEventThisMonth);
            }
        }

        public bool IsShowCalendarSection
        {
            get
            {
                return _isShowCalendarSection;
            }

            set
            {
                _isShowCalendarSection = value;
                RaisePropertyChanged(() => IsShowCalendarSection);
            }
        }

        public async Task LoadData()
        {
            if (!IsShowCalendarSection)
            {
                return;
            }

            ThisMonth = DateTime.Now.ToString("MMM");
            ThisDay = DateTime.Now.ToString("dd");

            await Task.WhenAll(GetEventToday(), GetEventThisWeek(), GetEventthisMonth());
        }

        private async Task GetEventToday()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddHours(24).AddTicks(-1);

            var countResult = await ExecuteBackendService(() => _calendarBackendService.GetCalendarEventCount(start, end));

            TotalEventToDay = countResult.Payload;
        }

        private async Task GetEventThisWeek()
        {
            DateTime monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime sunday = monday.AddDays(7).AddTicks(-1);

            var countResult = await ExecuteBackendService(() => _calendarBackendService.GetCalendarEventCount(monday, sunday));

            TotalEventThisWeek = countResult.Payload;
        }

        private async Task GetEventthisMonth()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddTicks(-1);

            var countResult = await ExecuteBackendService(() => _calendarBackendService.GetCalendarEventCount(startDate, endDate));

            TotalEventThisMonth = countResult.Payload;
        }
    }
}
