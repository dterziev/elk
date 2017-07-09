namespace log4net.Layout {
    public class JsonLayout : log4net.Layout.LayoutSkeleton
    {
        public override void ActivateOptions()
        {
            IgnoresException = false;
        }

        public override void Format(System.IO.TextWriter writer, Core.LoggingEvent loggingEvent)
        {
            loggingEvent.Fix = Core.FixFlags.All;

            writer.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(loggingEvent));
        }
    }
}