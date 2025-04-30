using System;

namespace WorkoutTracker.Web.Models
{
    public class RecentWorkout
    {
        public int WorkoutId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime WorkoutDate { get; set; }
        public int DurationMinutes { get; set; }
        public int ExerciseCount { get; set; }
    }
}