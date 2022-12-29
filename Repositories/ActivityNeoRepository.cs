﻿using Koala.ActivityConsumerService.Models;
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
            await CreateOrUpdateActivityTypeNode(activity);

            if (activity.SpotifyInfo != null)
            {
                await CreateOrUpdateSpotifyAlbumNode(activity);
                await CreateOrUpdateSpotifyArtistNodes(activity);
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

    private async Task CreateOrUpdateActivityTypeNode(Activity activity)
    {
        await _client.Cypher
            .Merge("(a:ActivityType {type: $type})")
            .OnCreate()
            .Set("a = $activity")
            .WithParams(new
            {
                type = activity.Type,
                activity = new ActivityTypeEntity()
                {
                    Type = activity.Type,
                }
            })
            .ExecuteWithoutResultsAsync();
    }

    private async Task CreateOrUpdateActivityNode(Activity activity)
    {
        if (activity.SpotifyInfo != null)
        {
            await _client.Cypher
                .Merge("(a:Activity {name: $name})")
                .OnCreate()
                .Set("a = $activity")
                .WithParams(new
                {
                    name = activity.SpotifyInfo?.Track,
                    activity = new ActivityEntity
                    {
                        Name = activity.SpotifyInfo?.Track
                    }
                })
                .ExecuteWithoutResultsAsync();
            
            await _client.Cypher
                .Match("(u:User)", "(a:Activity)", "(at:ActivityType)")
                .Where((UserEntity u) => u.UserName == activity.User.Username)
                .AndWhere((TrackEntity a) => a.Name == activity.SpotifyInfo.Track)
                .AndWhere((ActivityTypeEntity at) => at.Type == activity.Type)
                .Merge("(u)-[:IS_LISTENING]->(a)")
                .Merge("(a)-[:IS_TYPE]->(at)")
                .ExecuteWithoutResultsAsync();
        }
        else
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
                .Match("(u:User)", "(a:Activity)", "(at:ActivityType)")
                .Where((UserEntity u) => u.UserName == activity.User.Username)
                .AndWhere((ActivityEntity a) => a.Name == activity.Name)
                .AndWhere((ActivityTypeEntity at) => at.Type == activity.Type)
                .Merge("(u)-[:IS_PLAYING]->(a)")
                .Merge("(a)-[:IS_TYPE]->(at)")
                .ExecuteWithoutResultsAsync();
        }
    }
    private async Task CreateOrUpdateSpotifyAlbumNode(Activity activity)
    {
        await _client.Cypher
            .Merge("(sa:SpotifyAlbum {name: $name})")
            .OnCreate()
            .Set("sa = $spotifyAlbum")
            .WithParams(new
            {
                name = activity.SpotifyInfo.Album,
                spotifyAlbum = new AlbumEntity
                {
                    Name = activity.SpotifyInfo.Album,
                }
            })
            .ExecuteWithoutResultsAsync();
        
        await _client.Cypher
            .Match("(a:Activity)", "(sa:SpotifyAlbum)")
            .Where((ActivityEntity a) => a.Name == activity.SpotifyInfo.Track)
            .AndWhere((AlbumEntity sa) => sa.Name == activity.SpotifyInfo.Album)
            .Merge("(a)-[:IS_FROM_ALBUM]->(sa)")
            .ExecuteWithoutResultsAsync();
    }

    private async Task CreateOrUpdateSpotifyArtistNodes(Activity activity)
    {
        foreach (var artist in activity.SpotifyInfo.Artists)
        {
            await _client.Cypher
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
                .ExecuteWithoutResultsAsync();
            
            await _client.Cypher
                .Match("(sa:SpotifyArtist)", "(a:Activity)")
                .Where((ActivityEntity a) => a.Name == activity.SpotifyInfo.Track)
                .AndWhere((ArtistEntity sa) => artist == sa.Name)
                .Merge("(a)-[:HAS_ARTIST]->(sa)")
                .ExecuteWithoutResultsAsync();
        }
    }
}