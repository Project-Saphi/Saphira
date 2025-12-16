using Saphira.Saphi.Entity;

namespace Saphira.Saphi.Game;

public class StandardCalculator
{
    public CustomTrackStandard? CalculateStandard(CustomTrack customTrack, int category, int time)
    {
        CustomTrackStandard? customTrackStandard = null;

        if (customTrack.Standards == null || customTrack.Standards.Count == 0)
        {
            return null;
        }

        var standards = customTrack.Standards.Where(s => s.CategoryId == category).OrderBy(s => s.Time).ToList();

        foreach (var standard in standards)
        {
            if (time < standard.Time)
            {
                customTrackStandard = standard;
                break;
            }
        }

        return customTrackStandard;
    }
}
