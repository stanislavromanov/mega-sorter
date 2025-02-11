namespace Sorter.Infrastructure.Services;

public static class FileSorterService
{
    private const int ChunkSize = 128; // mb
    private const int ChunkSizeInBytes = ChunkSize * 1024 * 1024;

    public static async Task SortFileAsync(string fileName)
    {
        var outputFile = $"{Path.GetFileNameWithoutExtension(fileName)}_sorted{Path.GetExtension(fileName)}";
        var tempPath = Path.GetTempPath();
        var fileSizeInBytes = new FileInfo(fileName).Length;
        var totalChunks = Math.Ceiling((double)fileSizeInBytes / ChunkSizeInBytes);
        var fileChunks = new List<string>();

        Console.WriteLine("Splitting file to 128mb chunks...");
        using var reader = new StreamReader(fileName);
        for (var i = 0; i < totalChunks; i++)
        {
            var chunkFile = Path.Combine(tempPath, $"chunk_{i}.txt");
            fileChunks.Add(chunkFile);

            await using var writer = new StreamWriter(chunkFile);
            var bytesRead = 0L;
            while (bytesRead < ChunkSizeInBytes && !reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                await writer.WriteLineAsync(line);

                if (line != null)
                {
                    bytesRead += line.Length + Environment.NewLine.Length;
                }
            }
        }

        Console.WriteLine("Sorting chunks...");
        await Parallel.ForEachAsync(fileChunks, async (chunkFile, t) =>
            {
                var lines = await File.ReadAllLinesAsync(chunkFile, t);
                var sortedLines = lines
                    .OrderBy(line => line.Split(' ', 2)[1])
                    .ThenBy(line => long.Parse(line.Split(' ', 2)[0].TrimEnd('.')))
                    .ToList();

                await File.WriteAllLinesAsync(chunkFile, sortedLines, t);
            }
        );

        Console.WriteLine("Merging chunks...");
        await using var outputWriter = new StreamWriter(outputFile);
        var readers = fileChunks.Select(file => new StreamReader(file)).ToList();
        var lines = await Task.WhenAll(readers.Select(r => r.ReadLineAsync()));

        while (lines.Any(l => l != null))
        {
            var minIndex = lines
                .Select((line, index) => new { line, index })
                .Where(x => x.line != null)
                .OrderBy(x => x.line?.Split(' ', 2)[1])
                .ThenBy(x => long.Parse(x.line?.Split(' ', 2)[0].TrimEnd('.') ?? string.Empty))
                .First().index;

            await outputWriter.WriteLineAsync(lines[minIndex]);
            lines[minIndex] = await readers[minIndex].ReadLineAsync();
        }
    }
}
