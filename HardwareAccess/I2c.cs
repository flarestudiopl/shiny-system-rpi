using Commons;
using System.Diagnostics;
using System.Text;

namespace HardwareAccess
{
    public interface II2c
    {
        string GetI2cDetectResult();
        void WriteToDevice(int i2cDevice, byte value);
    }

    public class I2c : II2c
    {
        private static int OPEN_READ_WRITE = 2;

        private readonly int _i2cBusHandle;

        public I2c()
        {
            _i2cBusHandle = LibcWrapper.Open("/dev/i2c-1", OPEN_READ_WRITE);
        }

        public string GetI2cDetectResult()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "i2cdetect",
                    Arguments = "-y 1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            var stringBuilder = new StringBuilder();

            while (!proc.StandardOutput.EndOfStream)
            {
                stringBuilder.AppendLine(proc.StandardOutput.ReadLine());
            }

            return stringBuilder.ToString();
        }

        public void WriteToDevice(int i2cDevice, byte value)
        {
            if (LibcWrapper.Ioctl(_i2cBusHandle, 0x0703, i2cDevice) >= 0)
            {
                LibcWrapper.Write(_i2cBusHandle, new[] { value }, 1);
            } else
            {
                Logger.Warning("IOCTL < 0");
            }
        }
    }
}
