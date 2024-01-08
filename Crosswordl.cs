using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Globalization;

class Crosswordl
{
    static string CrosswordPath = Path.Combine(Environment.SpecialFolder.MyDocuments.ToString(), "Crosswords");
    static async Task Main(string[] args)
    {
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        Directory.CreateDirectory(CrosswordPath);
        DeleteOldFiles();
        await DownloadFileAsync(currentDate);
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
                OpenPDF(currentDate);
        };
        Thread.Sleep(10000);
    }

    static void OpenPDF(string currentDate)
    {
        using Process process = new Process();
        process.StartInfo = new ProcessStartInfo()
        {
            UseShellExecute = true,
            FileName = Path.Combine(CrosswordPath, currentDate + ".pdf")
        };
        process.Start();
    }

    static void DeleteOldFiles()
    {
        string[] files = Directory.GetFiles(CrosswordPath);
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            DateTime fileDate = DateTime.ParseExact(fileName, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (fileDate < DateTime.Now.AddDays(-7))
            {
                File.Delete(file);
            }
        }
    }

    static async Task DownloadFileAsync(string currentDate)
    {

        try
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://www.onlinecrosswords.net/printable-daily-crosswords-1.pdf");
            response.EnsureSuccessStatusCode();
            string filePath = Path.Combine(CrosswordPath, currentDate + ".pdf");
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            await response.Content.CopyToAsync(fileStream);
            new ToastContentBuilder()
            .AddText("Crossword")
            .AddText("Crossword downloaded")
            .Show();
        }
        catch
        (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}



