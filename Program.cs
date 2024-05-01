using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

class Country
{
    public string Name { get; set; }
    public double Odds { get; set; }
    public int TotalPoints { get; set; } = 0;
}

class EurovisionSimulator
{
    private List<Country> countries = new List<Country>();
    private Random random = new Random();

    public EurovisionSimulator()
    {
        AddCountry("France", 10);
        AddCountry("Germany", 15);
        AddCountry("Italy", 8);
        AddCountry("Spain", 12);
        AddCountry("United Kingdom", 5);
    }

    public void AddCountry(string name, double odds)
    {
        countries.Add(new Country { Name = name, Odds = odds });
    }

    public void Simulate()
    {
        foreach (var country in countries)
        {
            country.TotalPoints = CalculateJuryScore() + CalculateTelevoteScore();
        }

        Console.WriteLine("Eurovision Results:");
        Console.WriteLine("Country           | Jury Points | Televote Points | Total Points");
        Console.WriteLine("------------------+-------------+-----------------+-------------");
        foreach (var country in countries.OrderByDescending(c => c.TotalPoints))
        {
            Console.WriteLine($"{country.Name,-18} | {CalculateJuryScore(),-11} | {CalculateTelevoteScore(),-15} | {country.TotalPoints,-11}");
        }

        Country winner = countries.OrderByDescending(c => c.TotalPoints).First();
        Console.WriteLine($"\nThe winner is: {winner.Name}");
    }

    private int CalculateJuryScore()
    {
        int[] points = new int[] { 1, 2, 3, 4, 5, 6, 8, 10, 12 };
        points = points.OrderBy(p => random.Next()).ToArray();
        int count = Math.Min(countries.Count, 9);
        int score = 0;
        for (int i = 0; i < count; i++)
        {
            score += points[i];
        }
        return score;
    }

    private int CalculateTelevoteScore()
    {
        int[] points = new int[] { 1, 2, 3, 4, 5, 6, 8, 10, 12 };
        points = points.OrderBy(p => random.Next()).ToArray();
        int score = 0;
        foreach (var country in countries)
        {
            int count = Math.Min(countries.Count, 9);
            for (int i = 0; i < count; i++)
            {
                score += points[i];
            }
        }
        score /= countries.Count;
        return score;
    }

    public void ViewCurrentOdds()
    {
        Console.WriteLine("\nCurrent Odds:");
        foreach (var country in countries)
        {
            Console.WriteLine($"{country.Name}: {country.Odds:F2}");
        }
    }

    public void UpdateOdds(string countryName, double newOdds)
    {
        Country country = countries.Find(c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase));
        if (country != null)
        {
            country.Odds = newOdds;
            Console.WriteLine($"Updated odds for {country.Name} to {newOdds:F2}");
        }
        else
        {
            Console.WriteLine($"Country '{countryName}' not found.");
        }
    }

    public void RemoveCountry(string name)
    {
        Country countryToRemove = countries.Find(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (countryToRemove != null)
        {
            countries.Remove(countryToRemove);
            Console.WriteLine($"Removed country: {name}");
        }
        else
        {
            Console.WriteLine($"Country '{name}' not found.");
        }
    }

    public void ResetOddsToDefault()
    {
        foreach (var country in countries)
        {
            switch (country.Name)
            {
                case "France":
                    country.Odds = 10;
                    break;
                case "Germany":
                    country.Odds = 15;
                    break;
                case "Italy":
                    country.Odds = 8;
                    break;
                case "Spain":
                    country.Odds = 12;
                    break;
                case "United Kingdom":
                    country.Odds = 5;
                    break;
                default:
                    break;
            }
        }
        Console.WriteLine("Odds reset to default values.");
    }

    public void SaveState(string filePath)
    {
        try
        {
            string json = JsonConvert.SerializeObject(countries);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Current state saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving state: {ex.Message}");
        }
    }

    public void LoadState(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                countries = JsonConvert.DeserializeObject<List<Country>>(json);
                Console.WriteLine("Saved state loaded successfully.");
            }
            else
            {
                Console.WriteLine("Saved state file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading state: {ex.Message}");
            Console.WriteLine("The loaded state is not in the expected format.");
        }
    }

    public void DisplayMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. View current odds");
        Console.WriteLine("2. Update odds for a country");
        Console.WriteLine("3. Add a new country");
        Console.WriteLine("4. Remove a country");
        Console.WriteLine("5. Reset odds to default");
        Console.WriteLine("6. Simulate Eurovision");
        Console.WriteLine("7. Save current state");
        Console.WriteLine("8. Load saved state");
        Console.WriteLine("9. Exit");
    }
}

class Program
{
    static void Main(string[] args)
    {
        EurovisionSimulator simulator = new EurovisionSimulator();

        bool exit = false;
        while (!exit)
        {
            simulator.DisplayMenu();
            Console.WriteLine("\nEnter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    simulator.ViewCurrentOdds();
                    break;
                case "2":
                    Console.WriteLine("Enter country name: ");
                    string countryName = Console.ReadLine();
                    Console.WriteLine("Enter new odds: ");
                    double newOdds;
                    if (double.TryParse(Console.ReadLine(), out newOdds))
                    {
                        if (newOdds >= 0)
                        {
                            simulator.UpdateOdds(countryName, newOdds);
                        }
                        else
                        {
                            Console.WriteLine("Odds must be a non-negative number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid odds. Please enter a valid number.");
                    }
                    break;
                case "3":
                    Console.WriteLine("Enter new country name: ");
                    string newCountryName = Console.ReadLine();
                    Console.WriteLine("Enter odds for the new country: ");
                    double newCountryOdds;
                    if (double.TryParse(Console.ReadLine(), out newCountryOdds))
                    {
                        if (newCountryOdds >= 0)
                        {
                            simulator.AddCountry(newCountryName, newCountryOdds);
                        }
                        else
                        {
                            Console.WriteLine("Odds must be a non-negative number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid odds. Please enter a valid number.");
                    }
                    break;
                case "4":
                    Console.WriteLine("Enter country name to remove: ");
                    string countryToRemove = Console.ReadLine();
                    simulator.RemoveCountry(countryToRemove);
                    break;
                case "5":
                    simulator.ResetOddsToDefault();
                    break;
                case "6":
                    simulator.Simulate();
                    break;
                case "7":
                    Console.WriteLine("Enter file path to save state: ");
                    string saveFilePath = Console.ReadLine();
                    simulator.SaveState(saveFilePath);
                    break;
                case "8":
                    Console.WriteLine("Enter file path to load saved state: ");
                    string loadFilePath = Console.ReadLine();
                    simulator.LoadState(loadFilePath);
                    break;
                case "9":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
