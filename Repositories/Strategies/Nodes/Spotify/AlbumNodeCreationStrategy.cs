using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Spotify;

public class AlbumNodeCreationStrategy : BaseSpotifyNodeCreationStrategy
{
    public AlbumNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateNode(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }

        await Client.Cypher
            .Merge("(a:Album {name: $name})")
            .OnCreate()
            .Set("a = $album")
            .WithParams(new
            {
                name = activity.SpotifyInfo.Track.Album.Name,
                album = new AlbumEntity
                {
                    Name = activity.SpotifyInfo.Track.Album.Name,
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}