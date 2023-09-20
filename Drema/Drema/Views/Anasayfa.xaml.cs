using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Newtonsoft.Json.Linq;
using Android.Util;
using System.Reflection;
using System.Diagnostics;
using Android.Runtime;
using Drema.Resources;
using Newtonsoft.Json;
namespace Drema
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Anasayfa : ContentPage
    {
        public static List<RuyaTabir> oldRuyaListesi = new List<RuyaTabir>();
        private static readonly HttpClient client = new HttpClient();
        string user_id;
        string json = "";
        string result = "";
        Singleton singleton;
        public Anasayfa(string id)
        {
            try
            {
                InitializeComponent();
                singleton = new Singleton();
                try { oldRuyaListesi.Clear(); } catch (Exception) { }
                MessagingCenter.Subscribe<Anasayfa>(this, "RefreshTabirs", (sender) =>
                {
                    ListTabirs();
                });
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnOverlayTapped;
                OverlayBox.GestureRecognizers.Add(tapGestureRecognizer);

                var ruyaInputRecognizer = new TapGestureRecognizer();
                ruyaInputRecognizer.Tapped += (s, e) =>
                {
                    RuyaEditor.Focus();
                };
                RuyaInput.GestureRecognizers.Add(ruyaInputRecognizer);
                user_id = id;
                ListTabirs();
                Device.StartTimer(TimeSpan.FromSeconds(120), () =>
                {
                    if (App.IsInForeground)
                    {
                        ListTabirs();
                    }
                    return true;
                });
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }

        }
        public static string GetCallForExceptionThisMethod(MethodBase methodBase, Exception e)
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

        [Preserve(AllMembers = true)]
        public class User
        {
            public string user_id { get; set; }
            public string user_token { get; set; }
        }
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (App.token != Preferences.Get("token", "") && !singleton.tokenUpdated)
                {
                    using (var client = new HttpClient())
                    {
                        string url = "https://lavirarocket.com/api/ruya_user/update.php";
                        User newUser = new User
                        {
                            user_id = Preferences.Get("UserId", 0).ToString(),
                            user_token = App.token.Replace(" ", ""),
                        };

                        // JSON string'e çevir
                        var json = JsonConvert.SerializeObject(newUser);

                        // HTTP POST isteği oluştur
                        var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                        // Yanıtı kontrol et
                        if (!response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            JObject jsonResult = JObject.Parse(result);
                            string message = jsonResult["message"].ToString();
                            await Application.Current.MainPage.DisplayAlert("Hata", "Token güncellenemedi! " + message, "Tamam");
                        }
                        else
                            singleton.tokenUpdated = true;



                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM");
            }

        }
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            OverlayBox.IsVisible = false;
            PopupImage.IsVisible = false;
            PopupImage.Source = "yorum_sent.png";
        }
        void ShowImagePopup()
        {
            OverlayBox.IsVisible = true;
            PopupImage.IsVisible = true;
        }
        public void PopWait()
        {
            PopupImage.Source = "waitpop.png";
            ShowImagePopup();
        }
        //Class
        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Payload
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class ChatCompletionResponse
        {
            public string id { get; set; }

            [JsonProperty("object")]
            public string Object { get; set; }

            public int created { get; set; }
            public string model { get; set; }
            public List<Choice> choices { get; set; }
            public Usage usage { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Choice
        {
            public int index { get; set; }
            public Message message { get; set; }
            public string finish_reason { get; set; }
        }

        [Preserve(AllMembers = true)]
        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Ruya
        {
            public string ruya { get; set; }
            public string tabir { get; set; }
            public string user_id { get; set; }
            public string date_asked { get; set; }
            public string status { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class RuyaTabir
        {
            public string ruya_id { get; set; }
            public string ruya { get; set; }
            public string tabir { get; set; }
            public string user_id { get; set; }
            public string date_asked { get; set; }
            public string status { get; set; }
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                var r = (RuyaTabir)obj;
                return ruya == r.ruya && tabir == r.tabir && user_id == r.user_id && ruya_id == r.ruya_id && date_asked == r.date_asked && status == r.status;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ruya, tabir, user_id, ruya_id, date_asked, status);
            }
        }

        [Preserve(AllMembers = true)]
        public class RuyaListesi
        {
            public List<RuyaTabir> ruyas { get; set; }
        }
        //Class
        //Functions
        private void RuyaEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (RuyaEditor.Text.ToLower() == "q") { ListTabirs(); }
                if (RuyaEditor.Text.Length > 25) YourmlaButton.BackgroundColor = Color.FromHex("#3E206D");
                else YourmlaButton.BackgroundColor = Color.FromHex("#DADADA");

                if (RuyaEditor.Text.Length > 1000) RuyaEditor.Text = RuyaEditor.Text.Substring(0, 1000);
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM");
                Log.Info(Application.Current.MainPage.ToString(), GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message);
            }

        }
        public static string GetIstanbulTimeFormatted()
        {
            try
            {
                string timeServerUrl = "http://worldtimeapi.org/api/timezone/Europe/Istanbul.txt";
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    string response = client.DownloadString(timeServerUrl);
                    string[] lines = response.Split('\n');

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("datetime"))
                        {
                            string[] parts = line.Split(' ');
                            if (parts.Length >= 2)
                            {
                                string dateTimeString = parts[1].Trim();
                                DateTime istanbulTime = DateTime.Parse(dateTimeString);

                                // Belirli bir formatta tarih ve saat dizesi oluştur
                                string formattedDateTime = istanbulTime.ToString("dd.MM.yyyy HH:mm:ss");
                                return formattedDateTime;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return "01.01.1900 00:00:00";
        }
        private async void YorumlaButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (YourmlaButton.BackgroundColor != Color.FromHex("#DADADA"))
                {
                    if (!string.IsNullOrEmpty(RuyaEditor.Text))
                    {
                        string formattedDateTime = GetIstanbulTimeFormatted();
                        Ruya ruya = new Ruya { ruya = RuyaEditor.Text, tabir = "null", user_id = user_id, date_asked = formattedDateTime };
                        await AddTabir(ruya);
                    }
                }
                else
                {
                    await DisplayAlert("Uyarı!", "Rüya göndermek için lütfen kutucuğu doldurun", "TAMAM");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM");
            }

        }

        private void RuyaList_ChildAdded(object sender, ElementEventArgs e)
        {
            if (RuyaList.Children.Count > 0 && RuyaTitle.Text == "")
                RuyaTitle.Text = "Yorumlanan Rüyalar";
        }

        private async Task AddTabir(Ruya ruya)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(ruya);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://lavirarocket.com/api/ruya_tabir/create.php", content);
                if (response.StatusCode.ToString() == "Created")
                {
                    RuyaEditor.Text = "";
                    ShowImagePopup();
                    int wait = 0;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        wait++;
                        if (wait == 3) return false;
                        else return true;
                    });
                    ListTabirs();
                }
                else
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject jsonResult = JObject.Parse(result);
                    string message = jsonResult["message"].ToString();
                    if (message.Contains("limit")) PopLimit();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM");
            }

        }

        private void PopLimit()
        {
            PopupImage.Source = "limitpop.png";
            ShowImagePopup();
            RuyaEditor.Text = "";
        }
        [Preserve(AllMembers = true)]
        public class requestBody_Class
        {
            public string user_id { get; set; }
        }
        async void ListTabirs()
        {
            try
            {
                requestBody_Class requestBody = new requestBody_Class
                {
                    user_id = Preferences.Get("UserId", 0).ToString(),
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://lavirarocket.com/api/ruya_tabir/read_one.php", content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    RuyaListesi ruyaListesi = Newtonsoft.Json.JsonConvert.DeserializeObject<RuyaListesi>(responseString);
                    //ruyaListesi.ruyas.Reverse();
                    if (oldRuyaListesi.Count() == 0 || !oldRuyaListesi.SequenceEqual(ruyaListesi.ruyas))
                    {
                        RuyaList.Children.Clear();
                        foreach (var ruya in ruyaListesi.ruyas)
                        {
                            if (ruya.status != "-1")
                            {
                                CustomFrame newRuyaTabir = new CustomFrame(ruya.ruya, ruya.tabir, ruya.user_id, ruya.ruya_id, ruya.date_asked, ruya.status, ruya.ruya);
                                newRuyaTabir.CustomAction = PopWait;
                                RuyaList.Children.Add(newRuyaTabir);
                            }
                        }
                        oldRuyaListesi = ruyaListesi.ruyas.ToList();
                    }
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                    throw new Exception($"Unable to fetch data: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", GetCallForExceptionThisMethod(System.Reflection.MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM");
                Log.Info(Application.Current.MainPage.ToString(), GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message);
            }

        }
        private void Settings_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ProfilAyarlari());
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        double gridWidth;
        void SetPopupWidth()
        {
            double popupWidth = gridWidth * 0.8; // %80
            PopupImage.WidthRequest = popupWidth;
        }
        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            gridWidth = MainGrid.Width;
            SetPopupWidth();
        }
        public void RefreshTabirs()
        {
            oldRuyaListesi.Clear();
            ListTabirs();
        }
        private void Yorumla_Tapped(object sender, EventArgs e)
        {
            RuyaEditor.Focus();
        }
    }
}
