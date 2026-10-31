using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleStoreApp
{
    public static class DataManager
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public static void SaveData<T>(string filePath, List<T> data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, jsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to '{filePath}': {ex.Message}");
            }
        }

        public static List<T> LoadData<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<T>();
                }

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<T>>(json, jsonOptions) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from '{filePath}': {ex.Message}");
                return new List<T>();
            }
        }
    }
}