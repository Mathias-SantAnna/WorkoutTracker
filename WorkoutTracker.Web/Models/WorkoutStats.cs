namespace WorkoutTracker.Web.Models
{
    public class WorkoutStats
    {
        public int TotalWorkouts { get; set; }
        public int TotalExercises { get; set; }
        public int TotalDurationMinutes { get; set; }
        public int AverageDurationMinutes { get; set; }
    }
}