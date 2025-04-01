﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackerService;

public class WorkoutPlan
{
    public Dictionary<int, WorkoutSession> workoutSessions = new();
    public string filePath = "workoutPlan.json";
    public bool isImported { get; private set; } = false;

    public void createWorkoutPlan(UserPreferences userPreferences)
    {
        for (int i = 0; i < userPreferences.daysAvailable.Count; i++)
        {
            var index = i % userPreferences.timesAvailable.Count;
            var session = new WorkoutSession(i, userPreferences.daysAvailable[i], userPreferences.timesAvailable[index], userPreferences.workoutTypes, userPreferences.workoutGoal);
            workoutSessions[i] = session;
        }
    }

    public void displayWorkoutPlan()
    {
        foreach (var session in workoutSessions.Values)
        {
            Console.WriteLine(session.displaySession());
        }
    }

    public void exportWorkoutPlan()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        File.WriteAllText(filePath, JsonSerializer.Serialize(workoutSessions, options));
        Console.WriteLine("\nWorkout plan saved to disk.");
    }

    public static WorkoutPlan loadWorkoutPlan(string filePath = "workoutPlan.json")
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var sessions = JsonSerializer.Deserialize<Dictionary<int, WorkoutSession>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (sessions == null)
                {
                    Console.WriteLine("Failed to load workout plan. Returning empty workout plan.");
                    return new WorkoutPlan();
                }

                Console.WriteLine("Workout plan loaded from file.");
                return new WorkoutPlan { 
                    workoutSessions = sessions,
                    isImported = true };
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error reading workout plan: {exception.Message}");
                return new WorkoutPlan();
            }
        }
        else
        {
            return new WorkoutPlan();
        }
    }
}


