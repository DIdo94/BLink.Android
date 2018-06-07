using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using BLink.Business.Managers;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Xamarin.Auth;
using BLink.Business.Common;
using System.Net.Http;
using BLink.Business.Models;
using Newtonsoft.Json;
using BLink.Business.Enums;

namespace BLink.Droid
{
    [Activity(MainLauncher = true)]
    public class UserProfileActivity : AppCompatActivity
    {
        private TabLayout _tabLayout;
        private ViewPager _viewPager;
        private MemberDetailsFragment _detailsFragment;
        private ClubFragment _clubFragment;
        private InvitationsFragment _invitationsFragment;
        private ClubEventsFragment _clubEventsFragment;
        private Account _account;
        private MemberDetails _memberDetails;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.UserProfile);
            _account = AccountStore
               .Create(this)
               .FindAccountsForService(GetString(Resource.String.app_name))
               .FirstOrDefault();
            if (_account == null)
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
            }
            else
            {
                RestManager.SetAccessToken(_account.Properties["token"]);
                HttpResponseMessage httpResponse = await RestManager.GetMemberDetails(_account.Username);
                string response = await httpResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(response) && response != "null")
                {
                    _memberDetails = JsonConvert.DeserializeObject<MemberDetails>(response);
                }

                _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                SetupViewPager(_viewPager);

                _tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
                _tabLayout.SetupWithViewPager(_viewPager);
            }
        }

        private void InitializeFragments()
        {
            _detailsFragment = new MemberDetailsFragment();
            _clubFragment = new ClubFragment();
            if (!_memberDetails.ClubId.HasValue || _account.Properties["roles"].Contains(Role.Coach.ToString()))
            {
                _invitationsFragment = new InvitationsFragment();
            }

            _clubEventsFragment = new ClubEventsFragment();
        }

        public void SetupViewPager(ViewPager viewPager)
        {
            InitializeFragments();
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(_detailsFragment, Literals.Details);
            adapter.AddFragment(_clubFragment, Literals.Club);
            if (!_memberDetails.ClubId.HasValue || _account.Properties["roles"].Contains(Role.Coach.ToString()))
            {
                adapter.AddFragment(_invitationsFragment, Literals.Invitations);
            }

            adapter.AddFragment(_clubEventsFragment, Literals.Events);
            viewPager.Adapter = adapter;
        }
    }
}