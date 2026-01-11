using System.Text;

public class ConnectToSite
{
    private readonly HttpClient _httpClient;
    private readonly FileLogger _logger;
    public ConnectToSite(HttpClient connect,FileLogger logger)
    {
        _httpClient = connect;
        _logger = logger;
    }
    public void SetupBasicAuth(string username, string password)
    {
        _logger.Log("Начат процесс логирования");
        string loginAndPassword = $"{username}:{password}";
        byte[] bytes = Encoding.ASCII.GetBytes(loginAndPassword);
        string base64 = Convert.ToBase64String(bytes);
        _httpClient.DefaultRequestHeaders.Authorization = 
        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);
        _logger.Log("Аутентификация успешна");

    }
    public async Task<bool> TestAuth()
{
    try
    {
        // Используем тестовый URL с httpbin.org
        string testUrl = "https://ric.consultant.ru";
        
        // Отправляем запрос
        HttpResponseMessage response = await _httpClient.GetAsync(testUrl);
        
        // Смотрим статус
        Console.WriteLine($"Статус ответа: {response.StatusCode}");
        
        // Читаем ответ (для отладки)
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Тело ответа: {responseBody}");
        
        // 200 OK = авторизация работает
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при проверке: {ex.Message}");
        return false;
    }
}
}