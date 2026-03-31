namespace Singleton;

/// <summary>
/// CREATIONAL Pattern
/// </summary>
public class Logger
{
    /// <summary>
    /// Lazy ensures thread safety
    /// </summary>
    private static readonly Lazy<Logger> _lazyLogger = new Lazy<Logger>(() => new Logger());

    public static Logger Instance
    {
        get { return _lazyLogger.Value; }
    }

    protected Logger() { }

    public void Log(string message)    
    {
        Console.WriteLine($"Log: {message}");
    }
}
