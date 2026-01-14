using FileDownloader;
using System.Reflection.Metadata;
using System.Runtime;

internal class Programm
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Работа с сайтом");
        var settings = LoadSettings("settingsclass.json");
        var logger = new FileLogger();
        //string fileurl = "https://ric.consultant.ru/materials/?path=Soft";
        string fileurl = settings.ConsultantSettings.fileurl;
        // Базовый URL для скачивания
        string downloadBaseUrl = settings.ConsultantSettings.downloadBaseUrl;
        
        // string savePath = Path.Combine(
        //   Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        //    "FileDownloader",
        //    "Downloads"  
        //   );
        string savePath = Path.Combine(
           Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            settings.DownloadSettings.DownloadFolder
             
           );
         // Создаем папку, если не существует
        if (!Directory.Exists(savePath))
        {
            Console.WriteLine($"Создаю папку: {savePath}");
            Directory.CreateDirectory(savePath);
        }
        else
        {
            Console.WriteLine($"Папка существует: {savePath}");
        }
        
        // Проверяем права
        try
        {
            string testFile = Path.Combine(savePath, "test.txt");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            Console.WriteLine("Права на запись: OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка прав доступа: {ex.Message}");
        } 
        using var httpclient = new HttpClient();
        // var connector = new ConnectToSite(httpclient,logger);
        // connector.SetupBasicAuth("ric380zsa", "exh23m7q");
        var connector = new ConnectToSite(httpclient,logger);
        connector.SetupBasicAuth(settings.ConsultantSettings.Username, settings.ConsultantSettings.Password );
        logger.Log("Запуск скачки");
         if (await connector.TestAuth())
          {
            var downloader  = new  DownLoaderFiles(httpclient,logger);
            //await downloader.DownloadFile(fileurl,savePath);
            await downloader.DownloadLatestUpgradeFile(fileurl,savePath,downloadBaseUrl);
            System.Console.WriteLine("Works");
          }
    }
    static Settings LoadSettings(string json) {
    
        string jsonText = File.ReadAllText(json);
        var settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(jsonText);
        
        return settings;
    }
}
