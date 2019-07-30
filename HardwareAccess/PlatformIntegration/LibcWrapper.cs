using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace HardwareAccess.PlatformIntegration
{
    public interface ILibcWrapper
    {
        int Open(string fileName, LibcOpenMode libcOpenMode);
        int Close(int handle);
        int SendControl(int handle, int request, int data);
        int Write(int handle, byte[] data);
        byte[] Read(int handle, int length);
    }

    public enum LibcOpenMode : int
    {
        Read = 0,
        Write = 1,
        ReadWrite = 2
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

        [DllImport("libc.so.6", EntryPoint = "close", SetLastError = true)]
        public static extern int CloseHandle(int handle);

        public int Open(string fileName, LibcOpenMode libcOpenMode)
        {
            return Open(fileName, (int)libcOpenMode);
        }

        public int Close(int handle)
        {
            return CloseHandle(handle);
        }

        public int SendControl(int handle, int request, int data)
        {
            return Ioctl(handle, request, data);
        }

        public int Write(int handle, byte[] data)
        {
            return Write(handle, data, data.Length);
        }

        public byte[] Read(int handle, int length)
        {
            var result = new byte[length];
            var bytesRead = Read(handle, result, length);

            if(bytesRead == -1)
            {
                throw new ArgumentException("Can't read from handle.", nameof(handle));
            }

            if(bytesRead == length)
            {
                return result;
            }

            return result.Take(bytesRead).ToArray();
        }
    }
}
