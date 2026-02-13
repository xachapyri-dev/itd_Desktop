using System.Diagnostics;

Console.WriteLine(@"");
Console.WriteLine(@"    ██╗ ████████╗██████╗     ███╗   ██╗ ██████╗ ██╗   ██╗██╗  ██╗██╗");
Console.WriteLine(@"    ██║ ╚══██╔══╝██╔══██╗    ████╗  ██║██╔═══██╗██║   ██║██║ ██╔╝██║");
Console.WriteLine(@"    ██║    ██║   ██║  ██║    ██╔██╗ ██║██║   ██║██║   ██║█████╔╝ ██║");
Console.WriteLine(@"    ██║    ██║   ██║  ██║    ██║╚██╗██║██║   ██║██║   ██║██╔═██╗ ██║");
Console.WriteLine(@"    ██║    ██║   ██████╔╝    ██║ ╚████║╚██████╔╝╚██████╔╝██║  ██╗██║");
Console.WriteLine(@"    ╚═╝    ╚═╝   ╚═════╝     ╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚═╝");
Console.WriteLine(@"");

string i = "====================";
Console.WriteLine(i + "Очистка" + i);
string processName = "ИТД Desktop";
try
{
    // Получаем все процессы с указанным именем+
    Process[] processes = Process.GetProcessesByName(processName);
    if (processes.Length > 0)
    {
        // Перебираем все найденные процессы
        foreach (Process process in processes)
        {
            try
            {
                // Пытаемся корректно закрыть процесс
                if (!process.HasExited)
                {
                    process.CloseMainWindow();

                    // Ждем нормального закрытия (5 секунд)
                    if (!process.WaitForExit(5000))
                    {
                        // Если процесс не закрылся, принудительно завершаем
                        Console.WriteLine($"Принудительное завершение процесса {process.Id}...");
                        process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при закрытии процесса {process.Id}: {ex.Message}");
            }
            finally
            {
                // Освобождаем ресурсы
                if (process != null)
                    process.Dispose();
            }
        }
        // Дополнительная проверка и ожидание
        Thread.Sleep(2000); // Даем системе время на освобождение ресурсов
        // Проверяем, остались ли процессы
        processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            Console.WriteLine($"Процесс {processName}.exe успешно закрыт.");
            // Удаляем папку после закрытия процесса
            DeleteWebView2Folder();
        }
        else
        {
            Console.WriteLine($"Предупреждение: найдено {processes.Length} процессов после попытки закрытия.");
        }
    }
    else
    {
        Console.WriteLine($"Процесс {processName}.exe не запущен.");
        // Даже если процесс не был запущен, пытаемся удалить папку
        DeleteWebView2Folder();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при работе с процессами: {ex.Message}");
}
Console.WriteLine(i+"Очистка завершена."+i);
Console.WriteLine("Preess any key to exit");
Console.ReadKey();
void DeleteWebView2Folder()
{
    string folderPath = ""; // Объявляем переменную здесь
    try
    {
        // Папка для удаления (в текущей директории)
        string folderName = "ИТД Desktop.exe.WebView2";
        folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        // Проверяем существование папки
        if (Directory.Exists(folderPath))
        {
            Console.WriteLine($"Найдена папка {folderName}. Удаляю...");
            // Сначала снимаем атрибуты "только для чтения" если есть
            RemoveReadOnlyAttributes(folderPath);
            // Рекурсивное удаление папки
            Directory.Delete(folderPath, true);
            Console.WriteLine($"Папка {folderName} успешно удалена.");
        }
        else
        {
            Console.WriteLine($"Папка {folderName} не найдена в текущей директории.");
        }
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Ошибка доступа: {ex.Message}");
        Console.WriteLine("Попробуйте запустить программу от имени администратора.");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"Ошибка ввода-вывода: {ex.Message}");
        Console.WriteLine("Возможно, файлы все еще используются или заблокированы.");
        // Попробуем еще раз через небольшую задержку
        Thread.Sleep(1000);
        try
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Console.WriteLine($"Папка удалена при повторной попытке.");
            }
        }
        catch (Exception retryEx)
        {
            Console.WriteLine($"Не удалось удалить папку даже при повторной попытке: {retryEx.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Неожиданная ошибка при удалении папки: {ex.Message}");
    }
}
void RemoveReadOnlyAttributes(string directoryPath)
{
    try
    {
        // Рекурсивно проходим по всем файлам
        foreach (string file in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }
            }
            catch { }
        }
    }
    catch { }
}
