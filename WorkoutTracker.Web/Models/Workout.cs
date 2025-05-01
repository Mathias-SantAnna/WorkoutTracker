using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Web.Models
{
    public class Workout
    {
        public int WorkoutId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Display(Name = "Workout Date")]
        [DataType(DataType.Date)]
        public DateTime WorkoutDate { get; set; }
        
        [Range(1, 300)]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }
        
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}