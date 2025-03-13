namespace ureeka_backend.Services;

static public class Utilities
{
    public static string BuildConnectionString(string databaseUrl)
    {
        // Parse components from the URL format
        var databaseUri = new Uri(databaseUrl);
    
        var userInfo = databaseUri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo[1];
        var host = databaseUri.Host;
        var port = databaseUri.Port;
        var database = databaseUri.LocalPath.TrimStart('/');
    
        // Build the connection string for Entity Framework
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
}