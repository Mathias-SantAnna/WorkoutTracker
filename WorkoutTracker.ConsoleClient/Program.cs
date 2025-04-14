using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WorkoutTracker.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Workout Tracker - Analysis Client");
            Console.WriteLine("=================================");
            
            try
            {
                Console.WriteLine("\nFetching most popular exercises...");
                
                string apiUrl = "https://localhost:7117/api/workoutanalysis/popular";
                
                using HttpClient client = new HttpClient();
                var exercises = await client.GetFromJsonAsync<List<PopularExercise>>(apiUrl);
                
                Console.WriteLine("\nMost Popular Exercises:");
                Console.WriteLine("-----");
                
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
    
    public class PopularExercise
    {
        public string Exercise { get; set; }
        public int Count { get; set; }
    }
}
