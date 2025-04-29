# Workout Tracker - C# & .NET 8
CA2 Mini Project for Enterprise Computing and Digital Transformation - TUD

## Overview
This is a fitness tracker application built using C#, .NET 8, and Azure SQL. The application allows users to track their workouts, exercises, and monitor their progress over time.

## Tech Stack
- **Backend**: ASP.NET Core 8 MVC
- **Database**: Azure SQL Server
- **API**: RESTful Web API with Swagger
- **Client**: ASP.NET Core MVC Web Application + Console Client for Analysis
- **Cloud Deployment**: Microsoft Azure
- **Version Control**: GitHub

## Features
- **Workout Management**:
  - Create, view, edit, and delete workout sessions
  - Track workout date, duration, and description
  - Associate multiple exercises with each workout
- **Exercise Management**:
  - Maintain a library of exercises with names, descriptions, and target muscles
  - Track sets, reps, and weights for each exercise in a workout
  - Monitor progress over time
- **Data Analysis**:
  - RESTful API for workout data analysis
  - Most popular exercises tracking
  - Workout statistics (counts, durations, averages)
  - Exercise progress monitoring
- **Responsive Design**:
  - Clean, modern interface built with Bootstrap
  - Fully responsive design for mobile and desktop use
- **Cloud Deployment**:
  - Deployed on Azure App Service with CI/CD integration
  - Azure SQL Database for reliable, scalable data storage
  - Configured for high availability and security

## Architecture
The project follows a standard MVC (Model-View-Controller) pattern with:
- **Models**: Exercise, Workout, WorkoutExercise
- **Controllers**: For managing CRUD operations
- **Views**: For user interaction and data presentation
- **API Controllers**: For analysis and data consumption

## Setup Instructions
### 1. Clone the repository:
```sh
git clone https://github.com/yourusername/workout-tracker.git
cd workout-tracker
```

### 2. Configure the Azure SQL Database
- **Connection String**:
  - The application is configured to use Azure SQL Database
  - Update the connection string in `appsettings.json` if needed:
```md
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:severworkout.database.windows.net,1433;Initial Catalog=Workouttracker;Persist Security Info=False;User ID=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
  ```

### 3. Run the web application:
```sh
dotnet run --project WorkoutTracker.Web
```

### 4. Run the analysis console client:
```sh
dotnet run --project WorkoutTracker.ConsoleClient
```

### 5. API Access
- Swagger UI is available in development mode at `/swagger`
- API endpoints:
  - GET `/api/status` - Check API health status
  - GET `/api/exercises` - Get all exercises
  - GET `/api/exercises/{id}` - Get a specific exercise by ID

## Deployment
The application is deployed to Azure App Service using the following steps:
1. Create an Azure App Service
2. Configure the Azure SQL Server firewall to allow connections
3. Deploy the application using Visual Studio, Rider, or Azure DevOps pipelines
4. Set up connection strings and application settings in Azure

## Azure Resources
- **App Service**: workouttracker-akg5c7fhg3beggcg.northeurope-01.azurewebsites.net
- **SQL Server**: severworkout.database.windows.net
- **Database**: Workouttracker

## Database Schema
The application uses a relational database with the following main tables:
- **Exercises**: Store exercise information
- **Workouts**: Store workout sessions
- **WorkoutExercises**: Junction table for the many-to-many relationship

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you'd like to change.

## License
MIT License