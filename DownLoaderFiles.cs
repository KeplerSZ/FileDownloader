public class  DownLoaderFiles
{
    private readonly HttpClient _connect;
    private readonly FileLogger _fileLogger;
    public DownLoaderFiles(HttpClient connect, FileLogger fileLogger)
    {
        _connect = connect;
        _fileLogger = fileLogger;
    }
     
   
      public async Task DownloadLatestUpgradeFile(string baseUrl, string saveDirectory,string downloadBaseUrl)
    {
        try
        {
            _fileLogger.Log($"Получение списка файлов с: {baseUrl}");
            
            // Получаем HTML страницы
            string htmlContent = await _connect.GetStringAsync(baseUrl);
            
            // Находим самый свежий файл upgrade
            var latestFile = FindLatestUpgradeFile(htmlContent);
            
            if (latestFile == null)
            {
                _fileLogger.LogError("Не найдены файлы upgrade для скачивания");
                Console.WriteLine("Не найдены файлы upgrade");
                return;
            }
            if (!latestFile.StartsWith("-"))
                {
                    latestFile = "-" + latestFile;
                }
            Console.WriteLine($"Найден самый свежий файл: {latestFile}");
            Console.WriteLine($"Дата в имени: {ExtractDateFromFileName(latestFile):dd/MM/yyyy}");
            
            // Формируем полный URL и путь сохранения
            string fullUrl =  downloadBaseUrl + latestFile;
            string savePath = Path.Combine(saveDirectory, latestFile);
            if (File.Exists(savePath))
            {
                Console.WriteLine($"Файл уже скачан: {latestFile}");
                _fileLogger.Log($"Файл уже существует, пропускаю: {latestFile}");
                return; // Не скачиваем повторно
            }
            Console.WriteLine($"Найден новый файл: {latestFile}");
            Console.WriteLine($"Дата в имени: {ExtractDateFromFileName(latestFile):dd/MM/yyyy}");
            // Скачиваем файл
            await DownloadFile(fullUrl, savePath);
        }
        catch (Exception ex)
        {
            _fileLogger.LogError($"Ошибка при поиске файла: {ex.Message}");
        }
    }
    
    // Метод для поиска самого свежего файла upgrade
    private string FindLatestUpgradeFile(string htmlContent)
    {
        try
        {
            // Список для хранения найденных файлов upgrade
            var upgradeFiles = new List<string>();
            
            // Ищем все файлы с "upgrade" и ".rar" в имени
            // Разбиваем HTML по ссылкам или по имени файла
            int startIndex = 0;
            
            while (true)
            {
                // Ищем начало имени файла (формат :upgradeYYYYMMDD...)
                int fileStart = htmlContent.IndexOf("-upgrade", startIndex);
                if (fileStart == -1) break;
                
                // Ищем конец имени файла (.rar)
                int fileEnd = htmlContent.IndexOf(".rar", fileStart);
                if (fileEnd == -1) break;
                
                // Извлекаем полное имя файла
                string fileName = htmlContent.Substring(fileStart + 1, fileEnd - fileStart + 3); // +1 чтобы убрать двоеточие, +3 для ".rar"
                upgradeFiles.Add(fileName);
                
                // Перемещаемся дальше
                startIndex = fileEnd + 4;
            }
            
            if (upgradeFiles.Count == 0)
            {
                Console.WriteLine("Не найдено файлов upgrade");
                return null;
            }
            
            Console.WriteLine($"Найдено файлов: {upgradeFiles.Count}");
            foreach (var file in upgradeFiles)
            {
                Console.WriteLine($"  - {file}");
            }
            
            // Ищем самый свежий файл (с максимальной датой в имени)
            string latestFile = null;
            DateTime latestDate = DateTime.MinValue;
            
            foreach (var file in upgradeFiles)
            {
                DateTime fileDate = ExtractDateFromFileName(file);
                
                if (fileDate > latestDate)
                {
                    latestDate = fileDate;
                    latestFile = file;
                }
            }
            
            return latestFile;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при поиске файлов: {ex.Message}");
            return null;
        }
    }
    
    // Метод для извлечения даты из имени файла
    private DateTime ExtractDateFromFileName(string fileName)
    {
        try
        {
            // Имя файла: upgrade202511211610361.rar
            // Нужно извлечь: 20251121 (YYYYMMDD)
            
            // Находим "upgrade" и берем следующие 8 цифр (год+месяц+день)
            int upgradeIndex = fileName.IndexOf("upgrade");
            if (upgradeIndex == -1) return DateTime.MinValue;
            
            string datePart = fileName.Substring(upgradeIndex + 7, 8); // 7 = длина "upgrade"
            
            // Парсим дату: 20251121 -> 2025-11-21
            if (DateTime.TryParseExact(datePart, "yyyyMMdd", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            
            return DateTime.MinValue;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
     public async Task DownloadFile(string fileurl, string savePath)
    {
        try
        {
            _fileLogger.Log($"Начало загрузки файла: {fileurl}");
            byte[] fileData = await _connect.GetByteArrayAsync(fileurl);
            await File.WriteAllBytesAsync(savePath, fileData);
            _fileLogger.Log($"Файл сохранён: {savePath}");
            
            FileInfo fileInfo = new FileInfo(savePath);
            Console.WriteLine($"Размер файла: {fileInfo.Length} байт ({fileInfo.Length / 1024 / 1024} MB)");
        }
        catch (Exception ex)
        {
            _fileLogger.LogError($"Ошибка загрузки файла: {ex.Message}");
        }
    }
}