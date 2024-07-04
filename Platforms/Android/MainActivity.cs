using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace UsbApp.Droid
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        private UsbPermissionReceiver _usbPermissionReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Register USB permission receiver
            //_usbPermissionReceiver = new UsbPermissionReceiver();
            //var filter = new IntentFilter("USB_PERMISSION");
            //RegisterReceiver(_usbPermissionReceiver, filter);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Unregister USB permission receiver
            UnregisterReceiver(_usbPermissionReceiver);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}