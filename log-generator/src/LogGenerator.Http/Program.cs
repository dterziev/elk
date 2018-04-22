using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGenerator.EventHub
{
    class Program
    {
        private const string BaseUrl = "connection string";

        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var baseUrl = args[0];
            if (!long.TryParse(args[1], out var startSequenceNo)) startSequenceNo = 1;
            if (!long.TryParse(args[2], out var endSequenceNo)) endSequenceNo = 1000;
            if (!int.TryParse(args[3], out var interval)) interval = 10;
            if (!int.TryParse(args[4], out var batchSize)) batchSize = 10;

            Console.WriteLine($"Sequence [{startSequenceNo}:{endSequenceNo}] in {batchSize * interval}/s");

            await SendMessages(baseUrl, startSequenceNo, endSequenceNo, interval, batchSize);

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        private static async Task SendMessages(
            string baseUrl,
            long startSequenceNo,
            long endSequenceNo,
            int interval,
            int batchSize)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = 
                System.Net.DecompressionMethods.Deflate | 
                System.Net.DecompressionMethods.GZip;
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(baseUrl);
            

            while (startSequenceNo < endSequenceNo)
            {
                var payload = GenerateEvents(ref startSequenceNo, Math.Min(batchSize, (int)(endSequenceNo - startSequenceNo)));

                try
                {
                    var content = new ByteArrayContent(payload.Array, payload.Offset, payload.Count);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var result = await client.PostAsync("", content);
                    result.EnsureSuccessStatusCode();
                    Console.WriteLine($"{DateTime.Now} Sent {startSequenceNo}");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message} {exception}");
                }

                await Task.Delay(TimeSpan.FromSeconds(interval));
            }
        }

        private static ArraySegment<byte> GenerateEvents(ref long sequence, int eventCount)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(buffer, new UTF8Encoding(false), 4096, true))
                using (JsonTextWriter w = new JsonTextWriter(writer))
                {
                    w.WriteStartArray();

                    for (int i = 0; i < eventCount; i++)
                    {
                        var sequenceNo = Interlocked.Increment(ref sequence);
                        w.WriteStartObject();

                        w.WritePropertyName("@timestamp");
                        w.WriteValue(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'"));

                        w.WritePropertyName("logger");
                        w.WriteValue("Program.GridLogger");

                        w.WritePropertyName("level");
                        w.WriteValue("INFO");

                        w.WritePropertyName("hostname");
                        w.WriteValue(Environment.MachineName);

                        w.WritePropertyName("taskId");
                        w.WriteValue(1);

                        w.WritePropertyName("jobId");
                        w.WriteValue(3);

                        w.WritePropertyName("thread");
                        w.WriteValue("4");

                        w.WritePropertyName("message");
                        w.WriteValue($"log message sequence {sequenceNo}");

                        w.WritePropertyName("eventSequence");
                        w.WriteValue(sequenceNo);

                        w.WritePropertyName("metrics");
                        w.WriteStartObject();

                        w.WritePropertyName("metric1");
                        w.WriteValue(4.3);

                        w.WritePropertyName("metric2");
                        w.WriteValue(5.3);

                        w.WritePropertyName("metric3");
                        w.WriteValue(0.3 * sequenceNo);

                        w.WritePropertyName("seq");
                        w.WriteValue(sequenceNo);

                        w.WriteEndObject();


                        w.WriteEndObject();
                    }

                    w.WriteEndArray();
                }

                buffer.TryGetBuffer(out var result);
                return result;
            }
        }
    }
}
