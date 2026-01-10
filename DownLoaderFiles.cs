public class  DownLoaderFiles
{
    private readonly HttpClient _connect;
    public DownLoaderFiles(HttpClient connect)
    {
        _connect = connect;
    }
     
   
    public async Task DownloadFile(string fileurl,string savePath){
        
        try{
        
        byte[] imageDate = await _connect.GetByteArrayAsync(fileurl);

        await File.WriteAllBytesAsync(savePath,imageDate);
        //Проверка размера 
        FileInfo fileInfo = new FileInfo(savePath);
        System.Console.WriteLine($"Размер файла: {fileInfo.Length} байт");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}