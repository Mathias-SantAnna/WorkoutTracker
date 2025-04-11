using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTracker.Web.Models
{
    public class WorkoutExercise
    {
        public int WorkoutExerciseId { get; set; }
        
        [ForeignKey("Workout")]
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
        
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
        
        [Range(1, 100)]
        public int Sets { get; set; }
        
        [Range(1, 1000)]
        public int Reps { get; set; }
        
        [Range(0, 10000)]
        [Display(Name = "Weight (lbs)")]
        public decimal? WeightLbs { get; set; }
    }
}