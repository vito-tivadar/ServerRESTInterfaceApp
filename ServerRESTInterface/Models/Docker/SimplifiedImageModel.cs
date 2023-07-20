using Docker.DotNet.Models;

namespace ServerRESTInterface.Models.Docker;

public class SimplifiedImageModel
{
    private ImagesListResponse _imageResponse { get; set; }


    public string ID => _imageResponse.ID.Replace("sha256:", "");
    public IList<IDictionary<string, string>> repoTags => ConvertRepoTags(_imageResponse.RepoTags);
    public DateTimeModel created => new DateTimeModel(_imageResponse.Created);
    public long sizeInBytes => _imageResponse.Size;


    public SimplifiedImageModel(ImagesListResponse imageResponse)
    {
        _imageResponse = imageResponse;
    }

    private IList<IDictionary<string, string>> ConvertRepoTags(IList<string> repoTags)
    {
        IList<IDictionary<string, string>> newRepoTags = new List<IDictionary<string, string>>();

        if (repoTags.Count == 0) return newRepoTags;

        foreach (var repoTag in repoTags)
        {
            string[]? parsedRepoTag = repoTag.Split(':');

            IDictionary<string, string> repo = new Dictionary<string, string>();
            repo.Add("name", parsedRepoTag[0]);
            repo.Add("tag", parsedRepoTag[1]);

            newRepoTags.Add(repo);
        }

        return newRepoTags;
    }
}
