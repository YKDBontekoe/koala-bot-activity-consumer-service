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
        await CreateOrUpdateUserNode(activity);
        
        await CreateOrUpdateGuildNode(activity);
        
        if (!string.IsNullOrEmpty(activity.Name))
        {
            if (activity.SpotifyInfo != null)
            {
                await CreateOrUpdateSpotifyAlbumNode(activity);
                await CreateOrUpdateSpotifyArtistNodes(activity);
                await CreateOrUpdateSpotifyArtistGenresNodes(activity);
            }
            else
            {
                await CreateOrUpdateActivityNode(activity);
            }
            await CreateOrUpdateActivityNode(activity);
        }
    }

    private async Task CreateOrUpdateUserNode(Activity activity)
    {
        await _client.Cypher
            .Merge("(u:User {userName: $userName})")
            .OnCreate()
            .Set("u = $user")
            .WithParams(new
            {
                userName = activity.User.Username,
                user = new UserEntity()
                {
                    UserName = activity.User.Username
                }
            })
            .ExecuteWithoutResultsAsync();
    }
    
    private async Task CreateOrUpdateGuildNode(Activity activity)
    {
        if (activity.User.Guilds is null || !activity.User.Guilds.Any())
        {
            return;
        }
        
        foreach (var guild in activity.User.Guilds)
        {
            await _client.Cypher
                .Merge("(g:Guild {name: $name})")
                .OnCreate()
                .Set("g = $guild")
                .WithParams(new
                {
                    name = guild.Name,
                    guild = new GuildEntity()
                    {
                        Name = guild.Name
                    }
                })
                .ExecuteWithoutResultsAsync();
            
            await _client.Cypher
                .Match("(u:User)", "(g:Guild)")
                .Where((UserEntity u) => u.UserName == activity.User.Username)
                .AndWhere((GuildEntity g) => g.Name == guild.Name)
                .Merge("(u)-[:MEMBER_OF]->(g)")
                .ExecuteWithoutResultsAsync();
        }
    }

    private async Task CreateOrUpdateActivityNode(Activity activity)
    {
        if (activity.SpotifyInfo != null)
        {
            await _client.Cypher
                .Merge("(s:Song {name: $name})")
                .OnCreate()
                .Set("s = $song")
                .WithParams(new
                {
                    name = activity.SpotifyInfo?.Track.Name,
                    song = new SongEntity
                    {
                        Name = activity.SpotifyInfo?.Track.Name
                    }
                })
                .ExecuteWithoutResultsAsync();
            
            await _client.Cypher
                .Match("(u:User)", "(s:Song)")
                .Where((UserEntity u) => u.UserName == activity.User.Username)
                .AndWhere((SongEntity s) => s.Name == activity.SpotifyInfo.Track.Name)
                .Merge("(u)-[:IS_LISTENING]->(s)")
                .ExecuteWithoutResultsAsync();
        }
        else
        {
            if (!activity.Type.Equals("Status"))
            {
                await _client.Cypher
                    .Merge("(a:Activity {name: $name})")
                    .OnCreate()
                    .Set("a = $activity")
                    .WithParams(new
                    {
                        name = activity.Name,
                        activity = new ActivityEntity
                        {
                            Name = activity.Name,
                        }
                    })
                    .ExecuteWithoutResultsAsync();

                await _client.Cypher
                    .Match("(u:User)", "(a:Activity)")
                    .Where((UserEntity u) => u.UserName == activity.User.Username)
                    .AndWhere((ActivityEntity a) => a.Name == activity.Name)
                    .Merge("(u)-[:IS_PLAYING]->(a)")
                    .ExecuteWithoutResultsAsync();
            }
        }
    }
    private async Task CreateOrUpdateSpotifyAlbumNode(Activity activity)
    {
        await _client.Cypher
            .Merge("(a:Album {name: $name})")
            .OnCreate()
            .Set("a = $album")
            .WithParams(new
            {
                name = activity.SpotifyInfo.Track.Album.Name,
                album = new AlbumEntity
                {
                    Name = activity.SpotifyInfo.Track.Album.Name,
                }
            })
            .ExecuteWithoutResultsAsync();
        
        await _client.Cypher
            .Match("(s:Song)", "(a:Album)")
            .Where((SongEntity s) => s.Name == activity.SpotifyInfo.Track.Name)
            .AndWhere((AlbumEntity a) => a.Name == activity.SpotifyInfo.Track.Album.Name)
            .Merge("(s)-[:IS_FROM_ALBUM]->(a)")
            .ExecuteWithoutResultsAsync();
    }

    private async Task CreateOrUpdateSpotifyArtistNodes(Activity activity)
    {
        foreach (var artist in activity.SpotifyInfo.Track.Artists)
        {
            await _client.Cypher
                .Merge("(a:SpotifyArtist {name: $name})")
                .OnCreate()
                .Set("a = $spotifyArtistName")
                .WithParams(new
                {
                    name = artist.Name,
                    spotifyArtistName = new ArtistEntity
                    {
                        Name = artist.Name
                    }
                })
                .ExecuteWithoutResultsAsync();
            
            await _client.Cypher
                .Match("(sa:SpotifyArtist)", "(s:Song)")
                .Where((ActivityEntity s) => s.Name == activity.SpotifyInfo.Track.Name)
                .AndWhere((ArtistEntity sa) => artist.Name == sa.Name)
                .Merge("(s)-[:HAS_ARTIST]->(sa)")
                .ExecuteWithoutResultsAsync();
        }
    }
    
    private async Task CreateOrUpdateSpotifyArtistGenresNodes(Activity activity)
    {
        foreach (var artist in activity.SpotifyInfo.Track.Artists)
        {
            foreach (var genre in artist.Genres)
            {
                await _client.Cypher
                    .Merge("(g:Genre {name: $name})")
                    .OnCreate()
                    .Set("g = $genre")
                    .WithParams(new
                    {
                        name = genre,
                        genre = new GenreEntity
                        {
                            Name = genre
                        }
                    })
                    .ExecuteWithoutResultsAsync();
                
                await _client.Cypher
                    .Match("(sa:SpotifyArtist)", "(g:Genre)")
                    .Where((ArtistEntity sa) => sa.Name == artist.Name)
                    .AndWhere((GenreEntity g) => g.Name == genre)
                    .Merge("(sa)-[:HAS_GENRE]->(g)")
                    .ExecuteWithoutResultsAsync();
            }
        }
    }
}