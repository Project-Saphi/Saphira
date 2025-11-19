namespace Saphira.Saphi.Api;

public class DefaultCacheDuration
{
    public static readonly TimeSpan Category = TimeSpan.FromDays(1);
    public static readonly TimeSpan Character = TimeSpan.FromDays(1);
    public static readonly TimeSpan Country = TimeSpan.FromDays(1);
    public static readonly TimeSpan CustomTrack = TimeSpan.FromDays(1);
    public static readonly TimeSpan Engine = TimeSpan.FromDays(1);
    public static readonly TimeSpan Leaderboard = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan Player = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan PlayerPB = TimeSpan.FromMinutes(10);
    public static readonly TimeSpan RecentSubmission = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan Players = TimeSpan.FromHours(1);
}
