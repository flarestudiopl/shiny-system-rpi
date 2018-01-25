using System.Runtime.InteropServices;

namespace HardwareAccess.Buses.PlatformIntegration
{
    public interface ILibcWrapper
    {
        int OpenReadWrite(string fileName);
        int SendControl(int handle, int request, int data);
        int Write(int handle, byte[] data);
    }

    public class LibcWrapper : ILibcWrapper
    {
        [DllImport("libc.so.6", EntryPoint = "open")]
        private static extern int Open(string fileName, int mode);

        [DllImport("libc.so.6", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int Ioctl(int fd, int request, int data);

        [DllImport("libc.so.6", EntryPoint = "read", SetLastError = true)]
        private static extern int Read(int handle, byte[] data, int length);

        [DllImport("libc.so.6", EntryPoint = "write", SetLastError = true)]
        private static extern int Write(int handle, byte[] data, int length);

        private const int READ_WRITE_MODE = 2;

        public int OpenReadWrite(string fileName)
        {
            return Open(fileName, READ_WRITE_MODE);
        }

        public int SendControl(int handle, int request, int data)
        {
            return Ioctl(handle, request, data);
        }

        public int Write(int handle, byte[] data)
        {
            return Write(handle, data, data.Length);
        }
    }
}
