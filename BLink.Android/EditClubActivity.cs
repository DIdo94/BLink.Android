using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
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
        private EditText _clubName;
        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
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
            _clubName = FindViewById<EditText>(Resource.Id.et_editClub_clubName);
            _clubName.Text = _club.Name;

            Button editClub = FindViewById<Button>(Resource.Id.btn_editClub_editClub);
            editClub.Click += EditClub_Click;
            // Create your application here
        }

        private void PickImage_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionSend);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                _clubImage.SetImageURI(uri);
                var imageStream = ContentResolver.OpenInputStream(uri);
            }
        }

        private async void EditClub_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(_clubName.Text))
            {
                _clubName.Error = "Въведете име";
            }
            else
            {
                HttpResponseMessage response = await RestManager.EditClub(new EditClub
                {
                    ClubId = _club.Id,
                    Email = Intent.GetStringExtra("email"),
                    Name = _clubName.Text,
                    ClubPhoto = Assets.Open("club-main-photo.jpg") // TODO This should be user-picked 
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