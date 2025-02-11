using Generator.Infrastructure.Services;

namespace Generator;

public static class Program
{
    /// <summary>
    ///     This program will generate a dummy file with the specific size that is provided.
    ///     Example file generation command: dotnet run --file-name test.txt --file-size 2gb
    ///     Example file content:
    ///     4. Apple
    ///     6. Microsoft
    ///     2. Google
    ///     1. Facebook
    /// </summary>
    /// <param name="fileName">Name of the file to be generated e.g. test.txt.</param>
    /// <param name="fileSize">
    ///     Size of the file to be generated. Can be megabytes e.g. 2mb or gigabytes e.g. 2gb. Would not
    ///     recommend setting it to more than 10gb since it will take long time.
    /// </param>
    public static async Task Main(string fileName, string fileSize)
    {
        var fileSizeInMb = ValidatorService.GetFileSizeInMb(fileSize);

        if (string.IsNullOrEmpty(fileName) || fileSizeInMb < 1)
        {
            Console.WriteLine(
                "Invalid input. Please provide a --file-name and a --file-size e.g. --file-name test.txt --file-size 2gb. For more information use --help.");
            return;
        }

        Console.WriteLine($@"Writing {fileSizeInMb}mb to {fileName} ...");
        await FileGeneratorService.GenerateFileAsync(fileName, fileSizeInMb);
    }
}
