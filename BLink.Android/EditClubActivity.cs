using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BLink.Business.Common;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BLink.Droid
{
    [Activity(Label = "Промени клуб")]
    public class EditClubActivity : AppCompatActivity
    {
        private static readonly int PickImageId = 1000;

        private ClubDetails _club;
        private Button _pickImage;
        private ImageView _clubImage;
        private string _imagePath;
        private EditText _clubName;
        private Toolbar _toolbar;
        private Stream _imageStream;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditClub);

            _club = JsonConvert.DeserializeObject<ClubDetails>(Intent.GetStringExtra("club"));
            _toolbar = FindViewById<Toolbar>(Resource.Id.tbr_editClub_toolbar);

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _pickImage = FindViewById<Button>(Resource.Id.btn_editClub_pickPhoto);
            _pickImage.Click += PickImage_Click;

            _clubImage = FindViewById<ImageView>(Resource.Id.iv_editClub_clubPhoto);
            _imagePath = await RestManager.GetClubPhoto(_club.Id);
            var bitmap = BitmapFactory.DecodeFile(_imagePath);
            _clubImage.SetImageBitmap(bitmap);

            _clubName = FindViewById<EditText>(Resource.Id.et_editClub_clubName);
            _clubName.Text = _club.Name;

            Button editClub = FindViewById<Button>(Resource.Id.btn_editClub_editClub);
            editClub.Click += EditClub_Click;
        }

        private void PickImage_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, Literals.SelectPicture), PickImageId);
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

        private async void EditClub_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            if (string.IsNullOrWhiteSpace(_clubName.Text))
            {
                _clubName.Error = "Въведете име";
                hasError = true;
            }

            if (!hasError)
            {
                HttpResponseMessage response = await RestManager.EditClub(new EditClub
                {
                    ClubId = _club.Id,
                    Email = Intent.GetStringExtra("email"),
                    Name = _clubName.Text,
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