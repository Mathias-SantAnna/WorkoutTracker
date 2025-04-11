using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutAnalysisController : ControllerBase
    {
        private static List<WorkoutExercise> workoutExercises = new List<WorkoutExercise>
        {
            new WorkoutExercise { WorkoutExerciseId = 1, WorkoutId = 1, ExerciseId = 1, Sets = 3, Reps = 10, WeightLbs = 135 },
            new WorkoutExercise { WorkoutExerciseId = 2, WorkoutId = 1, ExerciseId = 2, Sets = 4, Reps = 8, WeightLbs = 225 },
            new WorkoutExercise { WorkoutExerciseId = 3, WorkoutId = 2, ExerciseId = 1, Sets = 5, Reps = 5, WeightLbs = 155 },
            new WorkoutExercise { WorkoutExerciseId = 4, WorkoutId = 2, ExerciseId = 3, Sets = 3, Reps = 5, WeightLbs = 275 },
            new WorkoutExercise { WorkoutExerciseId = 5, WorkoutId = 3, ExerciseId = 2, Sets = 4, Reps = 8, WeightLbs = 245 }
        };

        private static List<Exercise> exercises = new List<Exercise>
        {
            new Exercise { ExerciseId = 1, Name = "Bench Press", Description = "Compound chest exercise", TargetMuscle = "Chest" },
            new Exercise { ExerciseId = 2, Name = "Squat", Description = "Fundamental lower body exercise", TargetMuscle = "Quadriceps" },
            new Exercise { ExerciseId = 3, Name = "Deadlift", Description = "Full body pulling movement", TargetMuscle = "Lower Back, Hamstrings" }
        };

        // GET: api/workoutanalysis/popular
        [HttpGet("popular")]
        public IActionResult GetPopularExercises()
        {
            var popularity = workoutExercises
                .GroupBy(we => we.ExerciseId)
                .Select(group => new
                {
                    Exercise = exercises.First(e => e.ExerciseId == group.Key).Name,
                    Count = group.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return Ok(popularity);
        }
    }

    // Models for the API project (simplified versions)
    public class Exercise
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TargetMuscle { get; set; }
    }

    public class WorkoutExercise
    {
        public int WorkoutExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightLbs { get; set; }
    }
}