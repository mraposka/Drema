using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using Drema;
using Drema.Resources;

public class CustomFrame : ContentView
{

    public CustomFrame(string text, string detail, string user, string ruya_id, string date_asked, string stat, string caption)
    {
        bool loadTimerRunning = false;
        string istanbulTime = Anasayfa.GetIstanbulTimeFormatted();
        Device.StartTimer(TimeSpan.FromSeconds(60), () =>
        {
            istanbulTime = Anasayfa.GetIstanbulTimeFormatted();
            return true;
        });
        Frame frame = new Frame
        {
            WidthRequest = 300,
            HeightRequest = 30,
            BackgroundColor = Color.White,
            CornerRadius = 12,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Start
        };
        StackLayout stackLayout = new StackLayout();
        Grid grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Image iconImage = new Image
        {
            Source = stat == "1" ? "" : string.IsNullOrEmpty(detail) || detail == "null" ? "wait.png" : "ok.png", // İkon resminin yolu burada belirtilmeli
            WidthRequest = 24,
            HeightRequest = 24,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(iconImage, 0);
        Label ruya = new Label
        {
            Text = detail == "null" || string.IsNullOrEmpty(detail) ? "Rüyan yorumlanıyor" : stat == "1" ? text.Length > 20 ? text.Substring(0, 20) + "..." : text : "Rüya yorumunu gör",
            FontAttributes = stat == "1" ? FontAttributes.None : FontAttributes.Bold,
            TextColor = detail == "null" || string.IsNullOrEmpty(detail) ? Color.FromHex("#3E206D") : stat == "0" ? Color.FromHex("#54B435") : Color.FromHex("#3E206D"),
            VerticalOptions = LayoutOptions.Center
        };
        Label sayac = new Label
        {
            Text = "",
            VerticalOptions = LayoutOptions.Center,
            TextColor = Color.FromHex("#3E206D"),
            FontSize = 14,
            FontAttributes = FontAttributes.Bold
        };
        if (ruya.Text.Contains("Rüyan yorumlanıyor") && !loadTimerRunning)
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                loadTimerRunning = true;
                DateTime dt1;
                DateTime dt2;
                if (DateTime.TryParseExact(istanbulTime, "dd.MM.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt1)
                && DateTime.TryParseExact(date_asked, "dd.MM.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt2))
                {
                    TimeSpan diff = dt1 - dt2;
                    int diffMinutes = 60 - Convert.ToInt16(diff.TotalMinutes);

                    if (diffMinutes >= 1) sayac.Text = diffMinutes.ToString() + " dk";
                    else sayac.Text = "1 dk";
                    iconImage.RotateTo(180 + iconImage.Rotation, 1000).ContinueWith(t =>
                    {
                        if (iconImage.Rotation >= 3600)
                            iconImage.Rotation = 0;
                    });
                }
                return true;
            });   
        }

        Label id = new Label
        {
            Text = ruya_id
        };

        Label date = new Label
        {
            Text = date_asked
        };
        Grid.SetColumn(ruya, 1);
        Image image = new Image
        {
            Source = "arrow.png",
            Rotation = 180,
            HeightRequest = 50,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };
        TapGestureRecognizer tapGesture = new TapGestureRecognizer();
        // Tıklama olayında ne olacağını belirleyen kod
        tapGesture.Tapped += (s, e) =>
        {
            if (!string.IsNullOrEmpty(detail) && detail != "null")
            {
                Navigation.PushModalAsync(new YorumDetay(caption, detail, ruya_id, date_asked, user));
                //status 0
                var requestBody = new
                {

                    ruya_id = ruya_id,
                    ruya = text,
                    tabir = detail,
                    user_id = user,
                    date_asked = date_asked,
                    status = 1
                };
                ruya.Text = text.Length > 20 ? text.Substring(0, 20) + "..." : text;
                ruya.TextColor = Color.FromHex("#3E206D");
                ruya.FontAttributes = FontAttributes.None;
                iconImage.Source = "";
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                statusUpdate(content);
            }
            else
            {
                CallCustomAction();
            }
        };

        this.GestureRecognizers.Add(tapGesture);
        Grid.SetColumn(sayac, 2);
        Grid.SetColumn(image, 3);
        Grid.SetColumn(iconImage, 0);
        grid.Children.Add(iconImage);
        grid.Children.Add(image);
        grid.Children.Add(sayac);
        grid.Children.Add(ruya);
        stackLayout.Children.Add(grid);
        frame.Content = stackLayout;
        Content = frame;
    }
    public Action CustomAction { get; set; }

    public void CallCustomAction()
    {
        CustomAction?.Invoke();
    }
    void statusUpdate(StringContent content)
    {
        HttpClient client = new HttpClient();
        client.PostAsync("https://"+Singleton.apiURL+"/api/ruya_tabir/update.php", content).Wait();
    }
    public class Ruya
    {
        public string ruya { get; set; }
        public string tabir { get; set; }
        public string user_id { get; set; }
        public string date_asked { get; set; }
        public string status { get; set; }
    }
}
