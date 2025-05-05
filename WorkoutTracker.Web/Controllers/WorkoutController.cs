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
            // Updated to include WorkoutExercises to show correct exercise count
            return View(await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ToListAsync());
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
        public async Task<IActionResult> Create(Workout workout, 
            List<int> ExerciseIds, 
            List<int> Sets, 
            List<int> Reps, 
            List<decimal?> WeightLbs)
        {
            if (ModelState.IsValid)
            {
                // First save the workout
                _context.Add(workout);
                await _context.SaveChangesAsync();
        
                // Process all submitted exercises
                if (ExerciseIds != null)
                {
                    for (int i = 0; i < ExerciseIds.Count; i++)
                    {
                        // Only add if a valid exercise is selected and has sets/reps
                        if (ExerciseIds[i] > 0 && i < Sets.Count && i < Reps.Count && 
                            Sets[i] > 0 && Reps[i] > 0)
                        {
                            var workoutExercise = new WorkoutExercise
                            {
                                WorkoutId = workout.WorkoutId,
                                ExerciseId = ExerciseIds[i],
                                Sets = Sets[i],
                                Reps = Reps[i],
                                WeightLbs = i < WeightLbs.Count ? WeightLbs[i] : null
                            };
                    
                            _context.WorkoutExercises.Add(workoutExercise);
                        }
                    }
            
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
            
            // Get all exercises for the dropdown
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            
            // Get this workout's exercises
            ViewBag.WorkoutExercises = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .Where(we => we.WorkoutId == id)
                .ToListAsync();
            
            return View(workout);
        }

        // POST: Workout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, 
            Workout workout,
            // Existing exercises
            List<int> ExistingExerciseIds,
            List<int> ExistingExerciseTypeIds,
            List<int> ExistingSets,
            List<int> ExistingReps,
            List<decimal?> ExistingWeights,
            // New exercises
            List<int> NewExerciseIds,
            List<int> NewSets,
            List<int> NewReps,
            List<decimal?> NewWeights,
            // Deleted exercises
            List<int> DeletedExerciseIds)
        {
            if (id != workout.WorkoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update workout basic info
                    _context.Update(workout);
                    
                    // 1. Update existing exercises
                    if (ExistingExerciseIds != null)
                    {
                        for (int i = 0; i < ExistingExerciseIds.Count; i++)
                        {
                            // Get the workout exercise
                            var workoutExercise = await _context.WorkoutExercises
                                .FindAsync(ExistingExerciseIds[i]);
                            
                            if (workoutExercise != null)
                            {
                                // Update its properties
                                workoutExercise.ExerciseId = ExistingExerciseTypeIds[i];
                                workoutExercise.Sets = ExistingSets[i];
                                workoutExercise.Reps = ExistingReps[i];
                                workoutExercise.WeightLbs = i < ExistingWeights.Count ? ExistingWeights[i] : null;
                                
                                _context.Update(workoutExercise);
                            }
                        }
                    }
                    
                    // 2. Delete exercises marked for deletion
                    if (DeletedExerciseIds != null)
                    {
                        foreach (var exerciseId in DeletedExerciseIds)
                        {
                            var workoutExercise = await _context.WorkoutExercises
                                .FindAsync(exerciseId);
                            
                            if (workoutExercise != null)
                            {
                                _context.WorkoutExercises.Remove(workoutExercise);
                            }
                        }
                    }
                    
                    // 3. Add new exercises
                    if (NewExerciseIds != null)
                    {
                        for (int i = 0; i < NewExerciseIds.Count; i++)
                        {
                            if (NewExerciseIds[i] > 0 && i < NewSets.Count && i < NewReps.Count &&
                                NewSets[i] > 0 && NewReps[i] > 0)
                            {
                                var newExercise = new WorkoutExercise
                                {
                                    WorkoutId = workout.WorkoutId,
                                    ExerciseId = NewExerciseIds[i],
                                    Sets = NewSets[i],
                                    Reps = NewReps[i],
                                    WeightLbs = i < NewWeights.Count ? NewWeights[i] : null
                                };
                                
                                _context.WorkoutExercises.Add(newExercise);
                            }
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = workout.WorkoutId });
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
            }
            
            // If we got this far, something failed, redisplay form
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            ViewBag.WorkoutExercises = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .Where(we => we.WorkoutId == id)
                .ToListAsync();
            
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
        public async Task<IActionResult> AddExercise(WorkoutExercise workoutExercise, 
            List<int> AdditionalExerciseIds, 
            List<int> AdditionalSets, 
            List<int> AdditionalReps, 
            List<decimal?> AdditionalWeightLbs)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Add the main exercise
                    if (workoutExercise.ExerciseId > 0 && workoutExercise.Sets > 0 && workoutExercise.Reps > 0)
                    {
                        _context.WorkoutExercises.Add(workoutExercise);
                    }
                    
                    // Add any additional exercises
                    if (AdditionalExerciseIds != null && AdditionalExerciseIds.Count > 0)
                    {
                        for (int i = 0; i < AdditionalExerciseIds.Count; i++)
                        {
                            if (AdditionalExerciseIds[i] > 0 && i < AdditionalSets.Count && i < AdditionalReps.Count && 
                                AdditionalSets[i] > 0 && AdditionalReps[i] > 0)
                            {
                                var additionalExercise = new WorkoutExercise
                                {
                                    WorkoutId = workoutExercise.WorkoutId,
                                    ExerciseId = AdditionalExerciseIds[i],
                                    Sets = AdditionalSets[i],
                                    Reps = AdditionalReps[i],
                                    WeightLbs = i < AdditionalWeightLbs.Count ? AdditionalWeightLbs[i] : null
                                };
                                
                                _context.WorkoutExercises.Add(additionalExercise);
                            }
                        }
                    }
                    
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