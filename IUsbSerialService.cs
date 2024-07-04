
namespace UsbApp
{
    public interface IUsbSerialService
    {
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
        Task<int> WriteAsync(byte[] buffer, int offset, int count);
        bool IsConnected { get; }
    }
}