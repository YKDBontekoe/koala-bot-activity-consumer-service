using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class ArtistToGenreRelationshipCreationStrategy : IRelationshipCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public ArtistToGenreRelationshipCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateRelationship(Activity activity)
    {
        if (!activity.Type.Equals(MessageTypes.Listening))
        {
            return;
        }
        
        var spotifyActivity = (SpotifyActivity)activity;
        foreach (var artist in spotifyActivity.SpotifyInfo.Track.Artists)
        {
            foreach (var genre in artist.Genres)
            {
                await _client.Cypher
                    .Match("(a:Artist)", "(g:Genre)")
                    .Where((ArtistEntity a) => a.Name == artist.Name)
                    .AndWhere((GenreEntity g) => g.Name == genre)
                    .Merge("(a)-[:HAS_GENRE]->(g)")
                    .ExecuteWithoutResultsAsync();
            }
        }
    }
}