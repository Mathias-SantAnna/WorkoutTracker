using System;

namespace WorkoutTracker.Web.Models
{
    public class ExerciseProgressEntry
    {
        public DateTime Date { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightLbs { get; set; }
    }
}