using Plugin.Connectivity;
using System;
using Xamarin.Forms;

namespace Drema
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasNavigationBar(this, false);
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        DisplayAlert("Uyarı!", "İnternet bağlantısı mevcut değil. Lütfen internet bağlantınızı kontrol edin.", "Tamam");
                        GirisYapButton.BackgroundColor = Color.FromHex("#DADADA");//916DD5
                        HesapOlusturButton.BackgroundColor = Color.FromHex("#DADADA");//E4CDFF 
                        return true;
                    }
                    else
                    {
                        GirisYapButton.BackgroundColor = Color.FromHex("#916DD5");
                        HesapOlusturButton.TextColor = Color.FromHex("#E4CDFF");
                        HesapOlusturButton.BackgroundColor = Color.Transparent;
                        HesapOlusturButton.BorderColor = Color.FromHex("#916DD5");
                        return false;
                    }
                });
            }
            catch (Exception ex)
            { 
            }
        }
        private void HesapOlusturButton_Clicked(object sender, EventArgs e)
        {
            if (HesapOlusturButton.BackgroundColor != Color.FromHex("#DADADA"))
                Navigation.PushModalAsync(new HesapOlustur());
        }

        private void GirisYapButton_Clicked(object sender, EventArgs e)
        {
            if (GirisYapButton.BackgroundColor != Color.FromHex("#DADADA"))
                Navigation.PushModalAsync(new GirisYap());

        }
    }
}
