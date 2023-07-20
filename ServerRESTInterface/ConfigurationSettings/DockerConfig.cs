namespace ServerRESTInterface.ConfigurationSettings
{
    public class DockerConfig
    {
        public Uri DockerURI { get; set; }
        public string? TemporaryFolderPath { get; set; }
        public int MaxImageUploadSize { get; set; }
    }
}
