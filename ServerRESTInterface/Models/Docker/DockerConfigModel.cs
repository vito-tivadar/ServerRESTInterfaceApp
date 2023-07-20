using ServerRESTInterface.ConfigurationSettings;

namespace ServerRESTInterface.Models.Docker
{
    public class DockerConfigModel
    {
        private DockerConfig _dockerConfig { get; set; }

        public Uri DockerUri => _dockerConfig.DockerURI;
        public int MaxImageUploadSize => _dockerConfig.MaxImageUploadSize;
        public string TemporaryFolderPath { get; set; }


        public DockerConfigModel(DockerConfig dockerConfig)
        {
            if (dockerConfig == null) throw new ArgumentNullException(nameof(dockerConfig));
            _dockerConfig = dockerConfig;

            if (dockerConfig.TemporaryFolderPath == null || dockerConfig.TemporaryFolderPath == "")
            {
                string tempFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "temp");
                if (!Directory.Exists(tempFolderPath)) Directory.CreateDirectory(tempFolderPath);
                TemporaryFolderPath = tempFolderPath;
            }
            else TemporaryFolderPath = dockerConfig.TemporaryFolderPath;


        }
    }
}
