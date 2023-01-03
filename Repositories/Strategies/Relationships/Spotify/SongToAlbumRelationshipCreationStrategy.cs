using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Spotify;

public class SongToAlbumRelationshipCreationStrategy : BaseSpotifyRelationshipCreationStrategy
{
    public SongToAlbumRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        await Client.Cypher
            .Match("(s:Song)", "(a:Album)")
            .Where((SongEntity s) => s.Name == activity.SpotifyInfo.Track.Name)
            .AndWhere((AlbumEntity a) => a.Name == activity.SpotifyInfo.Track.Album.Name)
            .Merge("(s)-[:IS_FROM_ALBUM]->(a)")
            .ExecuteWithoutResultsAsync();
    }
}