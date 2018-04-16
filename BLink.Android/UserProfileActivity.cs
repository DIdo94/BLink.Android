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
        private string fullName = "Full Name";
        private TabLayout tabLayout;
        private ViewPager viewPager;
        private MemberDetailsFragment detailsFragment;
        private ClubFragment clubFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.UserProfile);
            Account account = AccountStore
               .Create(this)
               .FindAccountsForService(GetString(Resource.String.app_name))
               .FirstOrDefault();
            if (account == null)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                RestManager.SetAccessToken(account.Properties["token"]);

                viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                SetupViewPager(viewPager);

                tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
                tabLayout.SetupWithViewPager(viewPager);
            }
        }

        private void InitialFragment()
        {
            detailsFragment = new MemberDetailsFragment();
            clubFragment = new ClubFragment();
        }

        public void SetupViewPager(ViewPager viewPager)
        {
            InitialFragment();
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(detailsFragment, "Детайли");
            adapter.AddFragment(clubFragment, "Моят клуб");
            viewPager.Adapter = adapter;
        }
    }
}