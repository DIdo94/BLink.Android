using System;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Location.Places;
using Android.Gms.Location.Places.UI;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BLink.Business.Models;
using Newtonsoft.Json;
using static Android.Gms.Common.Apis.GoogleApiClient;

namespace BLink.Droid
{
    [Activity(Label = "GoogleMapActivity")]
    public class GoogleMapActivity : AppCompatActivity, IOnMapReadyCallback, IPlaceSelectionListener, IOnConnectionFailedListener
    {
        private MapView _mapView;
        private GoogleMap _googleMap;
        private MapFragment _mapFragment;
        private FusedLocationProviderClient _fusedLocationProviderClient;
        private Coordinates _markerPosition;
        private Button _setPlaceButton;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CustomMap);

            _setPlaceButton = FindViewById<Button>(Resource.Id.btn_cm_setCoordinates);
            PlaceAutocompleteFragment autocompleteFragment =
                (PlaceAutocompleteFragment)FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);

            var content = Intent.GetStringExtra("place");
            if (!string.IsNullOrWhiteSpace(content))
            {
                autocompleteFragment.View.Visibility = ViewStates.Gone;
                _setPlaceButton.Visibility = ViewStates.Gone;
                _markerPosition = JsonConvert.DeserializeObject<Coordinates>(content);
                //SetMarker(new LatLng(_markerPosition.Latitude, _markerPosition.Longtitute));
            }
            else
            {
                autocompleteFragment.SetOnPlaceSelectedListener(this);
                autocompleteFragment.SetBoundsBias(new LatLngBounds(
                    new LatLng(4.5931, -74.1552),
                    new LatLng(4.6559, -74.0837)));
                _setPlaceButton.Click += setPlaceButton_Click;
            }

            _fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                Location location = await _fusedLocationProviderClient.GetLastLocationAsync();
                var myPosition = new LatLng(location.Latitude, location.Longitude);// (20.5, 20.5);
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeMapToolbarEnabled(true)
                    .InvokeCamera(new CameraPosition(myPosition, 10, 0, 0))
                    .InvokeCompassEnabled(true);
                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }

            _mapFragment.GetMapAsync(this);
  

            // Create your application here
        }

        private void setPlaceButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(CreateClubEventActivity));
            var content = JsonConvert.SerializeObject(_markerPosition);
            intent.PutExtra("place", content);
            StartActivity(intent);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

        public void OnError(Statuses status)
        {
            throw new NotImplementedException();
        }

        public void OnPlaceSelected(IPlace place)
        {
            if (_markerPosition == null)
            {
                _markerPosition = new Coordinates();
            }

            _markerPosition.PlaceName = place.NameFormatted.ToString();
            _markerPosition.Latitude = place.LatLng.Latitude;
            _markerPosition.Longtitute = place.LatLng.Longitude;
            SetMarker(place.LatLng);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _googleMap = googleMap;
           
            _mapFragment.View.LayoutParameters.Height = 900;

            MapsInitializer.Initialize(this);
            if (_markerPosition == null)
            {
                _googleMap.MapLongClick += googleMap_MapLongClick;
            }
            else
            {
                var coordinates = new LatLng(_markerPosition.Latitude, _markerPosition.Longtitute);
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(coordinates);
                _googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(coordinates));
                _googleMap.AddMarker(markerOpt1);
            }
        }

        private void googleMap_MapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
        {
            MarkerOptions markerOpt1 = new MarkerOptions();
            _markerPosition = new Coordinates
            {
                Latitude = e.Point.Latitude,
                Longtitute = e.Point.Longitude,
                PlaceName = "Unknown"
            };
            SetMarker(e.Point);
        }


        private void SetMarker(LatLng coordinates)
        {
            if (_googleMap != null)
            {
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(coordinates);
                markerOpt1.SetTitle("Vimy Ridge");
                _googleMap.Clear();

                _googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(coordinates));
                _googleMap.AddMarker(markerOpt1);

                _setPlaceButton.Visibility = ViewStates.Visible;
            }
        }
    }
}