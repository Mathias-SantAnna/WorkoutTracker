using System.Net.Http.Json;

namespace WorkoutTracker.ConsoleClient
{
    class Program
    {
        // API base URL - change this as needed
        private static readonly string ApiBaseUrl = "https://workouttracker-api.azurewebsites.net/api";
        private static readonly HttpClient Client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Workout Tracker - Analysis Client");
            Console.WriteLine("=================================");
            
            while (true)
            {
                try

                {
                    DisplayMenu();
                    string choice = Console.ReadLine();
                    
                    switch (choice)
                    {
                        case "1":
                            await GetPopularExercises();
                            break;
                        case "2":
                            await GetWorkoutStats();
                            break;
                        case "3":
                            await GetRecentWorkouts();
                            break;
                        case "4":
                            await GetExerciseProgress();
                            break;
                        case "5":
                            Console.WriteLine("\nExiting...");
                            return;
                        default:
                            Console.WriteLine("\nInvalid choice. Please try again.");
                            break;
                    }
                    
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
        
        static void DisplayMenu()
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. View Most Popular Exercises");
            Console.WriteLine("2. View Workout Statistics");
            Console.WriteLine("3. View Recent Workouts");
            Console.WriteLine("4. View Exercise Progress");
            Console.WriteLine("5. Exit");
            Console.Write("\nEnter your choice (1-5): ");
        }
        
        static async Task GetPopularExercises()
        {
            Console.WriteLine("\nFetching most popular exercises...");
            
            string apiUrl = $"{ApiBaseUrl}/workoutanalysis/popular";
            var exercises = await Client.GetFromJsonAsync<List<ExerciseUsage>>(apiUrl);
            
            Console.WriteLine("\nMost Popular Exercises:");
            Console.WriteLine("----------------------");
            
            if (exercises != null && exercises.Count > 0)
            {
                foreach (var exercise in exercises)
                {
                    Console.WriteLine($"{exercise.Exercise}: Used {exercise.Count} times");
                }
            }
            else
            {
                Console.WriteLine("No exercise data found.");
            }
        }
        
        static async Task GetWorkoutStats()
        {
            Console.WriteLine("\nFetching workout statistics...");
            
            string apiUrl = $"{ApiBaseUrl}/workoutanalysis/stats";
            var stats = await Client.GetFromJsonAsync<WorkoutStats>(apiUrl);
            
            Console.WriteLine("\nWorkout Statistics:");
            Console.WriteLine("------------------");
            Console.WriteLine($"Total Workouts: {stats.TotalWorkouts}");
            Console.WriteLine($"Total Exercises: {stats.TotalExercises}");
            Console.WriteLine($"Total Duration: {stats.TotalDurationMinutes} minutes");
            Console.WriteLine($"Average Duration: {stats.AverageDurationMinutes} minutes per workout");
        }
        
        static async Task GetRecentWorkouts()
        {
            Console.WriteLine("\nFetching recent workouts...");
            
            Console.Write("How many recent workouts to display? (default 5): ");
            string input = Console.ReadLine();
            int count = string.IsNullOrEmpty(input) ? 5 : int.Parse(input);
            
            string apiUrl = $"{ApiBaseUrl}/workoutanalysis/recent?count={count}";
            var workouts = await Client.GetFromJsonAsync<List<RecentWorkout>>(apiUrl);
            
            Console.WriteLine("\nRecent Workouts:");
            Console.WriteLine("---------------");
            
            if (workouts != null && workouts.Count > 0)
            {
                foreach (var workout in workouts)
                {
                    Console.WriteLine($"{workout.WorkoutDate.ToShortDateString()} - {workout.Name} ({workout.DurationMinutes} min, {workout.ExerciseCount} exercises)");
                }
            }
            else
            {
                Console.WriteLine("No recent workouts found.");
            }
        }
        
        static async Task GetExerciseProgress()
        {
            Console.WriteLine("\nFetching exercises...");
            
            // First get all exercises to let user choose one
            string exercisesUrl = $"{ApiBaseUrl}/exercises";
            var exercises = await Client.GetFromJsonAsync<List<Exercise>>(exercisesUrl);            
            if (exercises == null || exercises.Count == 0)
            {
                Console.WriteLine("No exercises found.");
                return;
            }
            
            Console.WriteLine("\nAvailable Exercises:");
            for (int i = 0; i < exercises.Count; i++)
            {
                Console.WriteLine($"{i+1}. {exercises[i].Name}");
            }
            
            Console.Write("\nSelect an exercise (enter number): ");
            if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 1 || selection > exercises.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }
            
            var selectedExercise = exercises[selection - 1];
            
            Console.WriteLine($"\nFetching progress for {selectedExercise.Name}...");
            
            string progressUrl = $"{ApiBaseUrl}/workoutanalysis/progress/{selectedExercise.ExerciseId}";
            var progress = await Client.GetFromJsonAsync<ExerciseProgressResponse>(progressUrl);
            
            Console.WriteLine($"\nProgress for {progress.Exercise}:");
            Console.WriteLine("-------------------------");
            
            if (progress.ProgressData != null && progress.ProgressData.Count > 0)
            {
                foreach (var entry in progress.ProgressData)
                {
                    Console.WriteLine($"{entry.Date.ToShortDateString()}: {entry.Sets} sets x {entry.Reps} reps @ {entry.WeightLbs} lbs");
                }
                
                // Calculate improvement
                var firstEntry = progress.ProgressData.First();
                var lastEntry = progress.ProgressData.Last();
                decimal? firstWeight = firstEntry.WeightLbs;
                decimal? lastWeight = lastEntry.WeightLbs;
                
                if (firstWeight.HasValue && lastWeight.HasValue)
                {
                    decimal improvement = lastWeight.Value - firstWeight.Value;
                    decimal percentImprovement = firstWeight.Value > 0 ? (improvement / firstWeight.Value) * 100 : 0;
                    
                    Console.WriteLine($"\nWeight Improvement: {improvement} lbs ({percentImprovement:F1}%)");
                }
            }
            else
            {
                Console.WriteLine("No progress data found for this exercise.");
            }
        }
    }
    
    public class ExerciseUsage
    {
        public string Exercise { get; set; }
        public int Count { get; set; }
    }

    
    public class WorkoutStats
    {
        public int TotalWorkouts { get; set; }
        public int TotalExercises { get; set; }
        public int TotalDurationMinutes { get; set; }
        public int AverageDurationMinutes { get; set; }
    }
    
    public class RecentWorkout
    {
        public int WorkoutId { get; set; }
        public string Name { get; set; }
        public DateTime WorkoutDate { get; set; }
        public int DurationMinutes { get; set; }
        public int ExerciseCount { get; set; }
    }
    
    public class Exercise
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; }
    }
    
    public class ExerciseProgressEntry
    {
        public DateTime Date { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightLbs { get; set; }
    }
    
    public class ExerciseProgressResponse
    {
        public string Exercise { get; set; }
        public List<ExerciseProgressEntry> ProgressData { get; set; }
    }
}
