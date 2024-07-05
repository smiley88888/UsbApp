using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;


[assembly: Dependency(typeof(UsbApp.WinUI.UsbSerialService_Windows))]
namespace UsbApp.WinUI
{
    public class UsbSerialService_Windows : IUsbSerialService
    {
        private SerialPort _serialPort;

        public bool IsConnected => _serialPort?.IsOpen ?? false;

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _serialPort = new SerialPort("COM3", 115200); // Adjust COM port as needed
                _serialPort.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort = null;
            }
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await Task.Run(() => _serialPort.Read(buffer, offset, count));
        }

        public async Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            await Task.Run(() => _serialPort.Write(buffer, offset, count));
            return count; // Return the number of bytes intended to be written
        }
    }
}