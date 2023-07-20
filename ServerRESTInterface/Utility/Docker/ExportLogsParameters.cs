using Docker.DotNet.Models;

namespace ServerRESTInterface.Utility.Docker
{
    public class ExportLogsParameters
    {
        public bool? ShowTimestamps { get; set; }
        public bool? Follow { get; set; }
        public int? LastXMinutes { get; set; }
        public int? LastXLines { get; set; }
        public bool? ShowStdErr { get; set; }
        public bool? ShowStdOut { get; set; }

        public ExportLogsParameters() { }


        public ContainerLogsParameters GetLogParameters()
        {
            ContainerLogsParameters clp = new ContainerLogsParameters() { };

            clp.ShowStderr = ShowStdErr == null ? true : ShowStdErr.Value;
            clp.ShowStdout = ShowStdOut == null ? true : ShowStdOut.Value;

            if (ShowTimestamps.HasValue) clp.Timestamps = ShowTimestamps.Value;
            if (Follow.HasValue && Follow.Value == false) clp.Follow = Follow.Value;
            if (LastXLines.HasValue && LastXLines.Value > 0) clp.Tail = LastXLines.Value.ToString();
            if (LastXMinutes.HasValue && LastXMinutes > 0)
            {
                DateTime time = DateTime.Now.AddMinutes(-(int)LastXMinutes);
                clp.Since = LastXMinutes.ToString();
            }

            return clp;
        }
    }


}
