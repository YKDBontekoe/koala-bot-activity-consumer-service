using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Spotify;

public class UserToSongRelationshipCreationStrategy : BaseSpotifyRelationshipCreationStrategy
{
    public UserToSongRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        await Client.Cypher
            .Match("(u:User)", "(s:Song)")
            .Where((UserEntity u) => u.Name == activity.User.Username)
            .AndWhere((SongEntity s) => s.Name == activity.SpotifyInfo.Track.Name)
            .Merge("(u)-[:IS_LISTENING]->(s)")
            .ExecuteWithoutResultsAsync();
    }
}