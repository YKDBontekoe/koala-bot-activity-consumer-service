using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Koala.ActivityConsumerService.Repositories.Strategies.Nodes;
using Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Game;
using Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Spotify;
using Koala.ActivityConsumerService.Repositories.Strategies.Relationships;
using Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Game;
using Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Spotify;
using Neo4jClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Koala.ActivityConsumerService.Repositories
{
    public class ActivityNeoRepository : IActivityNeoRepository
    {
        private readonly IBoltGraphClient _client;

        public ActivityNeoRepository(IBoltGraphClient client)
        {
            _client = client;
        }

        public async Task GenerateRelationships(Activity activity)
        {
            var spotifyNodes = GetSpotifyNodes();
            var gameNodes = GetGameNodes();
            var otherNodes = GetOtherNodes();
            
            var spotifyRelationships = GetSpotifyRelationships();
            var gameRelationships = GetGameRelationships();
            var otherRelationships = GetOtherRelationships();
            
            switch (activity)
            {
                case SpotifyActivity spotifyActivity:
                {
                    foreach (var spotifyNode in spotifyNodes)
                    {
                        await spotifyNode.CreateNode(spotifyActivity);
                    }
                
                    foreach (var spotifyRelationship in spotifyRelationships)
                    {
                        await spotifyRelationship.CreateRelationship(spotifyActivity);
                    }

                    break;
                }
                case GameActivity gameActivity:
                {
                    foreach (var gameNode in gameNodes)
                    {
                        await gameNode.CreateNode(gameActivity);
                    }

                    foreach (var gameRelations in gameRelationships)
                    {
                        await gameRelations.CreateRelationship(gameActivity);
                    }

                    break;
                }
            }

            foreach (var otherNode in otherNodes)
            {
                await otherNode.CreateNode(activity);
            }
            
            foreach (var otherRelationship in otherRelationships)
            {
                await otherRelationship.CreateRelationship(activity);
            }
        }
        
        private IEnumerable<BaseSpotifyNodeCreationStrategy> GetSpotifyNodes() =>
            new List<BaseSpotifyNodeCreationStrategy>
            {
                new AlbumNodeCreationStrategy(_client),
                new ArtistNodeCreationStrategy(_client),
                new SongNodeCreationStrategy(_client),
                new GenresNodeCreationStrategy(_client),
            };
        
        private IEnumerable<BaseGameNodeCreationStrategy> GetGameNodes() =>
            new List<BaseGameNodeCreationStrategy>
            {
                new DeveloperNodeCreationStrategy(_client), 
                new GameNodeCreationStrategy(_client),
                new FeatureTagNodeCreationStrategy(_client),
                new ReviewScoreCreationStrategy(_client),
            };

        private IEnumerable<INodeCreationStrategy<Activity>> GetOtherNodes() =>
            new List<INodeCreationStrategy<Activity>>
            {
                new UserNodeCreationStrategy(_client),
                new GuildNodeCreationStrategy(_client),
            };
        
        
        private IEnumerable<BaseSpotifyRelationshipCreationStrategy> GetSpotifyRelationships() =>
            new List<BaseSpotifyRelationshipCreationStrategy>
            {
                new ArtistToGenreRelationshipCreationStrategy(_client),
                new ArtistToSongRelationshipCreationStrategy(_client),
                new SongToAlbumRelationshipCreationStrategy(_client),
                new UserToSongRelationshipCreationStrategy(_client)
            };
        
        private IEnumerable<BaseGameRelationshipCreationStrategy> GetGameRelationships() =>
            new List<BaseGameRelationshipCreationStrategy>
            {
                new GameToDeveloperRelationshipCreationStrategy(_client),
                new GameToDeveloperRelationshipCreationStrategy(_client),
                new GameToFeatureTagRelationshipCreationStrategy(_client)
            };
        
        private IEnumerable<IRelationshipCreationStrategy<Activity>> GetOtherRelationships() =>
            new List<IRelationshipCreationStrategy<Activity>>
            {
                new UserToGuildRelationshipCreationStrategy(_client)
            };
    }
}

