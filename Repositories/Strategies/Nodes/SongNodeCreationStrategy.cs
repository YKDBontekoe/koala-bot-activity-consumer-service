using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class SongNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public SongNodeCreationStrategy(IBoltGraphClient client)
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
        await _client.Cypher
            .Merge("(s:Song {name: $name})")
            .OnCreate()
            .Set("s = $song")
            .WithParams(new
            {
                name = activity.Name,
                song = new SongEntity
                {
                    Name = spotifyActivity.SpotifyInfo.Track.Name
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}