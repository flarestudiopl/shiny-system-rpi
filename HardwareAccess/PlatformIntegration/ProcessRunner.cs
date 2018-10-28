using System.Diagnostics;
using System.Threading.Tasks;

namespace HardwareAccess.PlatformIntegration
{
    public interface IProcessRunner
    {
        Task<string> Run(string fileName, string arguments);
    }

    public class ProcessRunner : IProcessRunner
    {
        public async Task<string> Run(string fileName, string arguments)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            return await proc.StandardOutput.ReadToEndAsync();
        }
    }
}
