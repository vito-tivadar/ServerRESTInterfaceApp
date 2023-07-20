using Docker.DotNet.Models;

namespace ServerRESTInterface.Models.Docker
{
    public class SimplifiedVolumeModel
    {
        private VolumeResponse _volume { get; set; }

        public string name => _volume.Name;
        public DateTimeModel created => new DateTimeModel(_volume.CreatedAt);
        public string mountPoint => _volume.Mountpoint;
        public string driver => _volume.Driver;
        public string scope => _volume.Scope;

        public SimplifiedVolumeModel(VolumeResponse volume)
        {
            _volume = volume;
        }
    }
}
