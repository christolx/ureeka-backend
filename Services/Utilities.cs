namespace ureeka_backend.Services;

static public class Utilities
{
    public static string BuildConnectionString(string databaseUrl)
    {
        if (string.IsNullOrEmpty(databaseUrl))
        {
            Console.WriteLine("DATABASE_URL is null or empty");
            return null;
        }
    
        Console.WriteLine("Parsing connection string...");
    
        try
        {
            string pattern = @"postgres:\/\/(.*):(.*)@(.*):(\d+)\/(.*)";
            var match = System.Text.RegularExpressions.Regex.Match(databaseUrl, pattern);
        
            if (!match.Success)
            {
                Console.WriteLine("Failed to match connection string pattern");
                return null;
            }
        
            var username = match.Groups[1].Value;
            var password = match.Groups[2].Value;
            var host = match.Groups[3].Value;
            var port = match.Groups[4].Value;
            var database = match.Groups[5].Value;
            
            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
        
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Host: {host}");
            Console.WriteLine($"Port: {port}");
            Console.WriteLine($"Database: {database}");
            Console.WriteLine($"Connection string created (password hidden)");
        
            return connectionString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
            return null;
        }
    }
}