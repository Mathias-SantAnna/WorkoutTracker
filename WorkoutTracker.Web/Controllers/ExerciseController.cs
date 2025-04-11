using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.Web.Controllers
{
    public class ExerciseController : Controller
    {
        private static List<Exercise> exercises = new List<Exercise>
        {
            new Exercise { 
                ExerciseId = 1, 
                Name = "Bench Press", 
                Description = "Compound chest exercise", 
                TargetMuscle = "Chest" 
            },
            new Exercise { 
                ExerciseId = 2, 
                Name = "Squat", 
                Description = "Fundamental lower body exercise", 
                TargetMuscle = "Quadriceps" 
            },
            new Exercise { 
                ExerciseId = 3, 
                Name = "Deadlift", 
                Description = "Full body pulling movement", 
                TargetMuscle = "Lower Back, Hamstrings" 
            },
        };

        public IActionResult Index() => View(exercises);

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                int newId = exercises.Any() ? exercises.Max(i => i.ExerciseId) + 1 : 1;
                exercise.ExerciseId = newId;
                exercises.Add(exercise);
                return RedirectToAction("Index");
            }
            return View(exercise);
        }

        public IActionResult Edit(int id)
        {
            var exercise = exercises.FirstOrDefault(i => i.ExerciseId == id);
            if (exercise == null)
                return NotFound();
            return View(exercise);
        }

        [HttpPost]
        public IActionResult Edit(int id, Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                var existing = exercises.FirstOrDefault(i => i.ExerciseId == id);
                if (existing != null)
                {
                    existing.Name = exercise.Name;
                    existing.Description = exercise.Description;
                    existing.TargetMuscle = exercise.TargetMuscle;
                }

                return RedirectToAction("Index");
            }
            return View(exercise);
        }

        public IActionResult Delete(int id)
        {
            var exercise = exercises.FirstOrDefault(i => i.ExerciseId == id);
            if (exercise == null)
                return NotFound();
            return View(exercise);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var exercise = exercises.FirstOrDefault(i => i.ExerciseId == id);
            if (exercise != null)
                exercises.Remove(exercise);

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var exercise = exercises.FirstOrDefault(i => i.ExerciseId == id);
            if (exercise == null)
                return NotFound();
            return View(exercise);
        }
    }
}