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

        protected override void OnCreate(Bundle savedInstanceState)
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

                _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                SetupViewPager(_viewPager);

                _tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
                _tabLayout.SetupWithViewPager(_viewPager);
            }
        }

        private void InitialFragment()
        {
            _detailsFragment = new MemberDetailsFragment(_account);
            _clubFragment = new ClubFragment(_account);
            _invitationsFragment = new InvitationsFragment(_account);
            _clubEventsFragment = new ClubEventsFragment(_account);
        }

        public void SetupViewPager(ViewPager viewPager)
        {
            InitialFragment();
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(_detailsFragment, Literals.Details);
            adapter.AddFragment(_clubFragment, Literals.Club);
            adapter.AddFragment(_invitationsFragment, Literals.Invitations);
            adapter.AddFragment(_clubEventsFragment, Literals.Events);
            viewPager.Adapter = adapter;
        }
    }
}