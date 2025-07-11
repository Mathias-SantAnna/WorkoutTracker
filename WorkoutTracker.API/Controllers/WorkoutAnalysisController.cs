using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Web.Models;

namespace WorkoutTracker.API.Controllers
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

        [HttpGet("popular")]
        public async Task<List<ExerciseUsage>> GetPopularExercises()
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

            return popularExercises;
        }

        [HttpGet("stats")]
        public async Task<WorkoutStats> GetWorkoutStats()
        {
            var totalWorkouts = await _context.Workouts.CountAsync();
            var totalExercises = await _context.WorkoutExercises.CountAsync();
            var totalDuration = await _context.Workouts.SumAsync(w => w.DurationMinutes);
            
            return new WorkoutStats
            {
                TotalWorkouts = totalWorkouts,
                TotalExercises = totalExercises,
                TotalDurationMinutes = totalDuration,
                AverageDurationMinutes = totalWorkouts > 0 ? totalDuration / totalWorkouts : 0
            };
        }

        [HttpGet("recent")]
        public async Task<List<RecentWorkout>> GetRecentWorkouts(int count = 5)
        {
            var recentWorkouts = await _context.Workouts
                .OrderByDescending(w => w.WorkoutDate)
                .Take(count)
                .Select(w => new RecentWorkout
                {
                    WorkoutId = w.WorkoutId,
                    Name = w.Name,
                    WorkoutDate = w.WorkoutDate,
                    DurationMinutes = w.DurationMinutes,
                    ExerciseCount = w.WorkoutExercises.Count
                })
                .ToListAsync();

            return recentWorkouts;
        }

        [HttpGet("progress/{exerciseId}")]
        public async Task<ActionResult<ExerciseProgressResponse>> GetExerciseProgress(int exerciseId)
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
                .Select(we => new ExerciseProgressEntry
                {
                    Date = we.Workout.WorkoutDate,
                    Sets = we.Sets,
                    Reps = we.Reps,
                    WeightLbs = we.WeightLbs
                })
                .ToListAsync();

            return new ExerciseProgressResponse
            {
                Exercise = exercise.Name,
                ProgressData = progressData
            };
        }
    }
}