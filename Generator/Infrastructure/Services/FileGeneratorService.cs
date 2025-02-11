using System.Collections.Concurrent;

namespace Generator.Infrastructure.Services;

public static class FileGeneratorService
{
    public static async Task GenerateFileAsync(string fileName, int fileSizeInMb)
    {
        File.Delete(fileName); // I found that sometimes it hangs if I do not delete the file beforehand

        var random = new ThreadLocal<Random>(() => new Random());
        var companies = new List<string>
            { "Apple", "Microsoft", "Google", "Facebook", "Amazon", "Tesla", "Netflix", "IBM", "Intel", "Oracle" };

        var maxThreads = Environment.ProcessorCount;
        var tempPath = Path.GetTempPath();
        var tempFiles =
            new ConcurrentBag<string>(); // ConcurrentBag is like a List but thread-safe, I do not know exact spec but I know it should be used in parallel scenarios

        // Generate temp files in parallel, based on number of CPU cores
        // I do not think adding more jobs than cores would be beneficial, but I might be wrong
        // In reality it should be asked to GPT/Internet and tested

        // From my limited tests when I increase maxTreads * 2 performance seems to be same or even worse
        // When the maxThreads = CPU cores performance is stable ~10 seconds
        // Test: time dotnet run --configuration=Release -- --file-size 1gb --file-name test.txt
        var tasks = Enumerable.Range(0, maxThreads).Select(i => Task.Run(() =>
        {
            var rnd = random.Value!;
            var tempFile = Path.Combine(tempPath, $"temp_{i}.txt");
            tempFiles.Add(tempFile);

            using var writer = new StreamWriter(tempFile);
            for (var y = 0; y < fileSizeInMb / maxThreads * (1024 * 1024 / 20); y++) // One line should be ~20 bytes
            {
                var line = $"{rnd.Next(1, int.MaxValue)}. {companies[rnd.Next(companies.Count)]}";
                writer.WriteLine(line);
            }
        }));
        await Task.WhenAll(tasks);

        await using var writer = new StreamWriter(fileName);
        foreach (var file in tempFiles)
        {
            using var reader = new StreamReader(file);
            var read = await reader.ReadToEndAsync();
            await writer.WriteAsync(read);
            File.Delete(file);
        }
    }
}
