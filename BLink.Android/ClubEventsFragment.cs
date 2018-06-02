using System.Collections.Generic;
using Android.OS;
using Android.Views;
using BLink.Business.Models;
using Android.Support.V7.Widget;
using System.Net.Http;
using BLink.Business.Managers;
using Xamarin.Auth;
using System.Linq;
using Newtonsoft.Json;
using Android.Widget;
using Android.Content;
using BLink.Business.Enums;
using Android.Support.Design.Widget;
using System;
using BLink.Business.Common;
using AndroidHUD;

namespace BLink.Droid
{
    public class ClubEventsFragment : Android.Support.V4.App.Fragment
    {
        private RecyclerView _recyclerView;
        private Spinner _timeSpans;
        private RecyclerView.LayoutManager _layoutManager;
        private ClubEventAdapter _adapter;
        private IEnumerable<ClubEventFilterResult> _clubEvents;
        private Account _account;
        private ClubDetails _clubDetails;
        private ClubEventFilterRequest _clubEventFilterRequest;
        private TextView _noClubEvents;
        private LinearLayout _eventsFilter;

        public ClubEventsFragment(Account account)
        {
            _account = account;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.ClubEvent, null);

            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            _noClubEvents = View.FindViewById<TextView>(Resource.Id.tv_clubEvent_noClubEvents);

            if (_account.Properties["roles"].Contains(Role.Coach.ToString()))
            {
                FloatingActionButton goToCreateClubEventButton = View.FindViewById<FloatingActionButton>(Resource.Id.btn_clubEvent_goToCreateClubEvent);
                goToCreateClubEventButton.BringToFront();
                goToCreateClubEventButton.Click += GoToCreateClubEventButton_Click;
                goToCreateClubEventButton.Visibility = ViewStates.Visible;
            }

            HttpResponseMessage clubHttpResponse = await RestManager.GetMemberClub(_account.Username);
            string clubResponse = await clubHttpResponse.Content.ReadAsStringAsync();
            if (clubResponse != "null")
            {
                _clubDetails = JsonConvert.DeserializeObject<ClubDetails>(clubResponse);
                _clubEventFilterRequest = new ClubEventFilterRequest
                {
                    MemberId = int.Parse(_account.Properties["memberId"]),
                    ClubId = _clubDetails.Id
                };

                HttpResponseMessage httpResponse = await RestManager.GetClubEvents(_clubEventFilterRequest);
                string response = await httpResponse.Content.ReadAsStringAsync();

                _clubEvents = JsonConvert.DeserializeObject<IEnumerable<ClubEventFilterResult>>(response);

                if (_clubEvents.Any())
                {
                    _timeSpans = View.FindViewById<Spinner>(Resource.Id.spn_clubEvent_timeSpan);
                    var timeSpans = Enum.GetNames(typeof(EventTimeSpan))
                        .Select(r => Literals.ResourceManager.GetString(r)).ToArray();
                    _timeSpans.Adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, timeSpans);
                    _timeSpans.ItemSelected += TimeSpans_ItemSelected;

                    _eventsFilter = View.FindViewById<LinearLayout>(Resource.Id.ll_clubEvent_clubEventsFilter);
                    _eventsFilter.Visibility = ViewStates.Visible;

                    _adapter = new ClubEventAdapter(Activity, _clubEvents.ToArray(), _clubDetails, _account);
                    _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.rv_clubEvent_clubEvents);
                    _recyclerView.SetAdapter(_adapter);
                    _layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                    _recyclerView.SetLayoutManager(_layoutManager);
                }
                else
                {
                    _noClubEvents.Visibility = ViewStates.Visible;
                }
            }
        }

        private async void TimeSpans_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var timeSpan = (EventTimeSpan)e.Position;
            _clubEventFilterRequest.EventTimeSpan = timeSpan;

            AndHUD.Shared.Show(Context, "Търсене…");

            HttpResponseMessage httpResponse = await RestManager.GetClubEvents(_clubEventFilterRequest);
            string response = await httpResponse.Content.ReadAsStringAsync();
            _clubEvents = JsonConvert.DeserializeObject<IEnumerable<ClubEventFilterResult>>(response);

            AndHUD.Shared.Dismiss(Context);
            _adapter = new ClubEventAdapter(Activity, _clubEvents.ToArray(), _clubDetails, _account);
            _recyclerView.SwapAdapter(_adapter, true);

            if (!_clubEvents.Any())
            {
                _noClubEvents = View.FindViewById<TextView>(Resource.Id.tv_clubEvent_noClubEvents);
                _noClubEvents.Visibility = ViewStates.Visible;
            }
            else
            {
                _noClubEvents.Visibility = ViewStates.Gone;
            }
        }

        private void GoToCreateClubEventButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(CreateClubEventActivity));
            intent.PutExtra("clubId", _clubDetails.Id);
            StartActivity(intent);
        }
    }
}