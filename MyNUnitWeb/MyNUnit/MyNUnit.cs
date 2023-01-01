namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using Info;

/// <summary>
/// Class that is used to start tests.
/// </summary>
public static class MyNUnit
{
    /// <summary>
    /// Starts all tests by the given path.
    /// </summary>
    /// <param name="path">Path to the assemblies.</param>
    /// <returns>List with information about tests in each assembly.</returns>
    public static List<AssemblyTestInfo> StartAllTests(string path)
    {
        var info = new DirectoryInfo(path);
        var files = info.GetFiles();

        var assemblies = new List<FileInfo>();
        foreach (var file in files)
        {
            if (CheckIsAssembly(file))
            {
                assemblies.Add(file);
            }
        }

        var assembliesInfo = new ConcurrentBag<AssemblyTestInfo>();
        Parallel.ForEach(assemblies, file =>
        {
            byte[] rawAssembly = File.ReadAllBytes(file.FullName);
            var assembly = Assembly.Load(rawAssembly);

            assembliesInfo.Add(AssemblyTestInfo.StartAssemblyTests(assembly));
        });

        return assembliesInfo.ToList();
    }

    private static bool CheckIsAssembly(FileInfo file)
    {
        bool isAssembly = true;

        try
        {
            AssemblyName.GetAssemblyName(file.FullName);
        }
        catch (BadImageFormatException exception)
        {
            isAssembly = false;
        }

        return isAssembly;
    }
}