using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Spotify;

public class ArtistNodeCreationStrategy : BaseSpotifyNodeCreationStrategy
{
    public ArtistNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }
    
    public override async Task CreateNode(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }
        
        foreach (var artist in activity.SpotifyInfo.Track.Artists)
        {
            await Client.Cypher
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