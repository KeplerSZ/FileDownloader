public class FileLogger
{
    private readonly string  _logFileName;
    private readonly string _logDirectory;
    private readonly object _lock = new object();
    public FileLogger (string basePath = null)
    {
        _logDirectory = Path.Combine( 
            basePath ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
             "FileWondloader",
             "logs");

        Directory.CreateDirectory(_logDirectory);

        _logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
    }
    private string GetLogFilePath() => Path.Combine(_logDirectory, _logFileName);
    public void Log(string message)
    {
        LogInternal("INFO", message);
    }
     public void LogError(string message)
        {
            LogInternal("ERROR", message);
        }

        private void LogInternal(string level, string message)
        {
            lock (_lock)
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
                File.AppendAllText(GetLogFilePath(), logEntry);
            }
        }
}