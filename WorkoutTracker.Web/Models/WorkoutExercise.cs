namespace WorkoutTracker.Web.Models
{
    public class WorkoutExercise
    {
        public int WorkoutExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightLbs { get; set; }
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Workout? Workout { get; set; }
        public virtual Exercise? Exercise { get; set; }
    }
}