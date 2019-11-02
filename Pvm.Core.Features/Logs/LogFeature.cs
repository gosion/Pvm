using System;
using System.Collections;
using System.Threading.Tasks;
using Pvm.Core.Abstractions;
using Pvm.Core.Abstractions.Features;

namespace Pvm.Core.Features.Logs
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error
    }

    public class LogFeature : ILogFeature, IFeature
    {
        private Queue queue = Queue.Synchronized(new Queue());
        public bool IsEnabled { get; private set; } = false;
        public ILogWriter Writer { get; set; }

        public LogFeature(LogOptions options = null)
        {
            this.Writer = options?.LogWriter ?? new ConsoleWriter();
        }

        public void Enable()
        {
            this.IsEnabled = true;
        }

        public void Disable()
        {
            this.IsEnabled = false;
        }

        public async Task Debug(string message, params object[] parameters)
        {
            this.add(message, LogLevel.Debug, parameters);
            await Task.Yield();
            this.write();
        }

        /// <summary>
        /// Add log to queue and write the first message of queue to file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task Info(string message, params object[] parameters)
        {
            this.add(message, LogLevel.Info, parameters);
            await Task.Yield();
            this.write();
        }

        public async Task Warning(string message, params object[] parameters)
        {
            this.add(message, LogLevel.Warning, parameters);
            await Task.Yield();
            this.write();
        }

        public async Task Error(string message, params object[] parameters)
        {
            this.add(message, LogLevel.Error, parameters);
            await Task.Yield();
            this.write();
        }

        private void add(string message, LogLevel level = LogLevel.Info, params object[] args)
        {
            if (this.IsEnabled)
            {
                if (args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                this.queue.Enqueue($"{DateTime.UtcNow} - [{level.ToString()}]: {message}{Environment.NewLine}");
            }
        }

        private void write()
        {
            try
            {
                while (this.queue.Count > 0)
                {
                    string message = (string)this.queue.Dequeue();
                    Writer.Write(message).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
