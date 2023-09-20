using Plugin.FirebasePushNotification;
using Xamarin.Essentials;
using Xamarin.Forms; 

namespace Drema
{
    public partial class App : Application
    {
        public static string token = "";
        public static bool IsInForeground { get; set; } = false;
        public App()
        {
            InitializeComponent();
            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
            CrossFirebasePushNotification.Current.OnNotificationReceived += Current_OnNotificationReceived;
            token = CrossFirebasePushNotification.Current.Token;
        }

        private void Current_OnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Notfiy:" + e.Data);
        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            token = e.Token;
        }
        protected override void OnStart()
        {
            base.OnStart();
            IsInForeground = true;
            int? userId = Preferences.ContainsKey("UserId") ? Preferences.Get("UserId", 0) : (int?)null;
            if (userId.HasValue)
            {

                // Kullanıcı zaten giriş yapmış. Anasayfayı yükleyin ve gerekirse ID'yi kullanın.
                MainPage = new NavigationPage(new Anasayfa(userId.Value.ToString()));
            }
            else
            {
                // Kullanıcı giriş yapmamış. Giriş sayfasını yükleyin.
                MainPage = new NavigationPage(new MainPage());
            }
        }

        protected override void OnSleep()
        {
            IsInForeground = false;
        }

        protected override void OnResume()
        {
            IsInForeground = true;
        }

    }
}
