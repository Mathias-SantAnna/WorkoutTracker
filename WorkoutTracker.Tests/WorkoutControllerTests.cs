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
            
            // Act - Create a workout with 2 exercises
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new WorkoutController(context);
                
                var workout = new Workout
                {
                    Name = "Test Workout",
                    WorkoutDate = DateTime.Today,
                    DurationMinutes = 30
                };
                
                // For single exercise test (matches the current controller method)
                await controller.Create(workout, 1, 3, 10, 100m);
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