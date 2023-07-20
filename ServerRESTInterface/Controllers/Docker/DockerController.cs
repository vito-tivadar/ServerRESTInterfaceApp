using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerRESTInterface.Controllers.Docker;

//[ServiceFilter]
[Route("api/[controller]")]
[ApiController]
public class DockerController : ControllerBase
{
    private DockerClient _client;

    public DockerController(DockerClient client)
    {
        _client = client;

    }

    // GET: api/<DockerController>
    [HttpGet("/info")]
    public async Task<SystemInfoResponse?>  Get()
    {
        try
        {
            SystemInfoResponse response = await _client.System.GetSystemInfoAsync();
            return response;

        }
        catch (Exception)
        {
            return null;
        }
    }

    [HttpGet("/status")]
    public async Task<bool> GetStatus()
    {
        try
        {
            //await _client.System.GetSystemInfoAsync();
            await _client.System.PingAsync();
            return true;

        }
        catch
        {
            return false;
        }
    }
}
