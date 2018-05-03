using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BLink.Business.Managers;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Xamarin.Auth;

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
                Intent intent = new Intent(this, typeof(MainActivity));
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
            _detailsFragment = new MemberDetailsFragment();
            _clubFragment = new ClubFragment();
            _invitationsFragment = new InvitationsFragment(_account);
        }

        public void SetupViewPager(ViewPager viewPager)
        {
            InitialFragment();
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(_detailsFragment, "Детайли");
            adapter.AddFragment(_clubFragment, "Моят клуб");
            adapter.AddFragment(_invitationsFragment, "Моите покани");
            viewPager.Adapter = adapter;
        }
    }
}