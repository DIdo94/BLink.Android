using Android.App;
using Android.OS;
using Android.Widget;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using System;

namespace BLink.Droid
{
    [Activity(Label = "CreateClubEventActivity")]
    public class CreateClubEventActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.CreateClubEvent);
            Spinner eventTypesSpinner = FindViewById<Spinner>(Resource.Id.spn_createClubEvent_eventType);

            string[] eventTypes = Enum.GetNames(typeof(EventType));
            eventTypesSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, eventTypes);

            var createButton = FindViewById<Button>(Resource.Id.btn_createClubEvent_createClubEvent);
            createButton.Click += CreateButton_Click;
        }

        private async void CreateButton_Click(object sender, System.EventArgs e)
        {
            var title = FindViewById<EditText>(Resource.Id.et_createClubEvent_clubEventTitle);
            var description = FindViewById<EditText>(Resource.Id.et_createClubEvent_clubEventDescription);
            var startTimeDay = FindViewById<DatePicker>(Resource.Id.dp_createClubEvent_clubEventStartTimeDay);
            var startTimeHour = FindViewById<TimePicker>(Resource.Id.tp_createClubEvent_clubEventStartTimeHour);
            Spinner eventTypesSpinner = FindViewById<Spinner>(Resource.Id.spn_createClubEvent_eventType);
            var clubId = Intent.GetIntExtra("clubId", 0);

            var startTime = new DateTime(
                startTimeDay.Year,
                startTimeDay.Month + 1, // January-0 December-11
                startTimeDay.DayOfMonth,
                startTimeHour.Hour,
                startTimeHour.Minute,
                0);
            ClubEventCreateRequest clubEventCreateRequest = new ClubEventCreateRequest
            {
                ClubId = clubId,
                Title = title.Text,
                Description = description.Text,
                StartTime = startTime,
                EventType = (EventType)Enum.Parse(typeof(EventType), eventTypesSpinner.SelectedItem.ToString())
            };

            var response = await RestManager.CreateClubEvent(clubEventCreateRequest);
            if (response.IsSuccessStatusCode)
            {
                Toast.MakeText(this, "Успешно създадено събитие!", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Неуспешно създадено събитие!", ToastLength.Short).Show();
            }
        }
    }
}