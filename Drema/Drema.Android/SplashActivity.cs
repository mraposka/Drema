using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Drema.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drema.Droid
{
    [Activity(Theme = "@style/SplashTheme", Icon = "@mipmap/ic_launcher",Label ="Drema", NoHistory = true, MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
        }
    }

}