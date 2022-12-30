using Koala.ActivityConsumerService.Models;

namespace Koala.ActivityHandlerService.Models;

public class Track
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Duration { get; set; }
    public bool Explicit { get; set; }
    public int Popularity { get; set; }
    public string PreviewUrl { get; set; }
    public string Uri { get; set; }
    public List<Artist> Artists { get; set; }
    public Album Album { get; set; }
}