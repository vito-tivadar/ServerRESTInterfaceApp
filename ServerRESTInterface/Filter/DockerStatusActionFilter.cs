using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ServerRESTInterface.Filter
{
    public class DockerStatusActionFilter : IAsyncActionFilter
    {
        private DockerClient _client;

        public DockerStatusActionFilter(DockerClient client)
        {
            _client = client;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                //await _client.System.GetSystemInfoAsync();
                await _client.System.PingAsync();
                
                await next();
            }
            catch
            {
                ProblemDetails response = new ProblemDetails()
                {
                    Type = "Internal Server Error",
                    Title = "Internal Server Error",
                    Detail = "Docker engine is not running",
                    Status = (int)HttpStatusCode.InternalServerError,
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, typeof(ProblemDetails));
            }
        }
    }
}
