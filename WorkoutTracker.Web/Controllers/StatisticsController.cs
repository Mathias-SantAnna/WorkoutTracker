using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Web.Models;

namespace WorkoutTracker.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get workout stats
            var workoutStats = new WorkoutStats
            {
                TotalWorkouts = await _context.Workouts.CountAsync(),
                TotalExercises = await _context.WorkoutExercises.CountAsync(),
                TotalDurationMinutes = await _context.Workouts.SumAsync(w => w.DurationMinutes),
                AverageDurationMinutes = await _context.Workouts.AnyAsync()
                    ? (int)await _context.Workouts.AverageAsync(w => (double)w.DurationMinutes)
                    : 0
            };

            // Get popular exercises
            var popularExercises = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .GroupBy(we => we.ExerciseId)
                .Select(g => new ExerciseUsage
                {
                    Exercise = g.First().Exercise.Name,
                    Count = g.Count()
                })
                .OrderByDescending(e => e.Count)
                .Take(5)
                .ToListAsync();

            // Get recent workouts
            var recentWorkouts = await _context.Workouts
                .OrderByDescending(w => w.WorkoutDate)
                .Take(10)
                .Select(w => new RecentWorkout
                {
                    WorkoutId = w.WorkoutId,
                    Name = w.Name,
                    WorkoutDate = w.WorkoutDate,
                    DurationMinutes = w.DurationMinutes,
                    ExerciseCount = w.WorkoutExercises.Count
                })
                .ToListAsync();

            // Get exercise progress
            ExerciseProgressResponse exerciseProgress = null;
            var commonExerciseId = await _context.WorkoutExercises
                .GroupBy(we => we.ExerciseId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (commonExerciseId != 0)
            {
                var exercise = await _context.Exercises.FindAsync(commonExerciseId);
                if (exercise != null)
                {
                    var progressData = await _context.WorkoutExercises
                        .Include(we => we.Workout)
                        .Where(we => we.ExerciseId == commonExerciseId)
                        .OrderBy(we => we.Workout.WorkoutDate)
                        .Select(we => new ExerciseProgressEntry
                        {
                            Date = we.Workout.WorkoutDate,
                            Sets = we.Sets,
                            Reps = we.Reps,
                            WeightLbs = we.WeightLbs
                        })
                        .ToListAsync();

                    exerciseProgress = new ExerciseProgressResponse
                    {
                        Exercise = exercise.Name,
                        ProgressData = progressData
                    };
                }
            }

            var viewModel = new StatisticsViewModel
            {
                WorkoutStats = workoutStats,
                PopularExercises = popularExercises,
                RecentWorkouts = recentWorkouts,
                ExerciseProgress = exerciseProgress
            };

            return View(viewModel);
        }
    }
}