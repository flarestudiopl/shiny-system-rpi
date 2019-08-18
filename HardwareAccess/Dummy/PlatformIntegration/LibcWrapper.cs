using HardwareAccess.PlatformIntegration;

namespace HardwareAccess.Dummy.PlatformIntegration
{
    public class LibcWrapper : ILibcWrapper
    {
        public int Close(int handle)
        {
            return -1;
        }

        public int Open(string fileName, LibcOpenMode libcOpenMode)
        {
            return -1;
        }

        public byte[] Read(int handle, int length)
        {
            return new byte[] { 0 };
        }

        public int SendControl(int handle, int request, int data)
        {
            return -1;
        }

        public int Write(int handle, byte[] data)
        {
            return -1;
        }
    }
}
