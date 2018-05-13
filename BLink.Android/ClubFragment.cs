using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Auth;

namespace BLink.Droid
{
    public class ClubFragment : Android.Support.V4.App.Fragment
    {
        private Account _account;
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private PlayerAdapter _adapter;
        private IEnumerable<MemberDetails> _memberDetails;
        private ClubDetails _clubDetails;

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
            var view = inflater.Inflate(Resource.Layout.Club, null);

            return view;
        }

        public override async void OnActivityCreated(Bundle OnActivityCreated)
        {
            base.OnActivityCreated(OnActivityCreated);

            _account = AccountStore
                   .Create()
                   .FindAccountsForService(GetString(Resource.String.app_name))
                   .FirstOrDefault();

            HttpResponseMessage httpResponse = await RestManager.GetMemberClub(_account.Username);
            string response = await httpResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(response) || response == "null")
            {
                Button createClubButton = View.FindViewById<Button>(Resource.Id.btn_club_createClub);
                TextView clubNameLabel = View.FindViewById<TextView>(Resource.Id.lbl_club_clubName);
                clubNameLabel.Text = "Нямате клуб";

                if (_account.Properties["roles"].Contains(Role.Coach.ToString()))
                {
                    createClubButton.Visibility = ViewStates.Visible;
                    createClubButton.Click += CreateClubButton_Click;
                }
            }
            else
            {
                _clubDetails = JsonConvert.DeserializeObject<ClubDetails>(response);
                TextView clubNameText = View.FindViewById<TextView>(Resource.Id.tv_club_clubName);
                clubNameText.Text = _clubDetails.Name;
                LinearLayout clubDetailsLayout = View.FindViewById<LinearLayout>(Resource.Id.ll_club_clubDetails);
                clubDetailsLayout.Visibility = ViewStates.Visible;

                if (_account.Properties["roles"].Contains(Role.Coach.ToString()))
                {
                    Button searchPlayersButton = View.FindViewById<Button>(Resource.Id.btn_club_searchPlayers);
                    LinearLayout coachClubDetailsLayout = View.FindViewById<LinearLayout>(Resource.Id.ll_club_coachClubDetails);

                    searchPlayersButton.Click += SearchPlayersButton_Click;

                    HttpResponseMessage getPlayersHttpResponse = await RestManager.GetClubPlayers(_clubDetails.Id);
                    string getPlayersResponse = await getPlayersHttpResponse.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(getPlayersResponse) && getPlayersResponse != "null")
                    {
                        _memberDetails = JsonConvert.DeserializeObject<IEnumerable<MemberDetails>>(getPlayersResponse);
                        if (_memberDetails.Any())
                        {
                            _adapter = new PlayerAdapter(Activity, _memberDetails.ToArray(), _clubDetails);
                            _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.rv_club_clubPlayers);
                            _recyclerView.SetAdapter(_adapter);
                            _layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                            _recyclerView.SetLayoutManager(_layoutManager);
                        }
                    }

                    Button goToCreateClubEventButton = View.FindViewById<Button>(Resource.Id.btn_club_goToCreateClubEvent);
                    goToCreateClubEventButton.Click += GoToCreateClubEventButton_Click;
                    coachClubDetailsLayout.Visibility = ViewStates.Visible;
                }
            }
        }

        private void GoToCreateClubEventButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(CreateClubEventActivity));
            intent.PutExtra("clubId", _clubDetails.Id);
            StartActivity(intent);
        }

        private void SearchPlayersButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(SearchPlayersActivity));
            StartActivity(intent);
        }

        private void CreateClubButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(CreateClubActivity));
            intent.PutExtra("email", _account.Username);
            StartActivity(intent);
        }
    }
}