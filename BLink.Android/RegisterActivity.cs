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
using System.Net.Http;
using BLink.Business.Models;
using BLink.Business.Enums;
using Newtonsoft.Json;
using Xamarin.Auth;

namespace BLink.Droid
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Register);
            // Create your application here

            Button registerButton = FindViewById<Button>(Resource.Id.btn_Register);
            Spinner rolesSpinner = FindViewById<Spinner>(Resource.Id.spn_registerRoles);

            string[] roles = Enum.GetNames(typeof(Role));
            rolesSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, roles);
            registerButton.Click += RegisterButton_Click;
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            EditText email = FindViewById<EditText>(Resource.Id.et_registerEmail);
            EditText password = FindViewById<EditText>(Resource.Id.et_registerPassword);
            EditText confirmPassword = FindViewById<EditText>(Resource.Id.et_registerConfirmPassword);
            EditText firstName = FindViewById<EditText>(Resource.Id.et_registerFirstName);
            EditText lastName = FindViewById<EditText>(Resource.Id.et_registerLastName);
            Spinner rolesSpinner = FindViewById<Spinner>(Resource.Id.spn_registerRoles);
            EditText height = FindViewById<EditText>(Resource.Id.et_register_height);
            EditText weight = FindViewById<EditText>(Resource.Id.et_register_weight);
            double.TryParse(weight.Text, out double weightValue);
            double.TryParse(height.Text, out double heightValue);

            var registerUser = new RegisterUser
            {
                Email = email.Text,
                Password = password.Text,
                ConfirmPassword = confirmPassword.Text,
                FirstName = firstName.Text,
                LastName = lastName.Text,
                Role = (Role)Enum.Parse(typeof(Role),rolesSpinner.SelectedItem.ToString()),
                Weight = weightValue,
                Height = heightValue
            };
            HttpResponseMessage httpResponse = await RestManager.RegisterUser(registerUser);
            if (httpResponse.IsSuccessStatusCode)
            {
                string response = await httpResponse.Content.ReadAsStringAsync();
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
                StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "Неуспешна регистрация!", ToastLength.Short).Show();
            }
        }
    }
}