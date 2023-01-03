using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Spotify;

public class SongNodeCreationStrategy : BaseSpotifyNodeCreationStrategy
{
    public SongNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }
    
    public override async Task CreateNode(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }
        
        await Client.Cypher
            .Merge("(s:Song {name: $name})")
            .OnCreate()
            .Set("s = $song")
            .WithParams(new
            {
                name = activity.SpotifyInfo.Track.Name,
                song = new SongEntity
                {
                    Name = activity.SpotifyInfo.Track.Name
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}