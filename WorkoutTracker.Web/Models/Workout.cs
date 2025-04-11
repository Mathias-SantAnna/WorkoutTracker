namespace WorkoutTracker.Web.Models;

public class Workout
{
    
}using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Web.Models
{
    public class Workout
    {
        public int WorkoutId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Display(Name = "Workout Date")]
        [DataType(DataType.Date)]
        public DateTime WorkoutDate { get; set; }
        
        [Range(1, 300)]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }
        
        // Navigation property
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}