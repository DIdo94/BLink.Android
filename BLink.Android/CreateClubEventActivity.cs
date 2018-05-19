using Android.App;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using System;

namespace BLink.Droid
{
    [Activity(Label = "CreateClubEventActivity")]
    public class CreateClubEventActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private MapView _mapView;
        private GoogleMap _googleMap;
        private MapFragment _mapFragment;
        FusedLocationProviderClient _fusedLocationProviderClient;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            SetContentView(Resource.Layout.CreateClubEvent);
            Spinner eventTypesSpinner = FindViewById<Spinner>(Resource.Id.spn_createClubEvent_eventType);

            string[] eventTypes = Enum.GetNames(typeof(EventType));
            eventTypesSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, eventTypes);

            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeMapToolbarEnabled(true)
                    .InvokeCompassEnabled(true);
                mapOptions.Camera.Zoom = 10;
                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }

            _mapFragment.GetMapAsync(this);
            _fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            //_mapView = FindViewById<MapView>(Resource.Id.map);
            //_mapView.OnCreate(savedInstanceState);
            //_mapView.GetMapAsync(this);
            var createButton = FindViewById<Button>(Resource.Id.btn_createClubEvent_createClubEvent);
            createButton.Click += CreateButton_Click;
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;
            _googleMap.MapLongClick += googleMap_MapLongClick;
            //Setup and customize your Google Map
            //_googleMap.UiSettings.CompassEnabled = false;
            //_googleMap.UiSettings.MyLocationButtonEnabled = false;
            //_googleMap.UiSettings.MapToolbarEnabled = false;
            _mapFragment.View.LayoutParameters.Height = 900;

            MapsInitializer.Initialize(this);
            Location location = await _fusedLocationProviderClient.GetLastLocationAsync();
            var myPosition = new LatLng(location.Latitude, location.Longitude);
            _googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(myPosition));
            //var position = await CrossGeolocator.Current.GetPositionAsync(10000);
            //var me = new LatLng(position.Latitude, position.Longitude);
            //googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(me));

        }

        private void googleMap_MapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            MarkerOptions markerOpt1 = new MarkerOptions();
            markerOpt1.SetPosition(e.Point);
            markerOpt1.SetTitle("Vimy Ridge");
            _googleMap.Clear();
            _googleMap.AddMarker(markerOpt1);
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