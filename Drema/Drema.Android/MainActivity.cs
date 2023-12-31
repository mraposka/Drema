﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.FirebasePushNotification;
using Android;

namespace Drema.Droid
{
    [Activity(Label = "Drema", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false,
         ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
         ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Firebase.FirebaseApp.InitializeApp(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                RequestPermissions(new string[] { Manifest.Permission.ReceiveBootCompleted, Manifest.Permission.ForegroundService, Manifest.Permission.PostNotifications, Manifest.Permission.BindNotificationListenerService, Manifest.Permission.AccessNotificationPolicy }, 0);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}