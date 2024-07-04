using System;
using System.Threading.Tasks;
using ExternalAccessory;
using Foundation;
using Microsoft.Maui.Controls;
using UsbApp.iOS;

//[assembly: Dependency(typeof(UsbSerialService_iOS))]
namespace UsbApp.iOS
{
    public class UsbSerialService_iOS : IUsbSerialService
    {
        private EASession _session;
        public bool IsConnected => _session != null;

        public async Task<bool> ConnectAsync()
        {
            try
            {
                var accessories = EAAccessoryManager.SharedAccessoryManager.ConnectedAccessories;
                var accessory = accessories.FirstOrDefault(a => a.ProtocolStrings.Contains("com.yourcompany.yourprotocol"));
                if (accessory == null)
                {
                    return false;
                }

                _session = new EASession(accessory, "com.yourcompany.yourprotocol");
                if (_session.InputStream != null)
                    _session.InputStream.Open();
                if (_session.OutputStream != null)
                    _session.OutputStream.Open();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to USB device: {ex.Message}");
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_session != null)
            {
                _session.InputStream?.Close();
                _session.OutputStream?.Close();
                _session = null;
            }
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            if (_session == null || _session.InputStream == null)
                throw new InvalidOperationException("Device not connected.");

            return await Task.Run(() =>
            {
                byte[] readBuffer = new byte[count];
                int bytesRead = (int)_session.InputStream.Read(readBuffer, (nuint)count);
                Array.Copy(readBuffer, 0, buffer, offset, bytesRead);
                return bytesRead;
            });
        }

        public async Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            if (_session == null || _session.OutputStream == null)
                throw new InvalidOperationException("Device not connected.");

            return await Task.Run(() =>
            {
                byte[] writeBuffer = new byte[count];
                Array.Copy(buffer, offset, writeBuffer, 0, count);
                _session.OutputStream.Write(writeBuffer, (nuint)count);
                return count;
            });
        }
    }
}