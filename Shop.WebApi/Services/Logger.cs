namespace Shop.WebApi.Services;

public static class Logger
{
    public static void Info(string message)
    {
        Console.WriteLine("Info: " + message);
    }

    public static void Error(string message)
    {
        Console.WriteLine("Error: " + message);
    }

    public static void Debug(string message)
    {
        Console.WriteLine("Debug: " + message);
    }
}