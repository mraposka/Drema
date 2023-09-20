using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Drema
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GizlilikSozlesmesi : ContentPage
    {
        public GizlilikSozlesmesi()
        {
            InitializeComponent();

            AppTheme currentTheme = (AppTheme)Application.Current.RequestedTheme;

            if (currentTheme != AppTheme.Dark)
            {
                sozlesmeScrollView.BackgroundColor = Color.White;
                sozlesmeStackLayout.BackgroundColor = Color.White; 
                foreach (var view in sozlesmeStackLayout.Children)
                {
                    if (view is Label label)
                        label.TextColor = Color.FromHex("#3E206D");
                }
            }
            else
            {
                sozlesmeScrollView.BackgroundColor = Color.Black;
                sozlesmeStackLayout.BackgroundColor = Color.Black;   
                foreach (var view in sozlesmeStackLayout.Children)
                {
                    if (view is Label label) 
                        label.TextColor = Color.White; 
                }
            }
        }

        private void GeriButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("http://www.drema.info/privacy"));
        }
    }
}