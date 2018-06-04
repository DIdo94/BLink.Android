using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using BLink.Business.Managers;
using System.Net.Http;
using BLink.Business.Models;
using BLink.Business.Enums;
using Newtonsoft.Json;
using Xamarin.Auth;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Text.RegularExpressions;
using AndroidHUD;
using BLink.Business.Common;
using System.IO;

namespace BLink.Droid
{
    [Activity(Label = "Регистрация")]
    public class RegisterActivity : AppCompatActivity
    {
        private static readonly int PickImageId = 1000;
        private Regex _emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        private Toolbar _toolbar;
        private EditText _email;
        private EditText _password;
        private EditText _confirmPassword;
        private EditText _firstName;
        private EditText _lastName;
        private Spinner _rolesSpinner;
        private EditText _height;
        private EditText _weight;
        private ImageView _userImage;
        private Button _pickImage;
        private DatePicker _dateOfBirth;
        private LinearLayout _playerSection;
        private Spinner _positionsSpinner;
        private Stream _imageStream;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            _email = FindViewById<EditText>(Resource.Id.et_registerEmail);
            _password = FindViewById<EditText>(Resource.Id.et_registerPassword);
            _confirmPassword = FindViewById<EditText>(Resource.Id.et_registerConfirmPassword);
            _firstName = FindViewById<EditText>(Resource.Id.et_registerFirstName);
            _lastName = FindViewById<EditText>(Resource.Id.et_registerLastName);
            _rolesSpinner = FindViewById<Spinner>(Resource.Id.spn_registerRoles);
            _height = FindViewById<EditText>(Resource.Id.et_register_height);
            _weight = FindViewById<EditText>(Resource.Id.et_register_weight);
            _toolbar = FindViewById<Toolbar>(Resource.Id.tbr_register_toolbar);
            _userImage = FindViewById<ImageView>(Resource.Id.iv_register_userImage);
            _pickImage = FindViewById<Button>(Resource.Id.btn_register_pickImage);
            _playerSection = FindViewById<LinearLayout>(Resource.Id.ll_register_playerSection);
            _positionsSpinner = FindViewById<Spinner>(Resource.Id.spn_register_preferedPosition);
            _dateOfBirth = FindViewById<DatePicker>(Resource.Id.dp_register_dateOfBirth);

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            Button registerButton = FindViewById<Button>(Resource.Id.btn_Register);

            string[] roles = Enum
                .GetNames(typeof(Role))
                .Select(r => Literals.ResourceManager.GetString(r)).ToArray();
            _rolesSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, roles);

            string[] positions = Enum
                .GetNames(typeof(Position))
                .Select(r => Literals.ResourceManager.GetString(r)).ToArray();
            _positionsSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, positions);

            _rolesSpinner.ItemSelected += RolesSpinner_ItemSelected;
            registerButton.Click += RegisterButton_Click;
            _pickImage.Click += PickImage_Click;
        }

        private void RolesSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if ((e.Position + 1) == (int)Role.Player)
            {
                _playerSection.Visibility = ViewStates.Visible;
            }
            else
            {
                _playerSection.Visibility = ViewStates.Gone;
            }
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

        private async void RegisterButton_Click(object sender, EventArgs e)
        {

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(_email.Text) || !_emailRegex.IsMatch(_email.Text))
            {
                _email.Error = "Въведете валиден имейл";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(_password.Text))
            {
                _password.Error = "Въведете парола";
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(_confirmPassword.Text) || _confirmPassword.Text != _password.Text)
            {
                _confirmPassword.Error = "Паролите трябва да съвпадат";
                hasError = true;
            }

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

            double.TryParse(_weight.Text, out double weightValue);
            double.TryParse(_height.Text, out double heightValue);
            var rolePosition = _rolesSpinner.SelectedItemPosition + 1;
            var role = (Role)rolePosition;

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

            if (_imageStream == null)
            {
                Toast.MakeText(this, Literals.SelectPicture, ToastLength.Long).Show();
                hasError = true;
            }

            if (hasError)
            {
                return;
            }

            int selectedPosition = _positionsSpinner.SelectedItemPosition + 1;
            Position? position = role == Role.Player ?
                (Position)selectedPosition :
                default(Position?);
            var dateOfBirth = role == Role.Player ?
                _dateOfBirth.DateTime :
                default(DateTime?);
            var registerUser = new RegisterUser
            {
                Email = _email.Text,
                Password = _password.Text,
                ConfirmPassword = _confirmPassword.Text,
                FirstName = _firstName.Text,
                LastName = _lastName.Text,
                Role = role,
                PreferedPosition = position,
                Weight = position.HasValue ? weightValue : default(double?),
                Height = position.HasValue ? heightValue : default(double?),
                File = _imageStream,
                DateOfBirth = dateOfBirth
            };

            AndHUD.Shared.Show(this, "Регистрация…");
            HttpResponseMessage httpResponse = await RestManager.RegisterUser(registerUser);
            string response = await httpResponse.Content.ReadAsStringAsync();
            if (httpResponse.IsSuccessStatusCode)
            {
                UserCredentials userCredentials = JsonConvert.DeserializeObject<UserCredentials>(response);
                Account account = new Account
                {
                    Username = registerUser.Email
                };

                account.Properties.Add("token", userCredentials.AccessToken);
                account.Properties.Add("roles", userCredentials.Roles);

                string appName = GetString(Resource.String.app_name);
                AccountStore.Create(this).Save(account, appName);
                RestManager.SetAccessToken(userCredentials.AccessToken);
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