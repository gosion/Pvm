namespace Pvm.Core.Features.Logs
{
    public sealed class LogOptions
    {
        public ILogWriter LogWriter { get; set; }
        public string LogPath { get; set; }
    }
}
