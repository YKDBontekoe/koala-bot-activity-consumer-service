using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class UserToSongRelationshipCreationStrategy : IRelationshipCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public UserToSongRelationshipCreationStrategy(IBoltGraphClient client)
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
            .Match("(u:User)", "(s:Song)")
            .Where((UserEntity u) => u.Id == activity.User.Id.ToString())
            .AndWhere((SongEntity s) => s.Name == spotifyActivity.SpotifyInfo.Track.Name)
            .Merge("(u)-[:IS_LISTENING]->(s)")
            .ExecuteWithoutResultsAsync();
    }
}