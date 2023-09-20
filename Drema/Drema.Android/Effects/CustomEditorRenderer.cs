using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(Editor), typeof(Drema.Droid.CustomEditorRenderer))]
namespace Drema.Droid
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        { 
        } 
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                //Control.SetTextCursorDrawable(Drema.Droid.Resource.Drawable.cursor); 
                Android.Graphics.Color androidColor = new Android.Graphics.Color(Android.Graphics.Color.Rgb(228,205,255));
                Control.SetHighlightColor(androidColor); 
            }

        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null && e.PropertyName == nameof(Editor.Text))
            {
                Control.Gravity = GravityFlags.CenterHorizontal;
            }
        }
    }
}