using System.Reflection.Metadata;

internal class Programm
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Работа с сайтом");
        var logger = new FileLogger();
        string fileurl = "https://ric.consultant.ru/materials/?path=Soft";
        //string fileurl = "https://ric.consultant.ru/materials/Download";
        // Базовый URL для скачивания (ОБРАТИТЕ ВНИМАНИЕ!)
        string downloadBaseUrl = "https://ric.consultant.ru/materials/Download/";
        string savePath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
           "FileDownloader",
           "Downloads"  
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
        var connector = new ConnectToSite(httpclient,logger);
        connector.SetupBasicAuth("", "");
        logger.Log("Запуск скачки");
         if (await connector.TestAuth())
          {
            var downloader  = new  DownLoaderFiles(httpclient,logger);
            //await downloader.DownloadFile(fileurl,savePath);
            await downloader.DownloadLatestUpgradeFile(fileurl,savePath,downloadBaseUrl);
            System.Console.WriteLine("Works");
          }
    }
}
