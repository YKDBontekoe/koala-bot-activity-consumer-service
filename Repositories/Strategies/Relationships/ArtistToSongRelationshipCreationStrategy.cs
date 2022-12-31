using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class ArtistToSongRelationshipCreationStrategy : IRelationshipCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public ArtistToSongRelationshipCreationStrategy(IBoltGraphClient client)
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
            await _client.Cypher
                .Match("(a:Artist)", "(s:Song)")
                .Where((GameEntity s) => s.Name == spotifyActivity.SpotifyInfo.Track.Name)
                .AndWhere((ArtistEntity a) => artist.Name == a.Name)
                .Merge("(s)-[:HAS_ARTIST]->(a)")
                .ExecuteWithoutResultsAsync();
        }
    }
}