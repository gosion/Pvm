using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pvm.Core.Features.Logs
{
    public sealed class FileWriter : ILogWriter
    {
        public ReaderWriterLock locker { get; private set; } = new ReaderWriterLock();
        public string FilePath { get; private set; }

        public FileWriter(LogOptions options = null)
        {
            this.FilePath = options?.LogPath ?? "logs/info.log";
        }

        public async Task Write(string message)
        {
            Task task = Task.CompletedTask;

            try
            {
                locker.AcquireWriterLock(int.MaxValue);

                string dir = Path.GetDirectoryName(this.FilePath);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                using (StreamWriter sw = File.AppendText(this.FilePath))
                {
                    task = sw.WriteAsync(message);
                }
            }
            finally
            {
                await task;
                if (locker.IsWriterLockHeld)
                {
                    locker.ReleaseWriterLock();
                }
            }
        }
    }
}
