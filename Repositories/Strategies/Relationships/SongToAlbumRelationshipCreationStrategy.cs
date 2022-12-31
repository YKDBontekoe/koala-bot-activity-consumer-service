using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class SongToAlbumRelationshipCreationStrategy : IRelationshipCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public SongToAlbumRelationshipCreationStrategy(IBoltGraphClient client)
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
        await _client.Cypher
            .Match("(s:Song)", "(a:Album)")
            .Where((SongEntity s) => s.Name == spotifyActivity.SpotifyInfo.Track.Name)
            .AndWhere((AlbumEntity a) => a.Name == spotifyActivity.SpotifyInfo.Track.Album.Name)
            .Merge("(s)-[:IS_FROM_ALBUM]->(a)")
            .ExecuteWithoutResultsAsync();
    }
}