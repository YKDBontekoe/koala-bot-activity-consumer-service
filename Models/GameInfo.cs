using Koala.ActivityPublisherService.Models;

namespace Koala.ActivityConsumerService.Models;

public class GameInfo
{
    public GameTimeStamps Timestamps { get; set; }
    public ulong ApplicationId { get; set; }
    public GameParty Party { get; set; }
    public string[] Developers { get; set; }
    public string[] Tags { get; set; }
    public string Price { get; set; }
    public string ReleaseDate { get; set; }
    public string RecentReviewScore { get; set; }
    public string AllTimeReviewScore { get; set; }
}