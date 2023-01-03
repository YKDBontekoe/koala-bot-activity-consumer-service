namespace Koala.ActivityConsumerService.Options;

public class Neo4JDbOptions
{
    public const string Neo4jDb = "Neo4jDb";

    public string Uri { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}