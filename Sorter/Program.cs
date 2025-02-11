using Sorter.Infrastructure.Services;

namespace Sorter;

public static class Program
{
    /// <summary>
    ///     This program will take a large file and sort it, saving it to a new file with _sorted suffix.
    ///     Example file sorting command: dotnet run --file-name test.txt; this will generate test_sorted.txt
    ///     The input file must follow this format:
    ///     4. Apple
    ///     6. Microsoft
    ///     2. Google
    ///     1. Facebook
    ///     ...
    /// </summary>
    /// <param name="fileName">Input file to sort.</param>
    public static async Task Main(string fileName = "test.txt")
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine(string.IsNullOrEmpty(fileName)
                ? "Parameter --file-name is required."
                : $@"File {fileName} does not exist.");

            return;
        }

        await FileSorterService.SortFileAsync(fileName);
        Console.WriteLine(
            $@"File sorted and saved to {Path.GetFileNameWithoutExtension(fileName)}_sorted{Path.GetExtension(fileName)}");
    }
}
