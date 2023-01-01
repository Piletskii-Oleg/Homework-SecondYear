namespace MyNUnit.Info;

using System.Collections.Concurrent;
using System.Reflection;
using SDK.Attributes;

/// <summary>
/// Contains information about tests in an assembly.
/// </summary>
public class AssemblyTestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyTestInfo"/> class.
    /// </summary>
    /// <param name="name">Name of the assembly.</param>
    /// <param name="classesInfo">List of <see cref="ClassTestInfo"/> of the assembly.</param>
    private AssemblyTestInfo(AssemblyName name, IReadOnlyList<ClassTestInfo> classesInfo)
    {
        this.Name = name;
        this.ClassesInfo = classesInfo;
    }

    /// <summary>
    /// Gets name of the assembly.
    /// </summary>
    public AssemblyName Name { get; }

    /// <summary>
    /// Gets list of <see cref="ClassTestInfo"/> of the assembly.
    /// </summary>
    public IReadOnlyList<ClassTestInfo> ClassesInfo { get; }

    /// <summary>
    /// Starts tests in the given assembly and returns info about it.
    /// </summary>
    /// <param name="assembly">Assembly that should be tested.</param>
    /// <returns>Information about tests contained in an assembly.</returns>
    /// <exception cref="InvalidOperationException">Throws if full name of the type is null.</exception>
    public static AssemblyTestInfo StartAssemblyTests(Assembly assembly)
    {
        var suitableTypes = GetTypes(assembly);
        var classesInfo = new ConcurrentBag<ClassTestInfo>();
        Parallel.ForEach(suitableTypes, type =>
        {
            if (type.FullName == null)
            {
                throw new InvalidOperationException("Type name cannot be null");
            }

            classesInfo.Add(ClassTestInfo.StartTests(type));
        });

        return new AssemblyTestInfo(assembly.GetName(), classesInfo.ToList());
    }

    /// <summary>
    /// Prints information about assembly tests on the console.
    /// </summary>
    public void Print()
    {
        Console.WriteLine($"Assembly name: {Name}");
        foreach (var classInfo in ClassesInfo)
        {
            classInfo.Print();
        }
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly)
        => (from type in assembly.DefinedTypes
            from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == typeof(TestAttribute)
            select type).Distinct();
}