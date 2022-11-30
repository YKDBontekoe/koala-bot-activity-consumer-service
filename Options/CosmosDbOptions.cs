namespace Koala.ActivityConsumerService.Options;

public class CosmosDbOptions
{
    public const string CosmosDb = "CosmosDb";
    
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}