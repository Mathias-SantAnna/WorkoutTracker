using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Web.Models;

namespace WorkoutTracker.Web.Controllers
{
    [ApiController]
    [Route("api/workoutanalysis")]
    public class WorkoutAnalysisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkoutAnalysisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/workoutanalysis/popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<ExerciseUsage>>> GetPopularExercises()
        {
            var popularExercises = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .GroupBy(we => we.ExerciseId)
                .Select(g => new ExerciseUsage
                {
                    Exercise = g.First().Exercise.Name,
                    Count = g.Count()
                })
                .OrderByDescending(e => e.Count)
                .ToListAsync();

            return Ok(popularExercises);
        }

        // GET: api/workoutanalysis/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetWorkoutStats()
        {
            var totalWorkouts = await _context.Workouts.CountAsync();
            var totalExercises = await _context.WorkoutExercises.CountAsync();
            var totalDuration = await _context.Workouts.SumAsync(w => w.DurationMinutes);
            
            var stats = new
            {
                TotalWorkouts = totalWorkouts,
                TotalExercises = totalExercises,
                TotalDurationMinutes = totalDuration,
                AverageDurationMinutes = totalWorkouts > 0 ? totalDuration / totalWorkouts : 0
            };

            return Ok(stats);
        }

        // GET: api/workoutanalysis/recent?count=5
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecentWorkouts(int count = 5)
        {
            var recentWorkouts = await _context.Workouts
                .OrderByDescending(w => w.WorkoutDate)
                .Take(count)
                .Select(w => new
                {
                    w.WorkoutId,
                    w.Name,
                    w.WorkoutDate,
                    w.DurationMinutes,
                    ExerciseCount = w.WorkoutExercises.Count
                })
                .ToListAsync();

            return Ok(recentWorkouts);
        }

        // GET: api/workoutanalysis/progress/5
        [HttpGet("progress/{exerciseId}")]
        public async Task<ActionResult<object>> GetExerciseProgress(int exerciseId)
        {
            var exercise = await _context.Exercises.FindAsync(exerciseId);
            if (exercise == null)
            {
                return NotFound();
            }

            var progressData = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .Where(we => we.ExerciseId == exerciseId)
                .OrderBy(we => we.Workout.WorkoutDate)
                .Select(we => new
                {
                    Date = we.Workout.WorkoutDate,
                    we.Sets,
                    we.Reps,
                    we.WeightLbs
                })
                .ToListAsync();

            var response = new
            {
                Exercise = exercise.Name,
                ProgressData = progressData
            };

            return Ok(response);
        }
    }
}