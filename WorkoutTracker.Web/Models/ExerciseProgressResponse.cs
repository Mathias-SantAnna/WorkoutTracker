using System.Collections.Generic;

namespace WorkoutTracker.Web.Models
{
    public class ExerciseProgressResponse
    {
        public string Exercise { get; set; } = string.Empty;
        public List<ExerciseProgressEntry> ProgressData { get; set; } = new List<ExerciseProgressEntry>();
    }
}