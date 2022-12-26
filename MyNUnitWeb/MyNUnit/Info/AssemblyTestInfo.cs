namespace MyNUnit.Info;

using System.Collections.Concurrent;
using System.Reflection;
using SDK.Attributes;
using State;

/// <summary>
///     Contains information about tests in an assembly.
/// </summary>
public class AssemblyTestInfo
{
    public AssemblyTestInfo()
    {
    }
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="AssemblyTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the assembly.</param>
    /// <param name="classesInfo">List of <see cref="ClassTestInfo" /> of the assembly.</param>
    private AssemblyTestInfo(AssemblyName name, List<ClassTestInfo> classesInfo)
    {
        Name = name.Name;
        ClassesInfo = classesInfo;
    }

    public int AssemblyTestInfoId { get; set; }
    
    /// <summary>
    ///     Gets name of the assembly.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets list of <see cref="ClassTestInfo" /> of the assembly.
    /// </summary>
    public List<ClassTestInfo> ClassesInfo { get; set; }

    /// <summary>
    ///     Starts tests in the given assembly and returns info about it.
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
            if (type.FullName == null) throw new InvalidOperationException("Type name cannot be null");

            var instance = assembly.CreateInstance(type.FullName);
            classesInfo.Add(ClassTestInfo.StartTests(type, instance));
        });

        return new AssemblyTestInfo(assembly.GetName(), classesInfo.ToList());
    }

    public int GetSuccessfulTestsCount()
    {
        return ClassesInfo
            .SelectMany(classInfo => classInfo.MethodsInfo)
            .Sum(methodInfo => methodInfo.State == TestState.Passed ? 1 : 0);
    }

    public int GetUnsuccessfulTestsCount()
    {
        return ClassesInfo
                .SelectMany(classInfo => classInfo.MethodsInfo)
                .Sum(methodInfo => methodInfo.State != TestState.Passed ? 1 : 0);
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        return (from type in assembly.DefinedTypes
            from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == typeof(TestAttribute)
            select type).Distinct();
    }
}