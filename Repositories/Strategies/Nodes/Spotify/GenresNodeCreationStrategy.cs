using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Spotify;

public class GenresNodeCreationStrategy : BaseSpotifyNodeCreationStrategy
{
    public GenresNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateNode(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        foreach (var genre in activity.SpotifyInfo.Track.Artists.SelectMany(artist => artist.Genres))
            await Client.Cypher
                .Merge("(g:Genre {name: $name})")
                .OnCreate()
                .Set("g = $genre")
                .WithParams(new
                {
                    name = genre,
                    genre = new GenreEntity
                    {
                        Name = genre
                    }
                })
                .ExecuteWithoutResultsAsync();
    }
}