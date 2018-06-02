using System;
using System.IO;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BLink.Business.Common;
using BLink.Business.Managers;
using BLink.Business.Models;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BLink.Droid
{
    [Activity(Label = "Създай клуб")]
    public class CreateClubActivity : AppCompatActivity
    {
        private static readonly int PickImageId = 1000;
        private Button _pickImage;
        private ImageView _clubImage;
        private Toolbar _toolbar;
        private Stream _imageStream;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateClub);

            _toolbar = FindViewById<Toolbar>(Resource.Id.tbr_register_toolbar);

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _pickImage = FindViewById<Button>(Resource.Id.btn_createClub_pickPhoto);
            _pickImage.Click += PickImage_Click;

            _clubImage = FindViewById<ImageView>(Resource.Id.iv_createClub_clubPhoto);

            Button createClub = FindViewById<Button>(Resource.Id.btn_createClub_createClub);
            createClub.Click += CreateClub_Click;
            // Create your application here
        }

        private void PickImage_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                _clubImage.SetImageURI(uri);
                _imageStream = ContentResolver.OpenInputStream(uri);
            }
        }

        private async void CreateClub_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            EditText clubName = FindViewById<EditText>(Resource.Id.et_createClub_clubName);

            if (string.IsNullOrWhiteSpace(clubName.Text))
            {
                clubName.Error = "Въведете име";
                hasError = true;
            }

            if (_imageStream == null)
            {
                Toast.MakeText(this, Literals.SelectPicture, ToastLength.Long).Show();
                hasError = true;
            }

            if (!hasError)
            {
                string email = Intent.GetStringExtra("email");
                HttpResponseMessage response = await RestManager.CreateClub(new CreateClub
                {
                    Email = email,
                    Name = clubName.Text,
                    ClubPhoto = _imageStream
                });

                if (response.IsSuccessStatusCode)
                {
                    Intent intent = new Intent(this, typeof(UserProfileActivity));
                    StartActivity(intent);
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    Toast.MakeText(this, message, ToastLength.Long).Show();
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Back button pressed -> toggle event
            if (item.ItemId == Android.Resource.Id.Home)
                OnBackPressed();

            return base.OnOptionsItemSelected(item);
        }
    }
}