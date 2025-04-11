using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Web.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(100)]
        [Display(Name = "Target Muscle")]
        public string? TargetMuscle { get; set; }
        
        // Navigation property
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}