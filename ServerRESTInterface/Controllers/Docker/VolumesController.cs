using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using ServerRESTInterface.Models.Docker;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerRESTInterface.Controllers.Docker
{
    [Route("api/Docker/[controller]")]
    [ApiController]
    public class VolumesController : ControllerBase
    {
        DockerClient _client;

        public VolumesController(DockerClient client)
        {
            _client = client;
        }

        // GET: api/<VolumesController>
        [HttpGet]
        public IEnumerable<SimplifiedVolumeModel> Get()
        {
            return GetSimplifiedVolumes();
        }

        // GET api/<VolumesController>/5
        [HttpGet("{name}")]
        public VolumeResponse? Get(string name)
        {
            foreach (VolumeResponse volume in GetVolumes().Result)
            {
                if(volume.Name == name) return volume;
            }
            return null;
        }

        // POST api/<VolumesController>
        [HttpPost]
        public void Post()
        {

        }

        // PUT api/<VolumesController>/5
        [HttpPut("{name}")]
        public void Put(string name, [FromBody] string value)
        {
            
        }

        // DELETE api/<VolumesController>/5
        [HttpDelete("{name}")]
        public async void Delete(string name, bool forceRemove = false)
        {
            await _client.Volumes.RemoveAsync(name, forceRemove);
        }



        #region private methods

        private async Task<IList<VolumeResponse>> GetVolumes()
        {
            VolumesListResponse volumesList = await _client.Volumes.ListAsync();
            return volumesList.Volumes;
        }
        
        private IList<SimplifiedVolumeModel> GetSimplifiedVolumes()
        {
            List<SimplifiedVolumeModel> volumes = new List<SimplifiedVolumeModel>();

            foreach (var volume in GetVolumes().Result)
            {
                volumes.Add(new SimplifiedVolumeModel(volume));
            }
            return volumes;
        }

        #endregion //private methods
    }
}
