using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using BLink.Business.Common;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using Xamarin.Auth;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BLink.Droid
{
    [Activity(Label = "@string/editDetails")]
    public class EditMemberDetailsActivity : AppCompatActivity
    {
        private static readonly int PickImageId = 1000;

        private Account _account;
        private MemberDetails _memberDetails;
        private Toolbar _toolbar;
        private EditText _firstName;
        private EditText _lastName;
        private EditText _height;
        private EditText _weight;
        private ImageView _userImage;
        private Button _pickImage;
        private LinearLayout _playerSection;
        private Spinner _positionsSpinner;
        private Stream _imageStream;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EditMemberDetails);

            _account = JsonConvert.DeserializeObject<Account>(Intent.GetStringExtra("account"));
            _memberDetails = JsonConvert.DeserializeObject<MemberDetails>(Intent.GetStringExtra("member"));

            _firstName = FindViewById<EditText>(Resource.Id.et_editMemberDetails_firstName);
            _lastName = FindViewById<EditText>(Resource.Id.et_editMemberDetails_lastName);
            _height = FindViewById<EditText>(Resource.Id.et_editMemberDetails_height);
            _weight = FindViewById<EditText>(Resource.Id.et_editMemberDetails_weight);
            _toolbar = FindViewById<Toolbar>(Resource.Id.tbr_editMemberDetails_toolbar);

            _userImage = FindViewById<ImageView>(Resource.Id.iv_editMemberDetails_userImage);
            var imagePath = await RestManager.GetMemberPhoto(_account.Username);
            var bitmap = BitmapFactory.DecodeFile(imagePath);
            _userImage.SetImageBitmap(bitmap);

            _pickImage = FindViewById<Button>(Resource.Id.btn_editMemberDetails_pickImage);
            _playerSection = FindViewById<LinearLayout>(Resource.Id.ll_editMemberDetails_playerSection);
            _positionsSpinner = FindViewById<Spinner>(Resource.Id.spn_editMemberDetails_preferedPosition);

            _firstName.Text = _memberDetails.FirstName;
            _lastName.Text = _memberDetails.LastName;

            if (string.Compare(_account.Properties["roles"], Role.Player.ToString(), true) == 0)
            {
                _height.Text = _memberDetails.Height.HasValue ? _memberDetails.Height.Value.ToString() : string.Empty;
                _weight.Text = _memberDetails.Weight.HasValue ? _memberDetails.Weight.Value.ToString() : string.Empty;
                string[] positions = Enum
                   .GetNames(typeof(Position))
                   .Select(r => Literals.ResourceManager.GetString(r)).ToArray();
                _positionsSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, positions);
                _positionsSpinner.SetSelection((int)_memberDetails.PreferedPosition.Value - 1);
                _playerSection.Visibility = ViewStates.Visible;
            }

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            Button editDetails = FindViewById<Button>(Resource.Id.btn_editMemberDetails_editDetails);

            editDetails.Click += EditDetails_Click;
            _pickImage.Click += PickImage_Click;
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
                _userImage.SetImageURI(uri);
                _imageStream = ContentResolver.OpenInputStream(uri);
            }
        }

        private async void EditDetails_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            if (string.IsNullOrWhiteSpace(_firstName.Text))
            {
                _firstName.Error = "Въведете Вашето име";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(_lastName.Text))
            {
                _lastName.Error = "Въведете Вашата фамилия";
                hasError = true;
            }

            Role role = (Role)Enum.Parse(typeof(Role), _account.Properties["roles"]);
            int selectedPosition = _positionsSpinner.SelectedItemPosition + 1;
            Position? position =
                string.Compare(role.ToString(), Role.Player.ToString(), true) == 0 ?
                (Position)selectedPosition :
               default(Position?);
            double.TryParse(_weight.Text, out double weightValue);
            double.TryParse(_height.Text, out double heightValue);

            if (weightValue == 0 && role == Role.Player)
            {
                _weight.Error = "Това поле е задължително за играчи";
                hasError = true;
            }

            if (heightValue == 0 && role == Role.Player)
            {
                _height.Error = "Това поле е задължително за играчи";
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            var editMemberDetails = new EditMemberDetails
            {
                Email = _account.Username,
                FirstName = _firstName.Text,
                LastName = _lastName.Text,
                PreferedPosition = position,
                Weight = position.HasValue ? weightValue : default(double?),
                Height = position.HasValue ? heightValue : default(double?),
                File = _imageStream
            };

            AndHUD.Shared.Show(this, "Промяна…");
            HttpResponseMessage httpResponse = await RestManager.EditMemberDetails(editMemberDetails);
            string response = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                Intent intent = new Intent(this, typeof(UserProfileActivity));
                AndHUD.Shared.Dismiss(this);
                StartActivity(intent);
            }
            else
            {
                AndHUD.Shared.Dismiss(this);
                Toast.MakeText(this, response, ToastLength.Long).Show();
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