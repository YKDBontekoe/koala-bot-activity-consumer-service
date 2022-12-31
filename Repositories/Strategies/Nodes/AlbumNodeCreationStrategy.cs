using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class AlbumNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public AlbumNodeCreationStrategy(IBoltGraphClient client)
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
            .Merge("(a:Album {name: $name})")
            .OnCreate()
            .Set("a = $album")
            .WithParams(new
            {
                name = spotifyActivity.SpotifyInfo.Track.Album.Name,
                album = new AlbumEntity
                {
                    Name = spotifyActivity.SpotifyInfo.Track.Album.Name,
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}