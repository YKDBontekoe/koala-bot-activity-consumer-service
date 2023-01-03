using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Spotify;

public class ArtistToGenreRelationshipCreationStrategy : BaseSpotifyRelationshipCreationStrategy
{
    public ArtistToGenreRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(SpotifyActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        foreach (var artist in activity.SpotifyInfo.Track.Artists)
        foreach (var genre in artist.Genres)
            await Client.Cypher
                .Match("(a:Artist)", "(g:Genre)")
                .Where((ArtistEntity a) => a.Name == artist.Name)
                .AndWhere((GenreEntity g) => g.Name == genre)
                .Merge("(a)-[:HAS_GENRE]->(g)")
                .ExecuteWithoutResultsAsync();
    }
}