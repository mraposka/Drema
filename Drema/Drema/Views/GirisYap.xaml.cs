using Android.Runtime;
using Android.Util;
using Drema.Resources;
using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection; 
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drema
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GirisYap : ContentPage
    {
        public GirisYap()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        [Preserve(AllMembers = true)]
        public class User
        {
            public string user_id { get; set; }
            public string user_name { get; set; }
            public string user_token { get; set; }
            public string user_pass { get; set; }
            public string user_mail { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class UsersList
        {
            public List<User> users { get; set; }
        }

        private void GeriButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private static string GetCallForExceptionThisMethod(MethodBase methodBase, Exception e)
        {
            StackTrace trace = new StackTrace(e);
            StackFrame previousFrame = null;

            foreach (StackFrame frame in trace.GetFrames())
            {
                if (frame.GetMethod() == methodBase)
                {
                    break;
                }

                previousFrame = frame;
            }

            return previousFrame != null ? previousFrame.GetMethod().Name : null;
        }
        private async void KayıtOlButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new HesapOlustur());
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }

        }
        public async Task<List<User>> GetUsersAsync(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var usersJson = jsonString.Split('[')[1].Split(']')[0];
                    usersJson = "[" + usersJson + "]";
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(usersJson);
                    return users;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
                return null; 
            }

        }
        private async void GirisButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text) && GirisBtn.BackgroundColor != Color.FromHex("#DADADA"))
                {
                    if (username.Text.Length <= 50 && password.Text.Length <= 255)
                    {
                        GirisBtn.BackgroundColor = Color.FromHex("#DADADA");
                        bool isLoggedIn = false;
                        string url = "https://"+Singleton.apiURL+"/api/ruya_user/read.php";
                        Drema.Resources.Singleton singleton = new Drema.Resources.Singleton();
                        List<User> users = await GetUsersAsync(url);
                        if (users != null)
                        {
                            foreach (var user in users)
                            {
                                if (user.user_name == username.Text && user.user_pass == singleton.CalculateSHA256Hash(password.Text))
                                {
                                    Preferences.Set("UserId", Convert.ToInt32(user.user_id));
                                    Preferences.Set("UserName", user.user_name);
                                    Preferences.Set("Password", password.Text);
                                    Preferences.Set("UserMail", user.user_mail);
                                    Preferences.Set("token", user.user_token);
                                    var pageOne = new Anasayfa(user.user_id);
                                    NavigationPage.SetHasNavigationBar(pageOne, false);
                                    NavigationPage mypage = new NavigationPage(pageOne);
                                    App.Current.MainPage = mypage;
                                    isLoggedIn = true;

                                    break;
                                }

                            }
                            if (!isLoggedIn)
                            {
                                await DisplayAlert("Hata", "Kullanıcı adı veya şifre hatalı!", "TAMAM");
                                GirisBtn.BackgroundColor = Color.FromHex("#916DD5");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Connection Error 0001!", "OK");
                        }
                    }
                    else { await DisplayAlert("Uyarı!", "Giriş yapmak için lütfen kutucukları kurallara uygun şekilde doldurun", "TAMAM"); }

                }
                else { await DisplayAlert("Uyarı!", "Giriş yapmak için lütfen kutucukları doldurun", "TAMAM"); }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM");
                Log.Info(Application.Current.MainPage.ToString(), GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message);
            }

        }

        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text)) GirisBtn.BackgroundColor = Color.FromHex("#916DD5");
            else GirisBtn.BackgroundColor = Color.FromHex("#DADADA");
        }

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text)) GirisBtn.BackgroundColor = Color.FromHex("#916DD5");
            else GirisBtn.BackgroundColor = Color.FromHex("#DADADA");
        }

        private void ShowHidePassword(object sender, EventArgs e)
        {
            password.IsPassword = !password.IsPassword;
        }
    }
}