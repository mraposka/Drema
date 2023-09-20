using System; 
using System.Net.Http;
using System.Text; 
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Drema.Resources;  
using System.Reflection;
using Android.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drema
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilAyarlari : ContentPage
    {
        bool passwordChanged = false;
        public ProfilAyarlari()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, false);
                username.Text = Preferences.Get("UserName", "");
                password.Text = Preferences.Get("Password", "");
                usermail.Text = Preferences.Get("UserMail", "");
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", singleton1.GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }

        }
        [Preserve(AllMembers = true)]
        public class UpdateUserName_Class
        {
            public string user_name { get; set; }
            public string user_id { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class UpdateUserMail_Class
        {
            public string user_mail { get; set; }
            public string user_id { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class UpdateUserPass_Class
        {
            public string user_pass { get; set; }
            public string user_id { get; set; }
        }
        private void GeriButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        Singleton singleton1 = new Singleton();
        private async void KaydetButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (passwordChanged == false)
                {
                    if (Preferences.Get("UserName", "") == username.Text)
                    {
                        if (Preferences.Get("UserMail", "") == usermail.Text)
                            DisplayAlert("Uyarı!", "Herhangi bir değişiklik yapmadınız!", "TAMAM");
                        else
                            UpdateUserMail(usermail.Text);
                    }
                    else
                        UpdateUserName(username.Text);
                }
                else
                {
                    if (password.Text != Preferences.Get("Password", ""))
                        UpdateUserPass(singleton1.CalculateSHA256Hash(password.Text));
                    else
                        DisplayAlert("Uyarı!", "Eski şifreniz yeni şifreniz ile aynı olamaz!", "TAMAM");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", Application.Current.MainPage.ToString() + singleton1.GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }

        }

        async void UpdateUserMail(string mail)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = "https://lavirarocket.com/api/ruya_user/update.php";
                    Drema.Resources.Singleton singleton = new Drema.Resources.Singleton();
                    UpdateUserMail_Class newUser = new UpdateUserMail_Class
                    {
                        user_id = Preferences.Get("UserId", 0).ToString(),
                        user_mail = mail
                    };

                    // JSON string'e çevir
                    var json = JsonConvert.SerializeObject(newUser);

                    DisplayAlert("Q", json, "OK");
                    // HTTP POST isteği oluştur
                    var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                    // Yanıtı kontrol et
                    if (!response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jsonResult = JObject.Parse(result);
                        string message = jsonResult["message"].ToString();
                        await Application.Current.MainPage.DisplayAlert("Hata", message, "Tamam");
                    }
                    else
                    {
                        Preferences.Set("UserName", username.Text);
                        Preferences.Set("Password", password.Text);
                        Preferences.Set("UserMail", usermail.Text);
                        passwordChanged = false;
                        DisplayAlert("Bilgi!", "Değişiklikleriniz başarıyla kaydedildi!", "TAMAM");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", Application.Current.MainPage.ToString() + singleton1.GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }

        }
        async void UpdateUserPass(string pass)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = "https://lavirarocket.com/api/ruya_user/update.php";
                    Drema.Resources.Singleton singleton = new Drema.Resources.Singleton();
                    UpdateUserPass_Class newUser = new UpdateUserPass_Class
                    {
                        user_id = Preferences.Get("UserId", 0).ToString(),
                        user_pass = pass
                    };

                    // JSON string'e çevir
                    var json = JsonConvert.SerializeObject(newUser);

                    DisplayAlert("Q", json, "OK");

                    // HTTP POST isteği oluştur
                    var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                    // Yanıtı kontrol et
                    if (!response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jsonResult = JObject.Parse(result);
                        string message = jsonResult["message"].ToString();
                        await Application.Current.MainPage.DisplayAlert("Hata", message, "Tamam");
                    }
                    else
                    {
                        Preferences.Set("UserName", username.Text);
                        Preferences.Set("Password", password.Text);
                        Preferences.Set("UserMail", usermail.Text);
                        passwordChanged = false;
                        DisplayAlert("Bilgi!", "Değişiklikleriniz başarıyla kaydedildi!", "TAMAM");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", Application.Current.MainPage.ToString() + singleton1.GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }
        }
        async void UpdateUserName(string name)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = "https://lavirarocket.com/api/ruya_user/update.php";
                    Drema.Resources.Singleton singleton = new Drema.Resources.Singleton();
                    UpdateUserName_Class newUser = new UpdateUserName_Class
                    {
                        user_id = Preferences.Get("UserId", 0).ToString(),
                        user_name = name
                    };

                    // JSON string'e çevir
                    var json = JsonConvert.SerializeObject(newUser);

                    DisplayAlert("Q", json, "OK");

                    // HTTP POST isteği oluştur
                    var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                    // Yanıtı kontrol et
                    if (!response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jsonResult = JObject.Parse(result);
                        string message = jsonResult["message"].ToString();
                        await Application.Current.MainPage.DisplayAlert("Hata", message, "Tamam");
                    }
                    else
                    {
                        Preferences.Set("UserName", username.Text);
                        Preferences.Set("Password", password.Text);
                        Preferences.Set("UserMail", usermail.Text);
                        passwordChanged = false;
                        DisplayAlert("Bilgi!", "Değişiklikleriniz başarıyla kaydedildi!", "TAMAM");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", Application.Current.MainPage.ToString() + singleton1.GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }

        }
        private void CikisButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Preferences.Remove("UserId");
                Preferences.Remove("UserName");
                Preferences.Remove("Password");
                Preferences.Remove("UserMail");
                var pageOne = new MainPage();
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(pageOne, false);
                Xamarin.Forms.NavigationPage mypage = new Xamarin.Forms.NavigationPage(pageOne);
                App.Current.MainPage = mypage; 
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", Application.Current.MainPage.ToString() + singleton1.GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            } 
        }
        private void UserNameEdit(object sender, EventArgs e)
        {
            username.Focus();
        }
        private void PasswordEdit(object sender, EventArgs e)
        {
            password.Focus();
        }

        private void UserMailEdit(object sender, EventArgs e)
        {
            usermail.Focus();
        }

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordChanged = true;
        } 
    }
}