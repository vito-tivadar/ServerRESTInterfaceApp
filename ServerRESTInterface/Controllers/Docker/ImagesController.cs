using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;
using ServerRESTInterface.Models.Docker;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerRESTInterface.Controllers.Docker;

[Route("api/Docker/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private DockerClient _client;
    private DockerConfigModel _dockerConfig;
    const long _maxImageSizeBytes = (long)10e9; // 1GB //209715200;

    public ImagesController(DockerClient client, DockerConfigModel dockerConfig)
    {
        _client = client;
        _dockerConfig = dockerConfig;
    }

    [HttpGet]
    //public List<SimplifiedImageModel> Get()
    public IList<SimplifiedImageModel> GetTest()
    {
        IList<ImagesListResponse> returnedImages = GetImages().Result;
        return GetSimplifiedImages(returnedImages);
    }

    // GET: api/<ImagesController>
    [HttpGet]
    //public List<SimplifiedImageModel> Get()
    public IList<SimplifiedImageModel> Get()
    {
        IList<ImagesListResponse> returnedImages = GetImages().Result;
        return GetSimplifiedImages(returnedImages);
    }

    // GET api/<ImagesController>/5
    [HttpGet("{id}")]
    public async Task<ImageInspectResponse?> Get(string id)
    {
        if(ImageExists(id) == false)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return null;
        }
        ImageInspectResponse image = await _client.Images.InspectImageAsync(id);
        return image;
    }

    // POST api/<ImagesController>
    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = _maxImageSizeBytes)]
    [RequestSizeLimit(_maxImageSizeBytes)]
    public async void Post(IFormFile imageFile, ImageLoadParameters imageParameters)
    {
        int retries = 3;

        // save file to temp location
        string tempFolderPath = _dockerConfig.TemporaryFolderPath;
        string fileName = imageFile.FileName;
        string fileExtension = Path.GetExtension(fileName);

        if(fileExtension != ".tar")
        {
            Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            return;
        }

        string finalPath = Path.Combine(tempFolderPath, fileName);

        if (Directory.Exists(tempFolderPath))
        {
            for (int i = 1; i <= retries; i++)
            {
                try
                {
                    using (FileStream saveFileStream = System.IO.File.Create(finalPath))
                    {
                        imageFile.CopyTo(saveFileStream);
                    }                   

                    long savedFileSize = new FileInfo(finalPath).Length;
                    if (savedFileSize != imageFile.Length)
                    {
                        Response.StatusCode = StatusCodes.Status409Conflict;
                        throw new Exception("File sizes are different.");
                    }
                    else break;
                }
                catch
                {
                    if (i == retries)
                    {
                        Response.StatusCode = StatusCodes.Status409Conflict;
                        return;
                    }
                }
                //break;
            }
        }

        if (!System.IO.File.Exists(finalPath))
        {
            Response.StatusCode = StatusCodes.Status409Conflict;
            return;
        }

        // read file from stream
        for (int i = 1; i <= retries; i++)
        {
            Stream readFileStream = System.IO.File.OpenRead(finalPath);
            try
            {
                await _client.Images.LoadImageAsync(
                    imageParameters,
                    readFileStream,
                    new Progress<JSONMessage>()
                    );
            }
            catch
            {
                readFileStream.Dispose();
                if (i == retries)
                {
                    Response.StatusCode = StatusCodes.Status409Conflict;
                    break;
                }
            }
            readFileStream.Dispose();
            break;
        }
        System.IO.File.Delete(finalPath);
    }

    // DELETE api/<ImagesController>/5
    [HttpDelete("{id}")]
    public async Task<string>? Delete(string id)
    {
        try
        {
            IList<IDictionary<string, string>> result = await _client.Images.DeleteImageAsync(id, new ImageDeleteParameters());
            return $"Deleted image: { result[0]["Untagged"] }";
        }
        catch (Exception)
        {
            Response.StatusCode = StatusCodes.Status409Conflict;
            return $"Image ({id}) does not exist.";
        }
    }

    [HttpDelete("prune")]
    public async void PutPrune(string id, ImagesPruneParameters? pruneParameters = null)
    {
        await _client.Images.PruneImagesAsync(pruneParameters);
    }




    #region Private methods

    private async Task<IList<ImagesListResponse>> GetImages(bool returnAll = true)
    {
        Task<IList<ImagesListResponse>> images = _client.Images.ListImagesAsync(
            new ImagesListParameters()
            {
                All = returnAll,
            });

        return await images;
    }

    private List<SimplifiedImageModel> GetSimplifiedImages(IList<ImagesListResponse> images)
    {
        List<SimplifiedImageModel> simplifiedImages = new List<SimplifiedImageModel>();
        foreach (ImagesListResponse image in images)
        {
            simplifiedImages.Add(new SimplifiedImageModel(image));
        }
        return simplifiedImages;
    }

    private bool ImageExists(string imageID)
    {
        IList<ImagesListResponse> images = _client.Images.ListImagesAsync(new ImagesListParameters() { All = true }).Result;

        foreach (ImagesListResponse image in images)
        {
            if (image.ID.Split(':')[1] == imageID)
            { 
                return true;
            }
        }
        return false;
    }

    #endregion //Private methods
}
