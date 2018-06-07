using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using BLink.Business.Common;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.RangeSlider;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BLink.Droid
{
    [Activity(Label = "Търси играчи")]
    public class SearchPlayersActivity : AppCompatActivity
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private Toolbar _toolbar;
        private PlayerAdapter _adapter;
        private IEnumerable<MemberDetails> _memberDetails;
        private ClubDetails _clubDetails;
        private Account _account;
        private EditText _name;
        private TextView _toggleFilters;
        private SearchPlayersCritera _searchPlayersCritera;
        private TableLayout _filters;
        private Spinner _positionsSpinner;
        private RangeSliderControl _heightRange;
        private RangeSliderControl _weightRange;
        private RangeSliderControl _ageRange;
        private Button _search;
        private Button _resetFilters;
        private TextView _noPlayersFound;

        public SearchPlayersActivity()
        {
            _searchPlayersCritera = new SearchPlayersCritera
            {
                MaxHeight = int.MaxValue,
                MaxWeight = int.MaxValue,
                MaxAge = int.MaxValue,
                Position = 0
            };
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SearchPlayers);

            Typeface tf = Typeface.CreateFromAsset(Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            _toolbar = FindViewById<Toolbar>(Resource.Id.tbr_searchPlayers_toolbar);
            _name = FindViewById<EditText>(Resource.Id.et_searchPlayers_name);
            _positionsSpinner = FindViewById<Spinner>(Resource.Id.spn_searchPlayers_preferedPosition);

            _heightRange = FindViewById<RangeSliderControl>(Resource.Id.sld_searchPlayers_heightRange);
            _heightRange.SetSelectedMinValue(0);
            _heightRange.SetSelectedMaxValue(300);

            _weightRange = FindViewById<RangeSliderControl>(Resource.Id.sld_searchPlayers_weightRange);
            _weightRange.SetSelectedMinValue(0);
            _weightRange.SetSelectedMaxValue(300);

            _ageRange = FindViewById<RangeSliderControl>(Resource.Id.sld_searchPlayers_ageRange);
            _ageRange.SetSelectedMinValue(_ageRange.AbsoluteMinValue);
            _ageRange.SetSelectedMaxValue(_ageRange.AbsoluteMaxValue);

            _search = FindViewById<Button>(Resource.Id.btn_searchPlayers_searchPlayers);
            _search.Click += Search_Click;

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.rv_searchPlayers_players);
            _layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            _recyclerView.SetLayoutManager(_layoutManager);

            _noPlayersFound = FindViewById<TextView>(Resource.Id.tv_searchPlayers_noPlayersFound);

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
           
            _toggleFilters = FindViewById<TextView>(Resource.Id.tv_searchPlayers_toggleFilters);
           
            _toggleFilters.Click += ToggleFilters_Click;
            _toggleFilters.Typeface = tf;

            _filters = FindViewById<TableLayout>(Resource.Id.tl_searchPlayers_filters);

            var positions = new List<string> { Literals.AllPositions };
            positions.AddRange(Enum
                 .GetNames(typeof(Position))
                 .Select(r => Literals.ResourceManager.GetString(r)));
            _positionsSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, positions.ToArray());

            _resetFilters = FindViewById<Button>(Resource.Id.btn_searchPlayers_resetFilters);
            _resetFilters.Click += ResetFilters_Click;

            _account = AccountStore
              .Create()
              .FindAccountsForService(GetString(Resource.String.app_name))
              .FirstOrDefault();

            HttpResponseMessage getPlayersHttpResponse = await RestManager.GetAvailablePlayers(_searchPlayersCritera);
            HttpResponseMessage getCoachClubHttpResponse = await RestManager.GetMemberClub(_account.Username);

            string getCoachClubResponse = await getCoachClubHttpResponse.Content.ReadAsStringAsync();
            string getPlayersResponse = await getPlayersHttpResponse.Content.ReadAsStringAsync();

            if (getPlayersHttpResponse.IsSuccessStatusCode &&
                !string.IsNullOrWhiteSpace(getPlayersResponse) &&
                getPlayersResponse != "null")
            {
                _clubDetails = JsonConvert.DeserializeObject<ClubDetails>(getCoachClubResponse);
                _memberDetails = JsonConvert.DeserializeObject<IEnumerable<MemberDetails>>(getPlayersResponse);
                if (_memberDetails.Any())
                {
                    _adapter = new PlayerAdapter(this, _memberDetails.ToArray(), _clubDetails, _account);
                    _recyclerView.SetAdapter(_adapter);
                }
            }
        }

        private async void ResetFilters_Click(object sender, EventArgs e)
        {
            _name.Text = string.Empty;

            _positionsSpinner.SetSelection(0);

            _heightRange.SetSelectedMinValue(0);
            _heightRange.SetSelectedMaxValue(300);

            _weightRange.SetSelectedMinValue(0);
            _weightRange.SetSelectedMaxValue(300);

            _ageRange.SetSelectedMinValue(_ageRange.AbsoluteMinValue);
            _ageRange.SetSelectedMaxValue(_ageRange.AbsoluteMaxValue);

            _searchPlayersCritera.MaxHeight = int.MaxValue;
            _searchPlayersCritera.MinHeight = 0;
            _searchPlayersCritera.MaxWeight = int.MaxValue;
            _searchPlayersCritera.MinWeight = 0;
            _searchPlayersCritera.MinAge = 0;
            _searchPlayersCritera.MaxAge = int.MaxValue;
            _searchPlayersCritera.Position = 0;
            _searchPlayersCritera.Name = string.Empty;

            await FilterPlayers();
        }

        private async Task FilterPlayers()
        {
            AndHUD.Shared.Show(this, "Търсене…");
            HttpResponseMessage getPlayersHttpResponse = await RestManager.GetAvailablePlayers(_searchPlayersCritera);
            string getPlayersResponse = await getPlayersHttpResponse.Content.ReadAsStringAsync();

            if (getPlayersHttpResponse.IsSuccessStatusCode &&
               !string.IsNullOrWhiteSpace(getPlayersResponse) &&
               getPlayersResponse != "null")
            {
                _memberDetails = JsonConvert.DeserializeObject<IEnumerable<MemberDetails>>(getPlayersResponse);
                _adapter = new PlayerAdapter(this, _memberDetails.ToArray(), _clubDetails, _account);
                _recyclerView.SetAdapter(_adapter);
                AndHUD.Shared.Dismiss(this);
                if (_memberDetails.Any())
                {
                    _noPlayersFound.Visibility = ViewStates.Gone;
                }
                else
                {
                    _noPlayersFound.Visibility = ViewStates.Visible;
                }
            }
        }

        private async void Search_Click(object sender, EventArgs e)
        {
            _searchPlayersCritera.Name = _name.Text;
            _searchPlayersCritera.MinHeight = _heightRange.GetSelectedMinValue();
            _searchPlayersCritera.MaxHeight = _heightRange.GetSelectedMaxValue();

            _searchPlayersCritera.MinWeight = _weightRange.GetSelectedMinValue();
            _searchPlayersCritera.MaxWeight = _weightRange.GetSelectedMaxValue();

            _searchPlayersCritera.MinAge = _ageRange.GetSelectedMinValue();
            _searchPlayersCritera.MaxAge = _ageRange.GetSelectedMaxValue();

            int postitionIndex = _positionsSpinner.SelectedItemPosition;
            _searchPlayersCritera.Position = (Position)postitionIndex;

            await FilterPlayers();
        }

        private void ToggleFilters_Click(object sender, EventArgs e)
        {
            if (_filters.Visibility == ViewStates.Gone)
            {
                _filters.Visibility = ViewStates.Visible;
                _toggleFilters.Text = GetString(Resource.String.iconChevronUp);
            }
            else
            {
                _filters.Visibility = ViewStates.Gone;
                _toggleFilters.Text = GetString(Resource.String.iconChevronDown);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                OnBackPressed();

            return base.OnOptionsItemSelected(item);
        }
    }
}