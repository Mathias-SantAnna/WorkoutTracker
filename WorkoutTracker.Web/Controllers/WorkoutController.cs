using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.Web.Controllers
{
    public class WorkoutController : Controller
    {
        private static List<Workout> workouts = new List<Workout>() // Temporary list before DB setup 
        {
            new Workout { 
                WorkoutId = 1, 
                Name = "Full Body Workout", 
                Description = "A comprehensive full body workout", 
                WorkoutDate = DateTime.Now.AddDays(-7), 
                DurationMinutes = 60 
            },
            new Workout { 
                WorkoutId = 2, 
                Name = "Upper Body Focus", 
                Description = "Targets chest, shoulders, and arms", 
                WorkoutDate = DateTime.Now.AddDays(-3), 
                DurationMinutes = 45 
            },
            new Workout { 
                WorkoutId = 3, 
                Name = "Leg Day", 
                Description = "Lower body workout for strength", 
                WorkoutDate = DateTime.Now.AddDays(-1), 
                DurationMinutes = 55 
            }
        };

        public IActionResult Index()
        {
            return View(workouts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Workout workout)
        {
            if (ModelState.IsValid)
            {
                int newId = workouts.Any() ? workouts.Max(w => w.WorkoutId) + 1 : 1;
                workout.WorkoutId = newId;
                workouts.Add(workout);
                return RedirectToAction("Index");
            }
            return View(workout);
        }

        public IActionResult Edit(int id)
        {
            var workout = workouts.FirstOrDefault(w => w.WorkoutId == id);
            if (workout == null)
                return NotFound();
            return View(workout);
        }

        public IActionResult Details(int id)
        {
            var workout = workouts.FirstOrDefault(w => w.WorkoutId == id);
            if (workout == null)
                return NotFound();

            return View(workout);
        }

        [HttpPost]
        public IActionResult Edit(int id, Workout workout)
        {
            if (ModelState.IsValid)
            {
                var existingWorkout = workouts.FirstOrDefault(w => w.WorkoutId == id);
                if (existingWorkout != null)
                {
                    existingWorkout.Name = workout.Name;
                    existingWorkout.Description = workout.Description;
                    existingWorkout.WorkoutDate = workout.WorkoutDate;
                    existingWorkout.DurationMinutes = workout.DurationMinutes;
                }
                return RedirectToAction("Index");
            }
            return View(workout);
        }

        public IActionResult Delete(int id)
        {
            var workout = workouts.FirstOrDefault(w => w.WorkoutId == id);
            if (workout == null)
                return NotFound();
            return View(workout);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var workout = workouts.FirstOrDefault(w => w.WorkoutId == id);
            if (workout != null)
                workouts.Remove(workout);
            return RedirectToAction("Index");
        }
    }
}