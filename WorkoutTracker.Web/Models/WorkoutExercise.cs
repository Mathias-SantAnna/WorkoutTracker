using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Web.Models
{
    public class WorkoutExercise
    {
        public int WorkoutExerciseId { get; set; }
        
        [Required]
        public int WorkoutId { get; set; }
        
        [Required]
        public int ExerciseId { get; set; }
        
        [Required]
        [Range(1, 20)]
        public int Sets { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Reps { get; set; }
        
        [Display(Name = "Weight (lbs)")]
        public decimal? WeightLbs { get; set; }
        
        // Navigation properties
        public virtual Workout? Workout { get; set; }
        public virtual Exercise? Exercise { get; set; }
    }
}