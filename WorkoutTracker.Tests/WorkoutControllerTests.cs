using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Web.Controllers;
using WorkoutTracker.Web.Models;

namespace WorkoutTracker.Tests
{
    [TestFixture]
    public class WorkoutControllerTests
    {
        [Test]
        public async Task Create_WithMultipleExercises_AddsExercisesToWorkout()
        {
            // Arrange - Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestWorkoutDb")
                .Options;
    
            // Add test exercises to database
            using (var context = new ApplicationDbContext(options))
            {
                context.Exercises.Add(new Exercise { ExerciseId = 1, Name = "Bench Press" });
                context.Exercises.Add(new Exercise { ExerciseId = 2, Name = "Squat" });
                context.SaveChanges();
            }
    
            // Act - Create a workout with exercises
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new WorkoutController(context);
        
                var workout = new Workout
                {
                    Name = "Test Workout",
                    WorkoutDate = DateTime.Today,
                    DurationMinutes = 30
                };
        
                // Create lists for the multiple exercise parameters
                var exerciseIds = new List<int> { 1 };
                var sets = new List<int> { 3 };
                var reps = new List<int> { 10 };
                var weights = new List<decimal?> { 100m };
        
                // Call Create with the lists
                await controller.Create(workout, exerciseIds, sets, reps, weights);
            }
    
            // Assert - Verify workout and exercise was saved correctly
            using (var context = new ApplicationDbContext(options))
            {
                var savedWorkout = context.Workouts
                    .Include(w => w.WorkoutExercises)
                    .FirstOrDefault(w => w.Name == "Test Workout");
        
                Assert.IsNotNull(savedWorkout);
                Assert.AreEqual(1, savedWorkout.WorkoutExercises.Count);
        
                var exercise1 = savedWorkout.WorkoutExercises.First(we => we.ExerciseId == 1);
                Assert.AreEqual(3, exercise1.Sets);
                Assert.AreEqual(10, exercise1.Reps);
                Assert.AreEqual(100m, exercise1.WeightLbs);
            }
        }
    }
}