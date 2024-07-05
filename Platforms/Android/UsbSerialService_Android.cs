using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Microsoft.Maui.Controls;


[assembly: Dependency(typeof(UsbApp.Droid.UsbSerialService_Android))]
namespace UsbApp.Droid
{
    public class UsbSerialService_Android : IUsbSerialService
    {
        private UsbManager _usbManager;
        private UsbDevice _usbDevice;
        private UsbDeviceConnection _connection;
        private UsbEndpoint _endpointIn;
        private UsbEndpoint _endpointOut;

        public bool IsConnected => _connection != null;

        public UsbSerialService_Android()
        {
            _usbManager = (UsbManager)Android.App.Application.Context.GetSystemService(Context.UsbService);
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                var deviceList = _usbManager.DeviceList;
                _usbDevice = deviceList.Values.FirstOrDefault();
                if (_usbDevice == null)
                {
                    return false;
                }

                if (_usbManager.HasPermission(_usbDevice))
                {
                    return InitializeConnection();
                }
                else
                {
                    var permissionIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, new Intent("USB_PERMISSION"), 0);
                    UsbPermissionReceiver.PermissionResultSource = new TaskCompletionSource<bool>();
                    var receiver = new UsbPermissionReceiver();
                    Android.App.Application.Context.RegisterReceiver(receiver, new IntentFilter("USB_PERMISSION"));
                    _usbManager.RequestPermission(_usbDevice, permissionIntent);

                    bool permissionGranted = await UsbPermissionReceiver.PermissionResultSource.Task;
                    Android.App.Application.Context.UnregisterReceiver(receiver);
                    return permissionGranted && InitializeConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to USB device: {ex.Message}");
                return false;
            }
        }

        private bool InitializeConnection()
        {
            _connection = _usbManager.OpenDevice(_usbDevice);
            if (_connection == null)
            {
                return false;
            }

            var usbInterface = _usbDevice.GetInterface(0);
            _endpointIn = usbInterface.GetEndpoint(0);
            _endpointOut = usbInterface.GetEndpoint(1);

            _connection.ClaimInterface(usbInterface, true);

            return true;
        }

        public async Task DisconnectAsync()
        {
            _connection?.ReleaseInterface(_usbDevice.GetInterface(0));
            _connection?.Close();
            _connection = null;
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            if (_connection == null || _endpointIn == null)
            {
                throw new InvalidOperationException("Connection or endpoint is not initialized.");
            }

            return await Task.Run(() =>
            {
                byte[] packet = new byte[_endpointIn.MaxPacketSize];
                int readBytes = _connection.BulkTransfer(_endpointIn, packet, packet.Length, 1000);
                if (readBytes > 0)
                {
                    Array.Copy(packet, 0, buffer, offset, Math.Min(readBytes, count));
                }
                return readBytes;
            });
        }

        public async Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            if (_connection == null || _endpointOut == null)
            {
                throw new InvalidOperationException("Connection or endpoint is not initialized.");
            }

            return await Task.Run(() =>
            {
                byte[] packet = new byte[_endpointOut.MaxPacketSize];
                Array.Copy(buffer, offset, packet, 0, Math.Min(count, packet.Length));
                return _connection.BulkTransfer(_endpointOut, packet, packet.Length, 1000);
            });
        }
    }
}