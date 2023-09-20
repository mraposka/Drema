using Android.Util;
using Drema.Resources;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drema
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YorumDetay : ContentPage
    {
        string ruya_id, ruya_detail, ruya_title, ruya_date_asked, ruya_user_id;
        Singleton singleton=new Singleton();
        public YorumDetay(string title, string detail, string id, string date_asked, string user_id)
        {
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            ruyaTtitle.Text = title.Length >= 20 ? title.Substring(0, 20) + "..." : title;
            ruyaDetail.Text = detail;
            ruya_id = id;
            ruya_detail = detail;
            ruya_title = title;
            ruya_date_asked = date_asked;
            ruya_user_id = user_id;
        }

        private void GeriButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void PaylasButton_Clicked(object sender, EventArgs e)
        {

        }

        private async void SilButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var requestBody = new
                {
                    ruya_id = ruya_id,
                    ruya = ruya_title,
                    tabir = ruya_detail,
                    user_id = ruya_user_id,
                    date_asked = ruya_date_asked,
                    status = -1
                };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                var response = await client.PostAsync("https://lavirarocket.com/api/ruya_tabir/update.php", content);
                if (App.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is Anasayfa anasayfa)
                {
                    anasayfa.RefreshTabirs();
                }
                Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata!", Application.Current.MainPage.ToString() + singleton.GetCallForExceptionThisMethod(MethodBase.GetCurrentMethod(), ex) + ex.Message, "TAMAM"); 
            }


        }
    }
}