using Microsoft.Maui.Controls;
using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsbApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IUsbSerialService _usbSerialService;
        private CancellationTokenSource _readCancellationTokenSource;

        public MainPage()
        {
            InitializeComponent();

            _readCancellationTokenSource = new CancellationTokenSource();

            _usbSerialService = DependencyService.Get<IUsbSerialService>();
            if (_usbSerialService == null)
            {
                throw new InvalidOperationException("Unable to resolve UsbSerialService.");
            }

            InitializeUsbSerial();
        }

        private async void InitializeUsbSerial()
        {
            var isConnected = await _usbSerialService.ConnectAsync();
            StatusLabel.Text = isConnected ? "Connected" : "Disconnected";
            if (isConnected)
            {
                StartReading();
            }
        }

        private void StartReading()
        {
            
            Task.Run(async () =>
            {
                var buffer = new byte[1024];
                while (!_readCancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var bytesRead = await _usbSerialService.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            var receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DataLabel.Text = receivedData;
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            StatusLabel.Text = $"Read Error: {ex.Message}";
                        });
                    }
                }
            }, _readCancellationTokenSource.Token);
        }

        private async void OnSendOneClicked(object sender, EventArgs e)
        {
            await SendDataAsync("1");
        }

        private async void OnSendZeroClicked(object sender, EventArgs e)
        {
            await SendDataAsync("0");
        }

        private async Task SendDataAsync(string data)
        {
            if (_usbSerialService != null && _usbSerialService.IsConnected)
            {
                var buffer = Encoding.ASCII.GetBytes(data);
                await _usbSerialService.WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                StatusLabel.Text = "Device not connected";
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _readCancellationTokenSource?.Cancel();
            _usbSerialService?.DisconnectAsync();
        }
    }
}