using Drema.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System; 
using System.Linq;
using System.Net.Http; 
using System.Text;
using System.Threading.Tasks; 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drema
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HesapOlustur : ContentPage
    {
        public HesapOlustur()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text) && !string.IsNullOrEmpty(usermail.Text))
                HesapOlustur_Buton.BackgroundColor = Color.FromHex("#916DD5");
            else
                HesapOlustur_Buton.BackgroundColor = Color.FromHex("#DADADA");
        }
        public async Task CreateNewUserAsync(string userName, string userPass, string userEmail)
        {
            Singleton singleton1 = new Singleton();
            try
            {
                Drema.Resources.Singleton singleton = new Drema.Resources.Singleton();
                using (var client = new HttpClient())
                {
                    string url = "https://"+Singleton.apiURL+"/api/ruya_user/create.php";
                    var newUser = new
                    {
                        user_name = userName,
                        user_pass = singleton.CalculateSHA256Hash(userPass),
                        user_mail = userEmail,
                        token = App.token.ToString().Replace(" ", "")
                    };

                    var json = JsonConvert.SerializeObject(newUser); 
                    var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")); 
                    if (response.IsSuccessStatusCode)
                    {
                        ShowImagePopup();
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jsonResult = JObject.Parse(result);
                        string message = jsonResult["message"].ToString();
                        await Application.Current.MainPage.DisplayAlert("Hata", "Kullanıcı oluşturulamadı. " + message, "Tamam");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", singleton1.GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }
        }
        private void GeriButton_Clicked(object sender, EventArgs e)
        { 
            Navigation.PopModalAsync();
        }
        private async void GirisButton_Clicked(object sender, EventArgs e)
        {  
                await Navigation.PushModalAsync(new GirisYap()); 
        }
        private async void GizlilikSozlesmesi_Click(object sender, EventArgs e)
        { 
            await Navigation.PushModalAsync(new GizlilikSozlesmesi());
        }

        private async void HesapOlusturButton_Clicked(object sender, EventArgs e)
        {
            if (HesapOlustur_Buton.BackgroundColor != Color.FromHex("#DADADA"))
            {
                if (GizlilikSozlesmesiCheckBox.IsChecked)
                {
                    if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text) && !string.IsNullOrEmpty(usermail.Text))
                    {
                        if (username.Text.Length <= 50 && password.Text.Length <= 255 && usermail.Text.Length <= 255 && username.Text.Length >= 5 && usermail.Text.Contains("@") && usermail.Text.Contains(".") && password.Text.Length >= 8 && usermail.Text.Length >= 8)
                        {
                            await CreateNewUserAsync(username.Text, password.Text, usermail.Text);
                            HesapOlustur_Buton.BackgroundColor = Color.FromHex("#DADADA");
                            int tick = 0;
                            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                            {
                                tick++;
                                if (tick > 3) { HesapOlustur_Buton.BackgroundColor = Color.FromHex("#916DD5"); return false; }
                                return true;
                            });
                        }
                        else
                        {
                            await DisplayAlert("Uyarı!", "Kullanıcı adı, e-posta ve şifre boşluklarını kurallara uygun doldurmanız gerekmektedir.", "TAMAM");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Uyarı!", "Kullanıcı adı, e-posta ve şifre boşluklarını doldurmanız gerekmektedir.", "TAMAM");
                    }
                }
                else
                {
                    await DisplayAlert("Uyarı!", "Gizlilik sözleşmesini onaylamanız gerekmektedir.", "TAMAM");
                }
            }
            else
            {
                await DisplayAlert("Uyarı!", "Kayıt olmak için lütfen kutucukları doldurun", "TAMAM");
            }
        }
        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(username.Text))
            {
                if (username.Text.Length >= 5)
                    UserToolTip.IsVisible = false;
                else
                    UserToolTip.IsVisible = true;
            }
        }
        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text) && !string.IsNullOrEmpty(usermail.Text))
            {
                HesapOlustur_Buton.BackgroundColor = Color.FromHex("#916DD5");
            }
            else HesapOlustur_Buton.BackgroundColor = Color.FromHex("#DADADA");
            if (!string.IsNullOrEmpty(password.Text))
            {
                if (password.Text.Length >= 8)
                    PasswordToolTip.IsVisible = false;
                else
                    PasswordToolTip.IsVisible = true;
            }
        }
        private void usermail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(usermail.Text))
            {
                if (usermail.Text.Length >= 5 && usermail.Text.Contains("@") && usermail.Text.Contains("."))
                    MailToolTip.IsVisible = false;
                else
                    MailToolTip.IsVisible = true;
            }
        }
        private void ShowHidePassword(object sender, EventArgs e)
        {
            password.IsPassword = !password.IsPassword;
        }
        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            OverlayBox.IsVisible = false;
            PopupImage.IsVisible = false;
            await Navigation.PopModalAsync(true);
        }
        void ShowImagePopup()
        {
            OverlayBox.IsVisible = true;
            PopupImage.IsVisible = true;
        }
        private void password_Focused(object sender, FocusEventArgs e)
        {
            PasswordToolTip.IsVisible = true;
            PasswordRow.Height = 105;
        }

        private void password_Unfocused(object sender, FocusEventArgs e)
        {
            PasswordToolTip.IsVisible = false;
            PasswordRow.Height = 80;
        }

        private void usermail_Focused(object sender, FocusEventArgs e)
        {
            MailToolTip.IsVisible = true;
            E_PostaRow.Height = 105;
        }
        private void usermail_Unfocused(object sender, FocusEventArgs e)
        {
            MailToolTip.IsVisible = false;
            E_PostaRow.Height = 80;
        }
        private void username_Focused(object sender, FocusEventArgs e)
        {
            UserToolTip.IsVisible = true;
            UsernameRow.Height = 105;
        }
        private void username_Unfocused(object sender, FocusEventArgs e)
        {
            UserToolTip.IsVisible = false;
            UsernameRow.Height = 80;
        }
    }
}
