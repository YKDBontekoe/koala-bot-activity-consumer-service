namespace Koala.ActivityHandlerService.Models;

public class TrackAudioFeatures
{
    public double Danceability { get; set; }
    public double Energy { get; set; }
    public int Key { get; set; }
    public double Loudness { get; set; }
    public int Mode { get; set; }
    public double Speechiness { get; set; }
    public double Acousticness { get; set; }
    public double Instrumentalness { get; set; }
    public double Liveness { get; set; }
    public double Valence { get; set; }
    public double Tempo { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }
    public string Uri { get; set; }
    public string TrackHref { get; set; }
    public string AnalysisUrl { get; set; }
    public int DurationMs { get; set; }
    public int TimeSignature { get; set; }
}