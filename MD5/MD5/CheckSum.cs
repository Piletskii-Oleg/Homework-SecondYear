namespace MD5;

using System.Text;
using System.Security.Cryptography;

public class CheckSum
{
    readonly MD5 checkSum = MD5.Create();

    public string CalculateSequential(string path)
    {
        if (Path.HasExtension(path))
        {
            return CalculateFileHash(path);
        }
        else
        {
            return CalculateFolderHash(path);
        }
    }

    public async Task<string> CalculateParallel(string path)
    {
        if (Path.HasExtension(path))
        {
            return await CalculateFileHashAsync(path);
        }
        else
        {
            return await CalculateFolderHashAsync(path);
        }
    }

    private string CalculateFileHash(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return Convert.ToHexString(checkSum.ComputeHash(bytes));
    }

    private string CalculateFolderHash(string path)
    {
        var folderNameHash = checkSum.ComputeHash(Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)));
        var result = Convert.ToHexString(folderNameHash);

        foreach (var file in Directory.GetFiles(path))
        {
            result += CalculateFileHash(file);
        }

        var directories = Directory.GetDirectories(path);
        Array.Sort(directories);
        foreach (var directory in directories)
        {
            result += CalculateFolderHash(directory);
        }

        return result;
    }

    private async Task<string> CalculateFileHashAsync(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return Convert.ToHexString(await checkSum.ComputeHashAsync(stream));
    }

    private async Task<string> CalculateFolderHashAsync(string path)
    {
        var folderNameHash = checkSum.ComputeHash(Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)));
        var result = Convert.ToHexString(folderNameHash);

        foreach (var file in Directory.GetFiles(path))
        {
            result += await CalculateFileHashAsync(file);
        }

        var directories = Directory.GetDirectories(path);
        Array.Sort(directories);
        foreach (var directory in directories)
        {
            result += await CalculateFolderHashAsync(directory);
        }

        return result;
    }
}