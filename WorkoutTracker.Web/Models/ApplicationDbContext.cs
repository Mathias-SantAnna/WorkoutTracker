using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => we.WorkoutExerciseId);

            // Configure WorkoutExercise → Workout
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId);

            // Configure WorkoutExercise → Exercise
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId);
            
            modelBuilder.Entity<WorkoutExercise>()
                .Property(we => we.WeightLbs)
                .HasPrecision(5, 2);
        }
    }
}
