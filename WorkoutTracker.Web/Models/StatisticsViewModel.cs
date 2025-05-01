using System.Collections.Generic;
using WorkoutTracker.Web.Models;

namespace WorkoutTracker.Web.Models
{
    public class StatisticsViewModel
    {
        public WorkoutStats WorkoutStats { get; set; }
        public List<ExerciseUsage> PopularExercises { get; set; }
        public List<RecentWorkout> RecentWorkouts { get; set; }
        public ExerciseProgressResponse ExerciseProgress { get; set; }
    }
}