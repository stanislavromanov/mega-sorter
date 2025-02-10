namespace Generator.Infrastructure.Services;

public static class ValidatorService
{
    public static int GetFileSizeInMb(string fileSize)
    {
        if ((!fileSize.ToLowerInvariant().EndsWith("mb") && !fileSize.ToLowerInvariant().EndsWith("gb")) ||
            !int.TryParse(fileSize.Substring(0, fileSize.Length - 2), out var size))
        {
            return -1;
        }

        if (fileSize.ToLowerInvariant().EndsWith("gb"))
        {
            size *= 1024;
        }

        return size;
    }
}
