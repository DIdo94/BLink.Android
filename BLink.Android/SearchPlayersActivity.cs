using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Auth;

namespace BLink.Droid
{
    [Activity(Label = "SearchPlayersActivity")]
    public class SearchPlayersActivity : Activity
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private PlayerAdapter _adapter;
        private IEnumerable<MemberDetails> _memberDetails;
        private ClubDetails _clubDetails;
        private Account _account;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SearchPlayers);

            _account = AccountStore
              .Create()
              .FindAccountsForService(GetString(Resource.String.app_name))
              .FirstOrDefault();
            HttpResponseMessage getPlayersHttpResponse = await RestManager.GetAvailablePlayers();
            HttpResponseMessage getCoachClubHttpResponse = await RestManager.GetMemberClub(_account.Username);

            string getCoachClubResponse = await getCoachClubHttpResponse.Content.ReadAsStringAsync();
            string getPlayersResponse = await getPlayersHttpResponse.Content.ReadAsStringAsync();

            if (getPlayersHttpResponse.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(getPlayersResponse) && getPlayersResponse != "null")
            {
                _clubDetails = JsonConvert.DeserializeObject<ClubDetails>(getCoachClubResponse);
                _memberDetails = JsonConvert.DeserializeObject<IEnumerable<MemberDetails>>(getPlayersResponse);
                if (_memberDetails.Any())
                {
                    _adapter = new PlayerAdapter(this, _memberDetails.ToArray(), _clubDetails);
                    _recyclerView = FindViewById<RecyclerView>(Resource.Id.rv_searchPlayers_players);
                    _recyclerView.SetAdapter(_adapter);
                    _layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    _recyclerView.SetLayoutManager(_layoutManager);
                }
            }
        }
    }
}