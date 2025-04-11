using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.Web.Controllers
{
    public class WorkoutExerciseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutExerciseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkoutExercise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.WorkoutExerciseId == id);

            if (workoutExercise == null)
            {
                return NotFound();
            }

            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            return View(workoutExercise);
        }

        // POST: WorkoutExercise/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutExercise workoutExercise)
        {
            if (id != workoutExercise.WorkoutExerciseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workoutExercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExerciseExists(workoutExercise.WorkoutExerciseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Workout", new { id = workoutExercise.WorkoutId });
            }
            
            ViewBag.Exercises = await _context.Exercises.ToListAsync();
            return View(workoutExercise);
        }

        // GET: WorkoutExercise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await _context.WorkoutExercises
                .Include(we => we.Exercise)
                .Include(we => we.Workout)
                .FirstOrDefaultAsync(we => we.WorkoutExerciseId == id);
                
            if (workoutExercise == null)
            {
                return NotFound();
            }

            return View(workoutExercise);
        }

        // POST: WorkoutExercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutExercise = await _context.WorkoutExercises.FindAsync(id);
            if (workoutExercise != null)
            {
                var workoutId = workoutExercise.WorkoutId;
                _context.WorkoutExercises.Remove(workoutExercise);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Workout", new { id = workoutId });
            }
            
            return RedirectToAction("Index", "Workout");
        }

        private bool WorkoutExerciseExists(int id)
        {
            return _context.WorkoutExercises.Any(e => e.WorkoutExerciseId == id);
        }
    }
}