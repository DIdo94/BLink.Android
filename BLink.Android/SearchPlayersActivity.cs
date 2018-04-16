using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BLink.Droid
{
    [Activity(Label = "SearchPlayersActivity")]
    public class SearchPlayersActivity : Activity
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private PlayerAdapter _adapter;
        private IEnumerable<MemberDetails> _memberDetails;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SearchPlayers);

            HttpResponseMessage getPlayersHttpResponse = await RestManager.GetAvailablePlayers();
            string getPlayersResponse = await getPlayersHttpResponse.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(getPlayersResponse) && getPlayersResponse != "null")
            {
                _memberDetails = JsonConvert.DeserializeObject<IEnumerable<MemberDetails>>(getPlayersResponse);
                if (_memberDetails.Any())
                {
                    _adapter = new PlayerAdapter(this, _memberDetails.ToArray());
                    _recyclerView = FindViewById<RecyclerView>(Resource.Id.rv_searchPlayers_players);
                    _recyclerView.SetAdapter(_adapter);
                    _layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                    _recyclerView.SetLayoutManager(_layoutManager);
                }
            }
        }
    }
}