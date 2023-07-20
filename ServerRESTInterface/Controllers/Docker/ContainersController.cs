using Microsoft.AspNetCore.Mvc;
using Docker.DotNet.Models;
using Docker.DotNet;
using ServerRESTInterface.Models.Docker;
using ServerRESTInterface.Utility.Docker;
using ServerRESTInterface.Filter;
using Microsoft.AspNetCore.Mvc.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerRESTInterface.Controllers.Docker;

[Route("api/Docker/[controller]")]
[ApiController]
[ServiceFilter(typeof(DockerStatusActionFilter))]
public class ContainersController : ControllerBase
{
    DockerClient _client;

    public ContainersController(DockerClient client)
    {
        _client = client;
    }

    [HttpGet]
    public List<SimplifiedContainerModel> Get(int? limit)
    {
        IList<ContainerListResponse> returnedContainers;
        if (limit.HasValue) returnedContainers = GetContainers(false, (int)limit).Result;
        else returnedContainers = GetContainers().Result;
        return GetSimplifiedContainers(returnedContainers);
    }

    [HttpGet("{id}")]
    public async Task<ContainerInspectResponse?> Get(string id)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return null;
        }

        ContainerInspectResponse container = await _client.Containers.InspectContainerAsync(id);
        return container;
    }

    [HttpPost]
    public async Task<CreateContainerResponse?> PostContainer([FromBody] CreateContainerParameters createParameters)
    {
        CreateContainerResponse response = await _client.Containers.CreateContainerAsync(createParameters);
        return response;
    }

    [HttpPut("{id}")]
    public async void PutUpdateAllParameters(string id, [FromBody] ContainerUpdateParameters updateParameters)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await _client.Containers.UpdateContainerAsync(id, updateParameters);
    }

    [HttpPut("{id}/rename")]
    public async void PutRename(string id, string newName)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await _client.Containers.RenameContainerAsync(id, new ContainerRenameParameters() { NewName = newName }, new CancellationToken());

    }
    
    [HttpPut("{id}/resize")]
    public async void PutResize(string id, ContainerResizeParameters resizeParameters)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await _client.Containers.ResizeContainerTtyAsync(id, resizeParameters);
    }

    [HttpPut("{id}/export")]
    public void PutExport(string id)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
    }
    
    [HttpPut("{id}/logs")]
    public async Task<string?> PutExportLogs(string id, ExportLogsParameters exportParameters)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Container with id: {id} not found.";
        }

        try
        {
            using MultiplexedStream stream = await _client.Containers.GetContainerLogsAsync(id, true, exportParameters.GetLogParameters());
            (string, string) logs = await stream.ReadOutputToEndAsync(new CancellationToken());


            return logs.Item1;
        }
        catch (Exception e)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return e.Message; 
        }
    }

    [HttpPut("{id}/start")]
    public async Task<string?> PutStartContainer(string id, ContainerStartParameters? startParameters)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Container with id: {id} not found.";
        }

        bool output;
        if(startParameters == null) output = await _client.Containers.StartContainerAsync(id, new ContainerStartParameters());
        else output = await _client.Containers.StartContainerAsync(id, startParameters);

        return output.ToString();
        
    }

    [HttpPut("{id}/stop")]
    public async Task<string?> PutStopContainer(string id, ContainerStopParameters? stopParameters)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Container with id: {id} not found.";
        }

        bool output;
        if (stopParameters == null) output = await _client.Containers.StopContainerAsync(id, new ContainerStopParameters());
        else output = await _client.Containers.StopContainerAsync(id, stopParameters);

        return output.ToString();


    }

    [HttpPut("{id}/restart")]
    public async Task<string?> PutRestartContainer(string id, ContainerRestartParameters? restartParameters)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Container with id: {id} not found.";
        }

        if (restartParameters == null) await _client.Containers.RestartContainerAsync(id, new ContainerRestartParameters());
        else await _client.Containers.RestartContainerAsync(id, restartParameters);

        return "Container (" + id + ") restarted";


    }

    [HttpPut("{id}/pause")]
    public async Task<string?> PutPauseContainer(string id)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Container with id: {id} not found.";
        }

        await _client.Containers.PauseContainerAsync(id);

        return "Container (" + id + ") paused";
    }

    [HttpPut("{id}/resume")]
    public async Task<string?> PutResumeContainer(string id)
    {
        if (ContainerExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Container with id: {id} not found.";
        }

        await _client.Containers.UnpauseContainerAsync(id);

        return "Container (" + id + ") resumed";
    }


    [HttpDelete]
    public async void Delete(string containerID, ContainerRemoveParameters removeParameters)
    {
        await _client.Containers.RemoveContainerAsync(containerID, removeParameters);
    }


    [HttpDelete("prune")]
    public async void PutPrune(string id, ContainersPruneParameters? pruneParameters = null)
    {        
        await _client.Containers.PruneContainersAsync(pruneParameters);
    }


    #region Private methods

    private async Task<IList<ContainerListResponse>> GetContainers(bool getAll = true, int limit = 10)
    {
        ContainersListParameters parameters;
        if(getAll) parameters = new ContainersListParameters() { All = true };
        else parameters = new ContainersListParameters() { Limit = limit };

        Task<IList<ContainerListResponse>> containers = _client.Containers.ListContainersAsync(parameters);

        return await containers;
    }

    private List<SimplifiedContainerModel> GetSimplifiedContainers(IList<ContainerListResponse> containers)
    {
        List<SimplifiedContainerModel> simplifiedContainers = new List<SimplifiedContainerModel>();
        foreach (ContainerListResponse container in containers)
        {
            simplifiedContainers.Add(new SimplifiedContainerModel(container));
        }
        return simplifiedContainers;
    }

    private bool ContainerExists(string id)
    {
        IList<ContainerListResponse> containers = GetContainers().Result;
        foreach (ContainerListResponse container in containers)
        {
            if (container.ID == id) return true;
        }
        return false;
    } 

    #endregion //Private methods
}
