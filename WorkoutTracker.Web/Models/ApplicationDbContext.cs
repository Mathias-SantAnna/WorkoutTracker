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
            // Configure the primary key
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => we.WorkoutExerciseId);

            // Configure WorkoutExercise → Workout (many-to-one)
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId);

            // Configure WorkoutExercise → Exercise (many-to-one)
            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId);

            // Set precision for decimal property to avoid SQL Server warning
            modelBuilder.Entity<WorkoutExercise>()
                .Property(we => we.WeightLbs)
                .HasPrecision(5, 2); // e.g., allows up to 999.99
        }
    }
}
