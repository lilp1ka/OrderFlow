using System.Text.Json;
using OrderFlow.Models;

namespace OrderFlow.Services;

public class InboxWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly OrderProcessor _processor;
    private readonly SemaphoreSlim _semaphore = new(2); // максимум 2 файла одновременно
    private readonly string _inboxPath;

    public InboxWatcher(string path, OrderProcessor processor)
    {
        _inboxPath = path;
        _processor = processor;

        Directory.CreateDirectory(path);

        _watcher = new FileSystemWatcher(path, "*.json");
        _watcher.Created += OnCreated;
        _watcher.EnableRaisingEvents = true;
    }

    private async void OnCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"[Watcher] New file: {e.Name}");

        await _semaphore.WaitAsync();

        try
        {
            await Task.Delay(300);

            var json = await File.ReadAllTextAsync(e.FullPath);

            var orders = JsonSerializer.Deserialize<List<Order>>(json);

            if (orders == null) return;

            foreach (var order in orders)
            {
                Console.WriteLine($"[Watcher] Processing order {order.Id}");
                _processor.ProcessOrder(order); // используем твой pipeline
            }

            var processedDir = Path.Combine(_inboxPath, "processed");
            Directory.CreateDirectory(processedDir);

            var newPath = Path.Combine(processedDir, Path.GetFileName(e.FullPath));
            File.Move(e.FullPath, newPath, true);

            Console.WriteLine($"[Watcher] moved to processed");
        }
        catch (Exception ex)
        {
            var failedDir = Path.Combine(_inboxPath, "failed");
            Directory.CreateDirectory(failedDir);

            var newPath = Path.Combine(failedDir, Path.GetFileName(e.FullPath));

            if (File.Exists(e.FullPath))
                File.Move(e.FullPath, newPath, true);

            await File.WriteAllTextAsync(newPath + ".error.txt", ex.Message);

            Console.WriteLine($"[Watcher] ERROR: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _semaphore.Dispose();
    }
}