using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BLink.Business.Common;
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
        private ImageView _mainPhoto;
        private SearchPlayersCritera _searchPlayersCritera;

        public ClubFragment(Account account)
        {
            _account = account;
            _searchPlayersCritera = new SearchPlayersCritera
            {
                MaxHeight = int.MaxValue,
                MaxWeight = int.MaxValue
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _account = AccountStore
               .Create()
               .FindAccountsForService(GetString(Resource.String.app_name))
               .FirstOrDefault();
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

            HttpResponseMessage httpResponse = await RestManager.GetMemberClub(_account.Username);
            string response = await httpResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(response) || response == "null")
            {
                TextView noClubMessage = View.FindViewById<TextView>(Resource.Id.tv_club_noClubMessage);
                noClubMessage.Text = Literals.NoClubMessage;
                noClubMessage.Visibility = ViewStates.Visible;

                if (_account.Properties["roles"].Contains(Role.Coach.ToString()))
                {
                    Button createClubButton = View.FindViewById<Button>(Resource.Id.btn_club_createClub);
                    createClubButton.Visibility = ViewStates.Visible;
                    createClubButton.Click += CreateClubButton_Click;
                }
            }
            else
            {
                _clubDetails = JsonConvert.DeserializeObject<ClubDetails>(response);
                string imagePath = await RestManager.GetClubPhoto(_clubDetails.Id);
                _mainPhoto = View.FindViewById<ImageView>(Resource.Id.iv_club_mainPhoto);
                var bitmap = BitmapFactory.DecodeFile(imagePath);
                _mainPhoto.SetImageBitmap(bitmap);

                TextView clubNameText = View.FindViewById<TextView>(Resource.Id.tv_club_clubName);
                clubNameText.Text = _clubDetails.Name;
                LinearLayout clubDetailsLayout = View.FindViewById<LinearLayout>(Resource.Id.ll_club_clubDetails);
                clubDetailsLayout.Visibility = ViewStates.Visible;

                Button searchPlayersButton = View.FindViewById<Button>(Resource.Id.btn_club_searchPlayers);
                searchPlayersButton.Click += SearchPlayersButton_Click;

                Button editClub = View.FindViewById<Button>(Resource.Id.btn_club_editClubDetails);
                editClub.Click += EditClub_Click;


                LinearLayout coachClubDetailsLayout = View.FindViewById<LinearLayout>(Resource.Id.ll_club_coachClubDetails);
                coachClubDetailsLayout.Visibility = ViewStates.Visible;

                _searchPlayersCritera.ClubId = _clubDetails.Id;
                HttpResponseMessage getPlayersHttpResponse = await RestManager.GetClubPlayers(_searchPlayersCritera);
                string getPlayersResponse = await getPlayersHttpResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(getPlayersResponse) && getPlayersResponse != "null")
                {
                    _memberDetails = JsonConvert.DeserializeObject<IEnumerable<MemberDetails>>(getPlayersResponse);
                    if (_memberDetails.Any())
                    {
                        _adapter = new PlayerAdapter(Activity, _memberDetails.ToArray(), _clubDetails, _account);
                        _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.rv_club_clubPlayers);
                        _recyclerView.SetAdapter(_adapter);
                        _layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                        _recyclerView.SetLayoutManager(_layoutManager);
                    }
                }
            }
        }

        private void EditClub_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(EditClubActivity));
            intent.PutExtra("club", JsonConvert.SerializeObject(_clubDetails));
            intent.PutExtra("email", _account.Username);
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