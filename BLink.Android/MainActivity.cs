using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Text.RegularExpressions;
using BLink.Business.Models;
using BLink.Business.Managers;
using System.Net.Http;
using Xamarin.Auth;
using Newtonsoft.Json;
using System.Linq;

namespace BLink.Droid
{
    [Activity(Label = "Login", MainLauncher = false)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            Button registerButton = FindViewById<Button>(Resource.Id.btn_LinkRegister);
            registerButton.Click += RegisterButton_Click;
            Button loginButton = FindViewById<Button>(Resource.Id.btnLogin);
            loginButton.Click += LoginButton_Click;

            // Cleanup for accounts TODO Better cleanup
            string appName = GetString(Resource.String.app_name);
            Account account = AccountStore
              .Create(this)
              .FindAccountsForService(appName)
              .FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create(this).Delete(account, appName);
            }
            
            // Set our view from the "main" layout resource
        }

        private async void LoginButton_Click(object sender, System.EventArgs e)
        {
            EditText email = FindViewById<EditText>(Resource.Id.editLoginEmail);
            EditText password = FindViewById<EditText>(Resource.Id.editLoginPassword);
            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrEmpty(email.Text))
            {
                email.Error = "Въведете e-mail";
            }
            else if (!emailRegex.IsMatch(email.Text))
            {
                email.Error = "Въведете валиден e-mail";
            }
            else if (string.IsNullOrEmpty(password.Text))
            {
                password.Error = "Въведете парола";
            }
            else if (password.Text.Length < 5 && password.Text.Length > 15)
            {
                password.Error = "Парола трябва да е между 5 и 15 символа";
            }
            else
            {
                var loginUser = new LoginUser
                {
                    Email = email.Text,
                    Password = password.Text
                };

                HttpResponseMessage httpResponse = await RestManager.LoginUser(loginUser);
                if (httpResponse.IsSuccessStatusCode)
                {
                    string response = await httpResponse.Content.ReadAsStringAsync();
                    UserCredentials userCredentials =  JsonConvert.DeserializeObject<UserCredentials>(response);
                    Account account = new Account
                    {
                        Username = loginUser.Email
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
                    Toast.MakeText(ApplicationContext, "Грешен имейл и/или парола", ToastLength.Long).Show();
                }
            }
        }

        private void RegisterButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }
    }
}

