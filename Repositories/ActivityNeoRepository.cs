using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Koala.ActivityConsumerService.Repositories.Strategies.Nodes;
using Koala.ActivityConsumerService.Repositories.Strategies.Relationships;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories;

public class ActivityNeoRepository : IActivityNeoRepository
{
    private readonly IBoltGraphClient _client;

    public ActivityNeoRepository(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task GenerateRelationships(Activity activity)
    {
        var nodes = GetNodes();
        var relationships = GetRelationships();
        
        await Task.WhenAll(nodes.Select(x => x.CreateNode(activity)));
        await Task.WhenAll(relationships.Select(x => x.CreateRelationship(activity)));
    }
    
    private IEnumerable<INodeCreationStrategy> GetNodes() =>
        new List<INodeCreationStrategy>
        {
            new UserNodeCreationStrategy(_client), 
            new GameNodeCreationStrategy(_client),
            new AlbumNodeCreationStrategy(_client),
            new ArtistNodeCreationStrategy(_client),
            new SongNodeCreationStrategy(_client),
            new GuildNodeCreationStrategy(_client),
            new GenresNodeCreationStrategy(_client),
        };

    private IEnumerable<IRelationshipCreationStrategy> GetRelationships() =>
        new List<IRelationshipCreationStrategy>
        {
            new ArtistToGenreRelationshipCreationStrategy(_client),
            new ArtistToSongRelationshipCreationStrategy(_client),
            new SongToAlbumRelationshipCreationStrategy(_client),
            new UserToGameRelationshipCreationStrategy(_client),
            new UserToGuildRelationshipCreationStrategy(_client),
            new UserToSongRelationshipCreationStrategy(_client)
        };
}