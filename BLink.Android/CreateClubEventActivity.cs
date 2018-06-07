using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using BLink.Business.Common;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Linq;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BLink.Droid
{
    [Activity(Label = "Създайте събитие")]
    public class CreateClubEventActivity : AppCompatActivity
    {
        private Coordinates _markerPosition;
        private Toolbar _toolbar;
        private DatePicker _startTimeDay;
        bool headerChangeFlag = true;
        TextView headerTextView;
        string headerDatePatternLocale;
        SimpleDateFormat monthDayFormatLocale;
        private TimePicker _startTimeHour;
        private Spinner _eventTypesSpinner;
        private TextView _placeInfo;
        private int _selectPlaceId = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            SetContentView(Resource.Layout.CreateClubEvent);

            // ... OnCreate method
            _toolbar = FindViewById<Toolbar>(Resource.Id.tv_createClubEvent_toolbar);
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _placeInfo = FindViewById<TextView>(Resource.Id.tv_createClubEvent_eventInfo);

            _eventTypesSpinner = FindViewById<Spinner>(Resource.Id.spn_createClubEvent_eventType);
            string[] eventTypes = Enum.GetNames(typeof(EventType))
                .Select(r => Literals.ResourceManager.GetString(r)).ToArray();
            _eventTypesSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, eventTypes);

            _startTimeDay = FindViewById<DatePicker>(Resource.Id.dp_createClubEvent_clubEventStartTimeDay);
            _startTimeDay.MinDate = (long)DateTime.Now.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
            _startTimeDay.MaxDate = (long)new DateTime(2019, 1, 1).ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
            _startTimeDay.DateChanged += StartTimeDay_DateChanged;

            _startTimeHour = FindViewById<TimePicker>(Resource.Id.tp_createClubEvent_clubEventStartTimeHour);
            _startTimeHour.SetIs24HourView(new Java.Lang.Boolean(true));
            _startTimeHour.TimeChanged += StartTimeHour_TimeChanged;

            var createButton = FindViewById<Button>(Resource.Id.btn_createClubEvent_createClubEvent);
            createButton.Click += CreateButton_Click;

            var setPlaceButton = FindViewById<Button>(Resource.Id.btn_createClubEvent_setEventPlace);
            setPlaceButton.Click += SetPlaceButton_Click;
        }

        private void StartTimeDay_DateChanged(object sender, DatePicker.DateChangedEventArgs e)
        {
            var now = DateTime.Now;
            bool isSameMonth = (e.MonthOfYear + 1) == now.Month;
            bool isSameDay = isSameMonth && (_startTimeDay.DayOfMonth == now.Day);
            if (isSameMonth && isSameDay)
            {
                _startTimeHour.Hour = now.Hour;
                _startTimeHour.Minute = now.Minute;
            }
        }

        private void StartTimeHour_TimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            var now = DateTime.Now;
            bool isBiggerMonth = (_startTimeDay.Month + 1) > now.Month;
            bool isSameMonth = (_startTimeDay.Month + 1) == now.Month;
            bool isBiggerDay = isSameMonth && (_startTimeDay.DayOfMonth > now.Day);
            if (isBiggerMonth || isBiggerDay)
            {
                return;
            }

            if (e.HourOfDay < now.Hour)
            {
                _startTimeHour.Hour = now.Hour;
            }
            else if (e.HourOfDay == now.Hour)
            {
                if ((e.Minute) < now.Minute)
                {
                    _startTimeHour.Minute = now.Minute;
                }
            }
        }

        private void SetPlaceButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GoogleMapActivity));
            if (_markerPosition != null)
            {
                intent.PutExtra("place", JsonConvert.SerializeObject(_markerPosition));
            }

            intent.PutExtra("isEditable", true);

            StartActivityForResult(intent, _selectPlaceId);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == _selectPlaceId && resultCode == Result.Ok)
            {
                var place = data.GetStringExtra("place");
                if (!string.IsNullOrWhiteSpace(place))
                {
                    _markerPosition = JsonConvert.DeserializeObject<Coordinates>(place);
                    _placeInfo.Text =
                        $"Ширина:{_markerPosition.Latitude}, Дължина:{_markerPosition.Longtitute}";
                }
            }
        }

        private async void CreateButton_Click(object sender, System.EventArgs e)
        {
            var title = FindViewById<EditText>(Resource.Id.et_createClubEvent_clubEventTitle);
            var description = FindViewById<EditText>(Resource.Id.et_createClubEvent_clubEventDescription);
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(title.Text))
            {
                title.Error = Literals.PleaseEnterTitle;
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(description.Text))
            {
                description.Error = Literals.PleaseEnterDescription;
                isValid = false;
            }

            if (_markerPosition == null)
            {
                Toast.MakeText(this, Literals.PleaseEnterLocation, ToastLength.Long).Show();
                isValid = false;
            }

            if (!isValid)
            {
                return;
            }

            var clubId = Intent.GetIntExtra("clubId", 0);
            var startTime = new DateTime(
               _startTimeDay.Year,
               _startTimeDay.Month + 1, // January-0 December-11
               _startTimeDay.DayOfMonth,
               _startTimeHour.Hour,
               _startTimeHour.Minute,
               0);

            ClubEventCreateRequest clubEventCreateRequest = new ClubEventCreateRequest
            {
                ClubId = clubId,
                Title = title.Text,
                Description = description.Text,
                StartTime = startTime,
                EventType = (EventType)_eventTypesSpinner.SelectedItemPosition,
                Coordinates = _markerPosition
            };

            AndHUD.Shared.Show(this, "Създаване на събитие…");
            var response = await RestManager.CreateClubEvent(clubEventCreateRequest);
            AndHUD.Shared.Dismiss(this);
            if (response.IsSuccessStatusCode)
            {
                Toast.MakeText(this, "Успешно създадено събитие!", ToastLength.Short).Show();
                //var intent = new Intent(this, typeof(ClubEventsFragment));
                //StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "Неуспешно създадено събитие!", ToastLength.Short).Show();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Back button pressed -> toggle event
            if (item.ItemId == Android.Resource.Id.Home)
            {
                OnBackPressed();
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void AttachBaseContext(Context @base)
        {
            Locale locale = new Locale("bg_BG");
            Locale.SetDefault(Locale.Category.Format, locale);
            @base.Resources.Configuration.SetLocale(locale);
            var newContext = @base.CreateConfigurationContext(@base.Resources.Configuration);
            base.AttachBaseContext(newContext);
        }

        void SetHeaderMonthDay(DatePickerDialog dialog, Locale locale)
        {
            if (headerTextView == null)
            {
                // Material Design formatted CalendarView being used, need to do API level check and skip on older APIs
                var id = base.Resources.GetIdentifier("date_picker_header_date", "id", "android");
                headerTextView = dialog.DatePicker.FindViewById<TextView>(id);
                headerDatePatternLocale = Android.Text.Format.DateFormat.GetBestDateTimePattern(locale, "EMMMd");
                monthDayFormatLocale = new SimpleDateFormat(headerDatePatternLocale, locale);
                headerTextView.SetTextColor(Android.Graphics.Color.Red);
                headerTextView.TextChanged += (sender, e) =>
                {
                    headerChangeFlag = !headerChangeFlag;
                    if (!headerChangeFlag)
                        return;
                    SetHeaderMonthDay(dialog, locale);
                };
            }
            var selectedDateLocale = monthDayFormatLocale.Format(new Date((long)dialog.DatePicker.DateTime.ToUniversalTime().Subtract(
                      new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds));
            headerTextView.Text = selectedDateLocale;
        }
    }
}