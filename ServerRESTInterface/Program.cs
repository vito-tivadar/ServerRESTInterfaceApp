using Docker.DotNet;
using ServerRESTInterface.ConfigurationSettings;
using ServerRESTInterface.Filter;
using ServerRESTInterface.Models.Docker;



var builder = WebApplication.CreateBuilder(args);

DockerConfig dockerConfig = builder.Configuration.GetSection("DockerConfig").Get<DockerConfig>();

DockerClient client = new DockerClientConfiguration(dockerConfig.DockerURI)
     .CreateClient();


// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<DockerStatusActionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(client);
builder.Services.AddSingleton(new DockerConfigModel(dockerConfig));

builder.Services.AddTransient< DockerStatusActionFilter>();

var app = builder.Build();


#region TEST
/*
DirectoryInfo path1 = new DirectoryInfo(dockerConfig.ContainersPath);
bool path1ReadOnly = path1.Attributes.HasFlag(FileAttributes.ReadOnly);

DirectoryInfo path2 = new DirectoryInfo(dockerConfig.ImagesPath);
bool path2ReadOnly = path2.Attributes.HasFlag(FileAttributes.ReadOnly);

DirectoryInfo path3 = new DirectoryInfo(dockerConfig.VolumesPath);
bool path3ReadOnly = path3.Attributes.HasFlag(FileAttributes.ReadOnly);
*/

Console.WriteLine();
#endregion //TEST


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
