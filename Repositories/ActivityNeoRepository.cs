using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Interfaces;
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
        await _client.Cypher
            .Merge("(a:Activity {name: $name})")
            .OnCreate()
            .Set("a = $activity")
            .WithParams(new
            {
                name = activity.Name,
                activity = new
                {
                    name = activity.Name,
                }
            })
            .ExecuteWithoutResultsAsync();
        
        await _client.Cypher
            .Merge("(u:User {userId: $userId})")
            .OnCreate()
            .Set("u = $user")
            .WithParams(new
            {
                userId = activity.User.Id.ToString(),
                user = new
                {
                    userId = activity.User.Id.ToString(),
                    username = activity.User.Username
                }
            })
            .ExecuteWithoutResultsAsync();
        
        // Create or update the SpotifyInfo node
        if (activity.SpotifyInfo != null)
        {
            await _client.Cypher
                .Merge("(si:Song {track: $track})")
                .OnCreate()
                .Set("si = $trackInfo")
                .WithParams(new
                {
                    track = activity.SpotifyInfo.Track,
                    trackInfo = new
                    {
                        track = activity.SpotifyInfo.Track,
                        durationInSeconds = activity.SpotifyInfo.DurationInSeconds
                    }
                })
                .ExecuteWithoutResultsAsync();
            
            await _client.Cypher
                .Merge("(sa:SpotifyAlbum {name: $name})")
                .OnCreate()
                .Set("sa = $spotifyAlbum")
                .WithParams(new
                {
                    name = activity.SpotifyInfo.Album,
                    spotifyAlbum = new
                    {
                        name = activity.SpotifyInfo.Album,
                    }
                })
                .ExecuteWithoutResultsAsync();
            
            activity.SpotifyInfo.Artists.Select(async artist => await _client.Cypher
                .Merge("(a:SpotifyArtist {name: $name})")
                .OnCreate()
                .Set("a = $spotifyArtistName")
                .WithParams(new
                {
                    name = artist,
                    spotifyArtistName = new
                    {
                        name = artist
                    }
                })
                .ExecuteWithoutResultsAsync());
        }
        
        // Create or update the Guild nodes
        if (activity.User.Guilds != null)
        {
            foreach (var guild in activity.User.Guilds)
            {
                await _client.Cypher
                    .Merge("(g:Guild {id: $id})")
                    .OnCreate()
                    .Set("g = $guild")
                    .WithParams(new
                    {
                        id = guild.Id.ToString(),
                        guild = new
                        {
                            id = guild.Id.ToString(),
                            name = guild.Name
                        }
                    })
                    .ExecuteWithoutResultsAsync();
            }
        }

        // Create or update the relationships between the nodes
        await _client.Cypher
            .Match("(a:Activity)", "(u:User)", "(si:SpotifyInfo)", "(sa:SpotifyAlbum)", "(spa:SpotifyArtist)", "(g:Guild)")
            .Where("a.name = $aName")
            .WithParam("aName", activity.Name)
            .AndWhere("u.userId = $userId")
            .WithParam("userId", activity.User.Id.ToString())
            // .AndWhereIf(activity.SpotifyInfo is not null ,"si.track = $track")
            // .WithParam("track", activity.SpotifyInfo?.Track)
            // .AndWhereIf(activity.SpotifyInfo is not null,"sa.name = $saName")
            // .WithParam("saName", activity.SpotifyInfo?.Album)
            // .AndWhere((NeoEntity spa) => activity.SpotifyInfo != null && activity.SpotifyInfo.Artists.Contains(spa.Name))
            // .AndWhere((Guild g) => activity.User.Guilds != null && activity.User.Guilds.Select(x => x.Id).Contains(g.Id))
            .Merge("(a)-[:PERFORMED_BY]->(u)")
            .Merge("(a)-[:LISTENING_TO]->(si)")
            .Merge("(si)-[:FROM_ALBUM]->(sa)")
            .Merge("(si)-[:BY_ARTIST]->(spa)")
            .Merge("(u)-[:MEMBER_OF]->(g)")
            .ExecuteWithoutResultsAsync();
    }
}