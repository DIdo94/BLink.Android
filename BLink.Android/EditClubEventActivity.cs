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
    [Activity(Label = "Редактиране на събитие")]
    public class EditClubEventActivity : AppCompatActivity
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
        private EditText _title;
        private EditText _description;
        private ClubEventFilterResult _clubEvent;
        private int selectLocationId = 1;
        private TextView _placeInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            SetContentView(Resource.Layout.EditClubEvent);
            var content = Intent.GetStringExtra("clubEvent");
            if (!string.IsNullOrWhiteSpace(content))
            {
                _clubEvent = JsonConvert.DeserializeObject<ClubEventFilterResult>(content);
            }
            // ... OnCreate method
            _toolbar = FindViewById<Toolbar>(Resource.Id.tv_editClubEvent_toolbar);
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _placeInfo = FindViewById<TextView>(Resource.Id.tv_editClubEvent_eventInfo);
            if (_clubEvent.Coordinates != null)
            {
                _markerPosition = _clubEvent.Coordinates;
                _placeInfo.Text =
                    $"Ширина:{_markerPosition.Latitude}, Дължина:{_markerPosition.Longtitute}";
            }

            _eventTypesSpinner = FindViewById<Spinner>(Resource.Id.spn_editClubEvent_eventType);
            string[] eventTypes = Enum.GetNames(typeof(EventType))
                .Select(r => Literals.ResourceManager.GetString(r)).ToArray();
            _eventTypesSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, eventTypes);
            _eventTypesSpinner.SetSelection((int)_clubEvent.EventType);
            _title = FindViewById<EditText>(Resource.Id.et_editClubEvent_clubEventTitle);
            _title.Text = _clubEvent.Title;

            _description = FindViewById<EditText>(Resource.Id.et_editClubEvent_clubEventDescription);
            _description.Text = _clubEvent.Description;

            _markerPosition = _clubEvent.Coordinates;

            _startTimeDay = FindViewById<DatePicker>(Resource.Id.dp_editClubEvent_clubEventStartTimeDay);
            _startTimeDay.DateTime = _clubEvent.StartTime;
            _startTimeDay.MinDate = (long)DateTime.Now.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
            _startTimeDay.MaxDate = (long)new DateTime(2019, 1, 1).ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
            _startTimeDay.DateChanged += StartTimeDay_DateChanged;

            _startTimeHour = FindViewById<TimePicker>(Resource.Id.tp_editClubEvent_clubEventStartTimeHour);
            _startTimeHour.Hour = _clubEvent.StartTime.Hour;
            _startTimeHour.Minute = _clubEvent.StartTime.Minute;
            _startTimeHour.SetIs24HourView(new Java.Lang.Boolean(true));
            _startTimeHour.TimeChanged += StartTimeHour_TimeChanged;

            var editButton = FindViewById<Button>(Resource.Id.btn_editClubEvent_editClubEvent);
            editButton.Click += EditButton_Click;

            var setPlaceButton = FindViewById<Button>(Resource.Id.btn_editClubEvent_setEventPlace);
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
            StartActivityForResult(intent, selectLocationId);
        }

        private async void EditButton_Click(object sender, EventArgs e)
        {

            bool isValid = true;
            if (string.IsNullOrWhiteSpace(_title.Text))
            {
                _title.Error = Literals.PleaseEnterTitle;
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(_description.Text))
            {
                _description.Error = Literals.PleaseEnterDescription;
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

            var startTime = new DateTime(
               _startTimeDay.Year,
               _startTimeDay.Month + 1, // January-0 December-11
               _startTimeDay.DayOfMonth,
               _startTimeHour.Hour,
               _startTimeHour.Minute,
               0);

            ClubEventCreateRequest clubEventCreateRequest = new ClubEventCreateRequest
            {
                EventId = _clubEvent.Id,
                Title = _title.Text,
                Description = _description.Text,
                StartTime = startTime,
                EventType = (EventType)_eventTypesSpinner.SelectedItemPosition,
                Coordinates = _markerPosition
            };

            AndHUD.Shared.Show(this, "Промяна на събитие…");
            var response = await RestManager.EditClubEvent(clubEventCreateRequest);
            AndHUD.Shared.Dismiss(this);
            if (response.IsSuccessStatusCode)
            {
                Toast.MakeText(this, "Успешна промяна събитие!", ToastLength.Short).Show();
                //var intent = new Intent(this, typeof(ClubEventsFragment));
                //StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "Неуспешна промяна събитие!", ToastLength.Short).Show();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Back button pressed -> toggle event
            if (item.ItemId == Android.Resource.Id.Home)
                OnBackPressed();

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == selectLocationId && resultCode == Result.Ok)
            {
                _markerPosition = JsonConvert.DeserializeObject<Coordinates>(data.GetStringExtra("place"));
                _placeInfo.Text =
                    $"Ширина:{_markerPosition.Latitude}, Дължина:{_markerPosition.Longtitute}";
            }
        }
        //protected override void AttachBaseContext(Context @base)
        //{
        //    Locale locale = new Locale("bg");
        //    Locale.SetDefault(Locale.Category.Format, locale);
        //    @base.Resources.Configuration.SetLocale(locale);
        //    var newContext = @base.CreateConfigurationContext(@base.Resources.Configuration);
        //    base.AttachBaseContext(newContext);
        //}

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