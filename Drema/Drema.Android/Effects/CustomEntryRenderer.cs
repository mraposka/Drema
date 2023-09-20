using Android.Graphics;
using Android.Views.InputMethods;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.OS;

[assembly: ExportRenderer(typeof(Entry), typeof(Drema.Droid.CustomEntryRenderer))]
namespace Drema.Droid
{ 
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                //Control.SetTextCursorDrawable(2131165300); 
                Android.Graphics.Color androidColor = new Android.Graphics.Color(Android.Graphics.Color.Rgb(228, 205, 255));
                Control.SetHighlightColor(androidColor);
            }
        }
    }
}
