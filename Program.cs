using System.Reflection.Metadata;

internal class Programm
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Работа с сайтом");
        string fileurl = "https://httpbin.org/image/jpeg";
        string savePath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
           "FileDownloader",
           "downloaded_image.jpg"  
          );
        using var httpclient = new HttpClient();
        var connector = new ConnectToSite(httpclient);
        connector.SetupBasicAuth("user", "passwd");
         if (await connector.TestAuth())
          {
            var connect = new  DownLoaderFiles(httpclient);
            await connect.DownloadFile(fileurl,savePath);
          }
    }
}
