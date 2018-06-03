using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;
using BLink.Business.Managers;
using Newtonsoft.Json;
using BLink.Business.Models;
using Android.Graphics;
using Android.App;
using BLink.Business.Enums;
using BLink.Business.Common;

namespace BLink.Droid
{
    public class MemberDetailsFragment : Android.Support.V4.App.Fragment
    {
        private MemberDetails _memberDetails;
        private Account _account;
        private ImageView _mainPhoto;

        public MemberDetailsFragment(Account account, MemberDetails memberDetails)
        {
            _account = account;
            _memberDetails = memberDetails;
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
            var view = inflater.Inflate(Resource.Layout.MemberDetails, null);

            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var imagePath = await RestManager.GetMemberPhoto(_account.Username);

            TextView firstName = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_firstName);
            TextView lastName = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_lastName);
            TextView height = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_height);
            TextView weight = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_weight);
            TextView dateOfBirth = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_dateOfBirth);
            TextView preferedPosition = View.FindViewById<TextView>(Resource.Id.tv_memberDetails_preferedPosition);
            LinearLayout playerSection = View.FindViewById<LinearLayout>(Resource.Id.ll_memberDetails_playerSection);

            _mainPhoto = View.FindViewById<ImageView>(Resource.Id.iv_memberDetails_mainPhoto);
            var bitmap = BitmapFactory.DecodeFile(imagePath);
            _mainPhoto.SetImageBitmap(bitmap);

            firstName.Text = _memberDetails.FirstName;
            lastName.Text = _memberDetails.LastName;
            if (string.Compare(_account.Properties["roles"], Role.Player.ToString(), true) == 0)
            {
                height.Text = _memberDetails.Height.HasValue ?
                    _memberDetails.Height.Value.ToString() :
                    "0";
                weight.Text = _memberDetails.Weight.HasValue ?
                    _memberDetails.Weight.Value.ToString() :
                    "0";
                dateOfBirth.Text = _memberDetails.DateOfBirth.HasValue ?
                    _memberDetails.DateOfBirth.Value.ToString() :
                    string.Empty;
                preferedPosition.Text = Literals.ResourceManager.GetString(_memberDetails.PreferedPosition.Value.ToString());
                playerSection.Visibility = ViewStates.Visible;
            }

            Button logout = View.FindViewById<Button>(Resource.Id.btn_memberDetails_logout);
            logout.Click += Logout_Click;
            logout.Visibility = ViewStates.Visible;

            Button editMemberDetails = View.FindViewById<Button>(Resource.Id.btn_memberDetails_edit);
            editMemberDetails.Click += EditMemberDetails_Click;
        }

        private void EditMemberDetails_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(EditMemberDetailsActivity));
            intent.PutExtra("account", JsonConvert.SerializeObject(_account));
            intent.PutExtra("member", JsonConvert.SerializeObject(_memberDetails));

            StartActivity(intent);
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Излизане от профила");
            alert.SetMessage("Сигурни ли сте, че искате да излезете от профила?");
            alert.SetPositiveButton("Да", (senderAlert, args) =>
            {
                AccountStore.Create().Delete(_account, GetString(Resource.String.app_name));
                Intent intent = new Intent(Context, typeof(LoginActivity));
                StartActivity(intent);
            });

            alert.SetNegativeButton("Не", (senderAlert, args) =>
            {
                return;
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
    }
}