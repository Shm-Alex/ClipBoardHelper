using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ClipBoardHelper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private static readonly List<string> _dataStore = new();
    private readonly string _dataDirectory;
    public DataController(IWebHostEnvironment environment)
    {
        // Убедимся, что папка для данных существует
        _dataDirectory = Path.Combine(environment.ContentRootPath, "AppData");
        Directory.CreateDirectory(_dataDirectory);
    }

    /// <summary>
    /// Получить все данные
    /// </summary>
    [HttpGet("Data")]
    public async Task<IActionResult> GetData()
    {
        var filePath = Path.Combine(_dataDirectory, "Request.html");

        if (!System.IO.File.Exists(filePath))
        {
            return Content("", "text/html", Encoding.UTF8);
        }

        try
        {
            var content = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return Content(content, "text/html", Encoding.UTF8);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to read Request  file", details = ex.Message });
        }
        
    }

    /// <summary>
    /// Сохранить данные как Request.html
    /// </summary>
    /// <remarks>
    /// Ожидается POST-запрос с телом в виде необработанного текста (например, HTML).
    /// Поддерживаемые Content-Type: <c>text/html</c> или <c>text/plain</c>.
    /// Тело запроса сохраняется в файл <c>Request.html</c>.
    /// Если содержимое отличается от текущего — создаётся резервная копия с временной меткой.
    /// </remarks>
    /// <response code="200">Данные успешно сохранены</response>
    /// <response code="400">Тело запроса пустое или Content-Type не поддерживается</response>
    /// <response code="500">Ошибка при записи файла</response>
    [HttpPost("Data")]
    [Consumes("text/html", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostData()
    {
        // Проверка Content-Type (опционально, но улучшает UX)
        var contentType = Request.ContentType?.Split(';')[0].Trim().ToLowerInvariant();
        if (contentType != null && contentType != "text/html" && contentType != "text/plain")
        {
            return BadRequest("Unsupported Content-Type. Only 'text/html' or 'text/plain' are allowed.");
        }

        // Чтение тела запроса как UTF-8 строки
        string data;
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            data = await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to read request body: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            return BadRequest("Request body cannot be empty or whitespace.");
        }

        var filePath = Path.Combine(_dataDirectory, "Request.html");

        // Чтение текущего содержимого (если файл существует)
        string existingContent = "";
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                existingContent = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to read existing file: {ex.Message}");
            }
        }

        // Сравнение и сохранение с резервной копией при изменении
        if (!string.Equals(existingContent, data, StringComparison.Ordinal))
        {
            if (!string.IsNullOrEmpty(existingContent))
            {
                try
                {
                    var backupName = $"Request_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.html";
                    var backupPath = Path.Combine(_dataDirectory, backupName);
                    System.IO.File.Move(filePath, backupPath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Failed to create backup: {ex.Message}");
                }
            }

            try
            {
                await System.IO.File.WriteAllTextAsync(filePath, data, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to write file: {ex.Message}");
            }
        }

        return Ok(new { message = "Data saved successfully", savedTo = "Request.html" });
    }

    /// <summary>
    /// Получить содержимое буфера обмена из файла clipboard.txt
    /// </summary>
    [HttpGet("ClipBoard")]
    public async Task<IActionResult> GetClipBoard()
    {
        var filePath = Path.Combine(_dataDirectory, "clipboard.txt");

        if (!System.IO.File.Exists(filePath))
        {
            return Content("", "text/html", Encoding.UTF8);
        }

        try
        {
            var content = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            return Content(content, "text/html", Encoding.UTF8);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to read clipboard file", details = ex.Message });
        }
    }
    /// <summary>
    /// Сохранить данные в ClipBoard
    /// </summary>
    /// <remarks>
    /// Ожидается POST-запрос с телом в виде необработанного текста (например, строка для буфера обмена).
    /// Поддерживаемый Content-Type: <c>text/plain</c>.
    /// Тело запроса сохраняется в файл <c>clipboard.txt</c>.
    /// Если содержимое отличается от текущего — создаётся резервная копия с временной меткой.
    /// </remarks>
    /// <response code="200">Данные успешно сохранены</response>
    /// <response code="400">Тело запроса пустое или Content-Type не поддерживается</response>
    /// <response code="500">Ошибка при чтении/записи файла</response>
    [HttpPost("ClipBoard")]
    [Consumes("text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostClipBoard()
    {
        // Проверка Content-Type
        var contentType = Request.ContentType?.Split(';')[0].Trim().ToLowerInvariant();
        if (contentType != null && contentType != "text/plain")
        {
            return BadRequest("Unsupported Content-Type. Only 'text/plain' is allowed for clipboard data.");
        }

        // Чтение тела запроса как UTF-8 строки
        string data;
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            data = await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to read request body: {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            return BadRequest("Clipboard data cannot be empty or whitespace.");
        }

        var filePath = Path.Combine(_dataDirectory, "clipboard.txt");

        // Чтение текущего содержимого (если файл существует)
        string existingContent = "";
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                existingContent = await System.IO.File.ReadAllTextAsync(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to read existing clipboard file: {ex.Message}");
            }
        }

        // Сравнение и сохранение с резервной копией при изменении
        if (!string.Equals(existingContent, data, StringComparison.Ordinal))
        {
            if (!string.IsNullOrEmpty(existingContent))
            {
                try
                {
                    var backupName = $"clipboard_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.txt";
                    var backupPath = Path.Combine(_dataDirectory, backupName);
                    System.IO.File.Move(filePath, backupPath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Failed to create clipboard backup: {ex.Message}");
                }
            }

            try
            {
                await System.IO.File.WriteAllTextAsync(filePath, data, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to write clipboard file: {ex.Message}");
            }
        }

        return Ok(new { message = "Clipboard data saved successfully", savedTo = "clipboard.txt" });
    }

}