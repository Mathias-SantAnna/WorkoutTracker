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
        [Range(1, 100)]
        public int Sets { get; set; }
        
        [Required]
        [Range(1, 1000)]
        public int Reps { get; set; }
        
        [Display(Name = "Weight (lbs)")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        public decimal? WeightLbs { get; set; }
        
        public virtual Workout? Workout { get; set; }
        public virtual Exercise? Exercise { get; set; }
    }
}