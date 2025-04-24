using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Firebase.Auth;
using Microsoft.Maui.Storage;

namespace BeatsAppClient
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();

            ////Variables
            var fb_auth_config = new FirebaseAuthConfig();
            fb_auth_config.ApiKey = "";

            var login_fb_auth = new FirebaseAuthClient(fb_auth_config);
            ////
            Application.Current.PageAppearing += (s, e) =>
            {
                if (Preferences.ContainsKey("email"))
                {
                    Navigation.PushAsync(new Home());
                }
            };
            LoginButton.Clicked += (s, e) =>
            {
                var login_attempt = login_fb_auth.SignInWithEmailAndPasswordAsync(Email.Text, Password.Text).GetAwaiter();
                login_attempt.OnCompleted(() =>
                {
                    Preferences.Set("email", Email.Text);
                    Preferences.Set("password", Password.Text);
                    Preferences.Set("name", login_attempt.GetResult().User.Info.LastName);
                    Preferences.Set("username", login_attempt.GetResult().User.Info.DisplayName);
                    Preferences.Set("role", login_attempt.GetResult().User.Info.FirstName);
                    Navigation.PushAsync(new Home());
                });
            };
            SigninButton.Clicked += (s, e) =>
            {
                var signin_attemp = login_fb_auth.CreateUserWithEmailAndPasswordAsync(Email.Text, Password.Text).GetAwaiter();
                signin_attemp.OnCompleted(() =>
                {
                    signin_attemp.GetResult().User.Info.FirstName = Role.Text;
                    signin_attemp.GetResult().User.Info.LastName = Legal_Name.Text;
                    signin_attemp.GetResult().User.Info.DisplayName = Username.Text;
                    Preferences.Set("email", Email.Text);
                    Preferences.Set("password", Password.Text);
                    Preferences.Set("name", signin_attemp.GetResult().User.Info.LastName);
                    Preferences.Set("username", signin_attemp.GetResult().User.Info.DisplayName);
                    Preferences.Set("role", Role.Text);
                    Navigation.PushAsync(new Home());
                });
            };
        }
    }
}