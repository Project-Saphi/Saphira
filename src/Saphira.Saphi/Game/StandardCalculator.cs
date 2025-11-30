using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Game;

public class StandardCalculator
{
    public CustomTrackStandard? CalculateStandard(CustomTrack customTrack, string category, string time)
    {
        CustomTrackStandard? customTrackStandard = null;

        if (customTrack.Standards == null || customTrack.Standards.Count == 0)
        {
            return null;
        }

        var standards = customTrack.Standards.Where(s => s.Type == category).OrderBy(s => s.Time).ToList();

        foreach (var standard in standards)
        {
            if (int.Parse(time) < int.Parse(standard.Time))
            {
                customTrackStandard = standard;
                break;
            }
        }

        return customTrackStandard;
    }
}
