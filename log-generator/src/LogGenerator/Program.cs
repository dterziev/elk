using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LogGenerator
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static int Main(string[] args)
        {
            Console.WriteLine(AppContext.BaseDirectory);

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));
            AppDomain.CurrentDomain.UnhandledException += (s, e) => 
            {
                Log.FatalFormat("Unhandled error occured. Terminating the process. Data loss might occur. Exception: {0}", e.ExceptionObject);
                Environment.ExitCode = -1;
            };

            var cts = new CancellationTokenSource();
            var exitRequestedTask = Task.Delay(int.MaxValue, cts.Token);

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var workerTask = Task.Run(
                () => LogStuff(cts.Token),
                cts.Token);

            Console.WriteLine("Press Ctrl+C to exit...");
            throw new Exception("this is unhandled");
            try
            {
                Task.WaitAny(exitRequestedTask, workerTask);

                if (!workerTask.Wait(TimeSpan.FromSeconds(5)))
                {
                    Log.Fatal("Shutdown timeout expired. Terminating the process. Data loss might occur.");
                    return -1;
                }
            }
            catch (AggregateException ex)
            {
                try
                {
                    ex.Flatten().Handle(exception => exception is TaskCanceledException);
                }
                catch (AggregateException ex2)
                {
                    Log.Fatal("Unexpected error occured. Terminating the process.", ex2);
                    return -1;
                }
            }

            Log.Info("Process terminated successfully.");
            return 0;
        }

        private static void LogStuff(CancellationToken cancellationToken)
        {
            var rnd = new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                if(rnd.Next(100) > 90)
                {
                    LogException();
                }
                else
                {
                    LogEvent(
                        Guid.NewGuid(), 
                        success: (rnd.Next(10)) > 1,
                        durationInMilliseconds: 50 + rnd.Next(200));
                }

                Task.Delay(500, cancellationToken).Wait();
            }
        }

        private static void LogEvent(Guid correlationId, bool success, int durationInMilliseconds)
        {
            if (success)
            {
                Log.Info($"Operation completed successfully. {{ duration:{durationInMilliseconds}, correlationId:{correlationId:N} }}");
            }
            else
            {
                Log.Warn($"Operation failed. {{ duration:{durationInMilliseconds}, correlationId:{correlationId:N} }}");
            }
        }

        private static void LogException()
        {
            try
            {
                throw new Exception("Some random exception.");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
