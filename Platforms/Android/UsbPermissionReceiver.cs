using Android.Content;
using Android.Hardware.Usb;

namespace UsbApp.Droid
{
    public class UsbPermissionReceiver : BroadcastReceiver
    {
        public static TaskCompletionSource<bool> PermissionResultSource { get; set; }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "USB_PERMISSION")
            {
                bool permissionGranted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
                UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);

                PermissionResultSource?.TrySetResult(permissionGranted && device != null);
            }
        }
    }
}