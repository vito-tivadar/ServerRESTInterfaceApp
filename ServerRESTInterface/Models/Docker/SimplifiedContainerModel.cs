using Docker.DotNet.Models;

namespace ServerRESTInterface.Models.Docker;

public class SimplifiedContainerModel
{
    private ContainerListResponse _container;

    public string id => _container.ID;
    public string shortId => id.Substring(0, 12);
    private IList<string> names => ParseContainerNames(_container.Names);
    public string firstName => names.First();
    //public DateTimeModel createdOn => new DateTimeModel(_container.Created);
    public string createdOn => _container.Created.ToString("dd-MM-yyyy HH:mm:ss");
    public string state => _container.State;
    public string status => _container.Status;

    public SimplifiedContainerModel(ContainerListResponse container)
    {
        _container = container;
    }

    private IList<string> ParseContainerNames(IList<string> containerNames)
    {
        IList<string> parsedContainerNames = new List<string>();
        foreach (string name in containerNames)
        {
            parsedContainerNames.Add(name.Replace("/", string.Empty));
        }

        return parsedContainerNames;
    }
}
