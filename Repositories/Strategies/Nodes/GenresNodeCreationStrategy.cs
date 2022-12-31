using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class GenresNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public GenresNodeCreationStrategy(IBoltGraphClient client)
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
        foreach (var genre in spotifyActivity.SpotifyInfo.Track.Artists.SelectMany(artist => artist.Genres))
        {
            await _client.Cypher
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
}