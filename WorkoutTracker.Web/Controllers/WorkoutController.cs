using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.Web.Controllers
{
    public class WorkoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Workout
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workouts.ToListAsync());
        }

        // GET: Workout/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var workout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(w => w.WorkoutId == id);

            if (workout == null)
                return NotFound();

            return View(workout);
        }

        // GET: Workout/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            return View();
        }

        // POST: Workout/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workout workout, int? ExerciseId, int? Sets, int? Reps, decimal? WeightLbs)
        {
            if (ModelState.IsValid)
            {
                // First save the workout
                _context.Add(workout);
                await _context.SaveChangesAsync();
        
                // If exercise details are provided, add the exercise to the workout
                if (ExerciseId.HasValue && Sets.HasValue && Reps.HasValue)
                {
                    var workoutExercise = new WorkoutExercise
                    {
                        WorkoutId = workout.WorkoutId,
                        ExerciseId = ExerciseId.Value,
                        Sets = Sets.Value,
                        Reps = Reps.Value,
                        WeightLbs = WeightLbs
                    };
            
                    _context.WorkoutExercises.Add(workoutExercise);
                    await _context.SaveChangesAsync();
                }
        
                return RedirectToAction(nameof(Index));
            }
    
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            return View(workout);
        }

        // GET: Workout/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
                return NotFound();
            return View(workout);
        }

        // POST: Workout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Workout workout)
        {
            if (id != workout.WorkoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.WorkoutId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: Workout/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
                return NotFound();
            return View(workout);
        }

        // POST: Workout/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Workout/AddExercise/5
        public async Task<IActionResult> AddExercise(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            
            ViewBag.Workout = workout;
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            return View(new WorkoutExercise { WorkoutId = id });
        }

        // POST: Workout/AddExercise
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExercise(WorkoutExercise workoutExercise)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.WorkoutExercises.Add(workoutExercise);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = workoutExercise.WorkoutId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error adding exercise: " + ex.Message);
                }
            }
            
            ViewBag.Workout = await _context.Workouts.FindAsync(workoutExercise.WorkoutId);
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            return View(workoutExercise);
        }

        private bool WorkoutExists(int id)
        {
            return _context.Workouts.Any(e => e.WorkoutId == id);
        }
    }
}