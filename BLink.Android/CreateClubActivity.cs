using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using BLink.Business.Managers;
using BLink.Business.Models;

namespace BLink.Droid
{
    [Activity(Label = "Създай клуб")]
    public class CreateClubActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateClub);

            Button createClub = FindViewById<Button>(Resource.Id.btn_createClub_createClub);
            createClub.Click += CreateClub_Click;
            // Create your application here
        }

        private async void CreateClub_Click(object sender, System.EventArgs e)
        {
            EditText clubName = FindViewById<EditText>(Resource.Id.et_createClub_clubName);
            if (string.IsNullOrWhiteSpace(clubName.Text))
            {
                clubName.Error = "Въведете име";
            }
            else
            {
                string email = Intent.GetStringExtra("email");
                await RestManager.CreateClub(new CreateClub
                {
                    Email = email,
                    Name = clubName.Text
                });
                Intent intent = new Intent(this, typeof(UserProfileActivity));
                StartActivity(intent);
            }
        }
    }
}