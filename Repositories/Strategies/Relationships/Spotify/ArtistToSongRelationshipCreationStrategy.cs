using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Spotify;

public class ArtistToSongRelationshipCreationStrategy : BaseSpotifyRelationshipCreationStrategy
{
    public ArtistToSongRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }
    
    public override async Task CreateRelationship(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }
        
        foreach (var artist in activity.SpotifyInfo.Track.Artists)
        {
            await Client.Cypher
                .Match("(a:Artist)", "(s:Song)")
                .Where((GameEntity s) => s.Name == activity.SpotifyInfo.Track.Name)
                .AndWhere((ArtistEntity a) => artist.Name == a.Name)
                .Merge("(s)-[:HAS_ARTIST]->(a)")
                .ExecuteWithoutResultsAsync();
        }
    }
}