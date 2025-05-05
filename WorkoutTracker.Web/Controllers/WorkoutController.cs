using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Web.Models;
using System.Globalization;

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
                try
                {
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
                                    WeightLbs = (i < WeightLbs?.Count && WeightLbs[i].HasValue) ? WeightLbs[i] : null
                                };
                
                                _context.WorkoutExercises.Add(workoutExercise);
                            }
                        }
        
                        await _context.SaveChangesAsync();
                    }
    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine("Error creating workout: " + ex.Message);
                    ModelState.AddModelError("", "Error creating workout: " + ex.Message);
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }
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
    
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
    
            ViewBag.WorkoutExercises = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .Where(we => we.WorkoutId == id)
                .ToListAsync();
    
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
            
            try
            {
                var existingWorkout = await _context.Workouts.FindAsync(id);
                if (existingWorkout == null)
                {
                    return NotFound();
                }
                
                existingWorkout.Name = workout.Name;
                existingWorkout.Description = workout.Description;
                existingWorkout.WorkoutDate = workout.WorkoutDate;
                existingWorkout.DurationMinutes = workout.DurationMinutes;
                
                _context.Update(existingWorkout);
                await _context.SaveChangesAsync();
                
                var exerciseKeys = Request.Form.Keys.Where(k => k.StartsWith("ExistingExerciseIds[")).ToList();
                if (exerciseKeys.Any())
                {
                    // Process each exercise
                    for (int i = 0; i < exerciseKeys.Count; i++)
                    {
                        var idKey = exerciseKeys[i];
                        var idValue = Request.Form[idKey][0];
                        
                        var index = idKey.Substring(idKey.IndexOf('[') + 1, idKey.IndexOf(']') - idKey.IndexOf('[') - 1);
                        
                        var typeKey = $"ExistingExerciseTypeIds[{index}]";
                        var setsKey = $"ExistingSets[{index}]";
                        var repsKey = $"ExistingReps[{index}]";
                        var weightKey = $"ExistingWeights[{index}]";
                        
                        if (int.TryParse(idValue, out int exerciseId))
                        {
                            // Get from database
                            var exercise = await _context.WorkoutExercises.FindAsync(exerciseId);
                            if (exercise != null)
                            {
                                exercise.ExerciseId = int.Parse(Request.Form[typeKey][0]);
                                exercise.Sets = int.Parse(Request.Form[setsKey][0]);
                                exercise.Reps = int.Parse(Request.Form[repsKey][0]);
                                
                                var weightStr = Request.Form[weightKey][0];
                                
                                if (string.IsNullOrEmpty(weightStr))
                                {
                                    exercise.WeightLbs = null;
                                }
                                else
                                {
                                    weightStr = weightStr.Replace(",", ".");
                                    if (decimal.TryParse(weightStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weightValue))
                                    {
                                        exercise.WeightLbs = weightValue;
                                    }
                                    else
                                    {
                                        exercise.WeightLbs = null;
                                    }
                                }
                                
                                _context.Update(exercise);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                
                var newExerciseKeys = Request.Form.Keys.Where(k => k.StartsWith("NewExerciseIds[")).ToList();
                if (newExerciseKeys.Any())
                {
                    for (int i = 0; i < newExerciseKeys.Count; i++)
                    {
                        var idKey = newExerciseKeys[i];
                        var idValue = Request.Form[idKey][0];
                        
                        var index = idKey.Substring(idKey.IndexOf('[') + 1, idKey.IndexOf(']') - idKey.IndexOf('[') - 1);
                        
                        var setsKey = $"NewSets[{index}]";
                        var repsKey = $"NewReps[{index}]";
                        var weightKey = $"NewWeights[{index}]";
                        
                        if (!string.IsNullOrEmpty(idValue) && int.TryParse(idValue, out int exerciseTypeId) && exerciseTypeId > 0)
                        {
                            if (Request.Form.ContainsKey(setsKey) && Request.Form.ContainsKey(repsKey) &&
                                int.TryParse(Request.Form[setsKey][0], out int sets) && sets > 0 &&
                                int.TryParse(Request.Form[repsKey][0], out int reps) && reps > 0)
                            {
                                decimal? weight = null;
                                if (Request.Form.ContainsKey(weightKey) && !string.IsNullOrEmpty(Request.Form[weightKey][0]))
                                {
                                    var weightStr = Request.Form[weightKey][0].Replace(",", ".");
                                    if (decimal.TryParse(weightStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weightValue))
                                    {
                                        weight = weightValue;
                                    }
                                }
                                
                                var newExercise = new WorkoutExercise
                                {
                                    WorkoutId = workout.WorkoutId,
                                    ExerciseId = exerciseTypeId,
                                    Sets = sets,
                                    Reps = reps,
                                    WeightLbs = weight
                                };
                                
                                _context.WorkoutExercises.Add(newExercise);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                
                // Handle deleted exercises
                var deletedExerciseKeys = Request.Form.Keys.Where(k => k.StartsWith("DeletedExerciseIds")).ToList();
                if (deletedExerciseKeys.Any())
                {
                    foreach (var key in deletedExerciseKeys)
                    {
                        var idValue = Request.Form[key][0];
                        if (int.TryParse(idValue, out int exerciseId))
                        {
                            var exercise = await _context.WorkoutExercises.FindAsync(exerciseId);
                            if (exercise != null)
                            {
                                _context.WorkoutExercises.Remove(exercise);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                
                return RedirectToAction(nameof(Details), new { id = workout.WorkoutId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception in Edit: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Error saving workout: " + ex.Message);
            }
            
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
                    // Add the first exercise
                    if (workoutExercise.ExerciseId > 0 && workoutExercise.Sets > 0 && workoutExercise.Reps > 0)
                    {
                        _context.WorkoutExercises.Add(workoutExercise);
                    }
                    
                    // Add more exercises
                    if (AdditionalExerciseIds != null && AdditionalExerciseIds.Count > 0)
                    {
                        for (int i = 0; i < AdditionalExerciseIds.Count; i++)
                        {
                            if (AdditionalExerciseIds[i] > 0 && i < AdditionalSets?.Count && i < AdditionalReps?.Count && 
                                AdditionalSets[i] > 0 && AdditionalReps[i] > 0)
                            {
                                var additionalExercise = new WorkoutExercise
                                {
                                    WorkoutId = workoutExercise.WorkoutId,
                                    ExerciseId = AdditionalExerciseIds[i],
                                    Sets = AdditionalSets[i],
                                    Reps = AdditionalReps[i],
                                    WeightLbs = (i < AdditionalWeightLbs?.Count && AdditionalWeightLbs[i].HasValue) 
                                        ? AdditionalWeightLbs[i] 
                                        : null
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
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
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