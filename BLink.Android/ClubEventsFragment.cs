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

namespace BLink.Droid
{
    public class ClubEventsFragment : Android.Support.V4.App.Fragment
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private ClubEventAdapter _adapter;
        private IEnumerable<ClubEventFilterResult> _clubEvents;
        private Account _account;
        private ClubDetails _clubDetails;

        public ClubEventsFragment(Account account)
        {
            _account = account;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.ClubEvent, null);

            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            HttpResponseMessage clubHttpResponse = await RestManager.GetMemberClub(_account.Username);
            string clubResponse = await clubHttpResponse.Content.ReadAsStringAsync();
            if (clubResponse != "null")
            {
                _clubDetails = JsonConvert.DeserializeObject<ClubDetails>(clubResponse);

                ClubEventFilterRequest clubEventFilterRequest = new ClubEventFilterRequest
                {
                    MemberId = int.Parse(_account.Properties["memberId"]),
                    ClubId = _clubDetails.Id
                };

                HttpResponseMessage httpResponse = await RestManager.GetClubEvents(clubEventFilterRequest);
                string response = await httpResponse.Content.ReadAsStringAsync();

                _clubEvents = JsonConvert.DeserializeObject<IEnumerable<ClubEventFilterResult>>(response);

                if (_clubEvents.Any())
                {
                    _adapter = new ClubEventAdapter(Activity, _clubEvents.ToArray(), _clubDetails);
                    _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.rv_clubEvent_clubEvents);
                    _recyclerView.SetAdapter(_adapter);
                    _layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                    _recyclerView.SetLayoutManager(_layoutManager);
                }
            }
        }
    }
}