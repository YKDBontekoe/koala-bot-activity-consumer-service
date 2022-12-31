using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class ArtistNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public ArtistNodeCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }
    
    public async Task CreateNode(Activity activity)
    {
        if (!activity.Type.Equals(MessageTypes.Listening))
        {
            return;
        }
        
        var spotifyActivity = (SpotifyActivity)activity;
        foreach (var artist in spotifyActivity.SpotifyInfo.Track.Artists)
        {
            await _client.Cypher
                .Merge("(a:Artist {name: $name})")
                .OnCreate()
                .Set("a = $artistName")
                .WithParams(new
                {
                    name = artist.Name,
                    artistName = new ArtistEntity
                    {
                        Name = artist.Name
                    }
                })
                .ExecuteWithoutResultsAsync();
        }
    }
}