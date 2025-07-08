namespace StudentProgressTracker.DTOs
{
    public class PerformanceLevels
    {
        public int Struggling { get; set; }
        public int OnTrack { get; set; }
        public int Advanced { get; set; }
        public int Total => Struggling + OnTrack + Advanced;
        public double StrugglingPercentage => Total > 0 ? Math.Round((double)Struggling / Total * 100, 1) : 0;
        public double OnTrackPercentage => Total > 0 ? Math.Round((double)OnTrack / Total * 100, 1) : 0;
        public double AdvancedPercentage => Total > 0 ? Math.Round((double)Advanced / Total * 100, 1) : 0;
    }

}
