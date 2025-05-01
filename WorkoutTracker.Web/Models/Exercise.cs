using System.Collections.Generic;

namespace WorkoutTracker.Web.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetMuscle { get; set; } = string.Empty;
        
        public virtual ICollection<WorkoutExercise>? WorkoutExercises { get; set; }
    }
}