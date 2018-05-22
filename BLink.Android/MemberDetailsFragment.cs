using System;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;
using BLink.Business.Managers;
using System.Net.Http;
using Newtonsoft.Json;
using BLink.Business.Models;

namespace BLink.Droid
{
    public class MemberDetailsFragment : Android.Support.V4.App.Fragment
    {
        private MemberDetails memberDetails;
        private Account _account;
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
            var view = inflater.Inflate(Resource.Layout.MemberDetails, null);

            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            _account = AccountStore
               .Create()
               .FindAccountsForService(GetString(Resource.String.app_name))
               .FirstOrDefault();
            HttpResponseMessage httpResponse = await RestManager.GetMemberDetails(_account.Username);
            string response = await httpResponse.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(response) && response != "null")
            {
                memberDetails = JsonConvert.DeserializeObject<MemberDetails>(response);
                TextView firstName = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_firstName);
                TextView lastName = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_lastName);
                TextView height = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_height);
                TextView weight = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_weight);


                firstName.Text = memberDetails.FirstName;
                lastName.Text = memberDetails.LastName;
                height.Text = memberDetails.Height.HasValue ?
                    memberDetails.Height.Value.ToString() :
                    "0";
                weight.Text = memberDetails.Weight.HasValue ?
                    memberDetails.Weight.Value.ToString() :
                    "0";

                Button logout = View.FindViewById<Button>(Resource.Id.btn_memberDetails_logout);
                logout.Click += Logout_Click;
                logout.Visibility = ViewStates.Visible;
            }
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            AccountStore.Create().Delete(_account, GetString(Resource.String.app_name));
            Intent intent = new Intent(Context, typeof(LoginActivity));
            StartActivity(intent);
        }
    }
}