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
using Android.Support.V7.App;
using AndroidHUD;

namespace BLink.Droid
{
    [Activity(Label = "Вход", Theme = "@style/ActionBarTheme")]
    public class LoginActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            Button registerButton = FindViewById<Button>(Resource.Id.btn_login_linkRegister);
            registerButton.Click += RegisterButton_Click;
            Button loginButton = FindViewById<Button>(Resource.Id.btn_login_login);
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
        }

        private async void LoginButton_Click(object sender, System.EventArgs e)
        {
            EditText email = FindViewById<EditText>(Resource.Id.et_login_email);
            EditText password = FindViewById<EditText>(Resource.Id.et_login_password);
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

                AndHUD.Shared.Show(this, "Влизане…");
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
                    account.Properties.Add("memberId", userCredentials.MemberId.ToString());

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

                AndHUD.Shared.Dismiss(this);
            }
        }

        private void RegisterButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }
    }
}

