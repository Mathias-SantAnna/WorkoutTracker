using WorkoutTracker.Web.Models;

namespace WorkoutTracker.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any exercises
            if (context.Exercises.Any())
            {
                return;
            }

            var exercises = new Exercise[]
            {
                new Exercise { Name = "Bench Press", Description = "Compound chest exercise", TargetMuscle = "Chest" },
                new Exercise { Name = "Squat", Description = "Fundamental lower body exercise", TargetMuscle = "Quadriceps" },
                new Exercise { Name = "Deadlift", Description = "Full body pulling movement", TargetMuscle = "Lower Back, Hamstrings" },
                new Exercise { Name = "Pull-Up", Description = "Upper body pulling exercise", TargetMuscle = "Back, Biceps" },
                new Exercise { Name = "Shoulder Press", Description = "Overhead pressing movement", TargetMuscle = "Shoulders" },
                new Exercise { Name = "Barbell Row", Description = "Horizontal pulling movement", TargetMuscle = "Back" },
                new Exercise { Name = "Leg Press", Description = "Machine-based leg exercise", TargetMuscle = "Quadriceps, Hamstrings" },
                new Exercise { Name = "Lat Pulldown", Description = "Vertical pulling movement", TargetMuscle = "Back, Biceps" }
            };

            context.Exercises.AddRange(exercises);
            context.SaveChanges();

            var workouts = new Workout[]
            {
                new Workout {
                    Name = "Full Body Workout",
                    Description = "A comprehensive full body workout",
                    WorkoutDate = DateTime.Now.AddDays(-7),
                    DurationMinutes = 60
                },
                new Workout {
                    Name = "Upper Body Focus",
                    Description = "Targets chest, shoulders, and arms",
                    WorkoutDate = DateTime.Now.AddDays(-3),
                    DurationMinutes = 45
                },
                new Workout {
                    Name = "Leg Day",
                    Description = "Lower body workout for strength",
                    WorkoutDate = DateTime.Now.AddDays(-1),
                    DurationMinutes = 55
                }
            };

            context.Workouts.AddRange(workouts);
            context.SaveChanges();

            var workoutExercises = new WorkoutExercise[]
            {
                // Full Body Workout
                new WorkoutExercise {
                    Workout = workouts[0],
                    Exercise = exercises[0], // Bench Press
                    Sets = 3, Reps = 10, WeightLbs = 135
                },
                new WorkoutExercise {
                    Workout = workouts[0],
                    Exercise = exercises[1], // Squat
                    Sets = 4, Reps = 8, WeightLbs = 225
                },
                new WorkoutExercise {
                    Workout = workouts[0],
                    Exercise = exercises[2], // Deadlift
                    Sets = 3, Reps = 5, WeightLbs = 275
                },

                // Upper Body Workout
                new WorkoutExercise {
                    Workout = workouts[1],
                    Exercise = exercises[0], // Bench Press
                    Sets = 5, Reps = 5, WeightLbs = 155
                },
                new WorkoutExercise {
                    Workout = workouts[1],
                    Exercise = exercises[4], // Shoulder Press
                    Sets = 4, Reps = 8, WeightLbs = 95
                },
                new WorkoutExercise {
                    Workout = workouts[1],
                    Exercise = exercises[3], // Pull-Up
                    Sets = 3, Reps = 10, WeightLbs = null
                },

                // Leg Day
                new WorkoutExercise {
                    Workout = workouts[2],
                    Exercise = exercises[1], // Squat
                    Sets = 5, Reps = 5, WeightLbs = 245
                },
                new WorkoutExercise {
                    Workout = workouts[2],
                    Exercise = exercises[6], // Leg Press
                    Sets = 3, Reps = 12, WeightLbs = 360
                },
                new WorkoutExercise {
                    Workout = workouts[2],
                    Exercise = exercises[2], // Deadlift
                    Sets = 3, Reps = 5, WeightLbs = 295
                }
            };

            context.WorkoutExercises.AddRange(workoutExercises);
            context.SaveChanges();
        }
    }
}
