namespace MyNUnit.Info;

using System.Reflection;
using SDK.Attributes;
using State;

/// <summary>
///     Contains information about tests in a class.
/// </summary>
public class ClassTestInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClassTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the class.</param>
    /// <param name="methodsInfo">List of <see cref="MethodTestInfo" /> that contain information about all tests.</param>
    private ClassTestInfo(string name, IReadOnlyList<MethodTestInfo> methodsInfo)
    {
        Name = name;
        MethodsInfo = methodsInfo;
        State = ClassState.Passed;
    }

    private ClassTestInfo(string name, ClassState state)
    {
        Name = name;
        State = state;
    }

    /// <summary>
    ///     Gets name of the class.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets list of <see cref="MethodTestInfo" /> that contain information about all tests.
    /// </summary>
    public IReadOnlyList<MethodTestInfo>? MethodsInfo { get; }

    /// <summary>
    ///     Gets state of the class.
    /// </summary>
    public ClassState State { get; }

    /// <summary>
    ///     Starts all tests in a single class.
    /// </summary>
    /// <param name="type">Type that should be tested.</param>
    /// <param name="instance">Instance on which tests should be done.</param>
    /// <returns><see cref="ClassTestInfo" /> that contains information about tests.</returns>
    public static ClassTestInfo StartTests(Type type, object instance)
    {
        if (type.IsAbstract) return new ClassTestInfo(type.Name, ClassState.ClassIsAbstract);

        var state = StartSupplementaryClassMethods(type, typeof(BeforeClassAttribute));
        if (state != ClassState.Passed) return new ClassTestInfo(type.Name, state);

        var testMethods = GetMethods(type, typeof(TestAttribute));
        var methodsInfo = new List<MethodTestInfo>();

        foreach (var method in testMethods)
        {
            state = StartSupplementaryMethods(type, instance, typeof(BeforeAttribute));
            if (state != ClassState.Passed) return new ClassTestInfo(type.Name, state);

            methodsInfo.Add(method.IsStatic
                ? MethodTestInfo.StartTest(null, method)
                : MethodTestInfo.StartTest(instance, method));

            state = StartSupplementaryMethods(type, instance, typeof(AfterAttribute));
            if (state != ClassState.Passed) return new ClassTestInfo(type.Name, state);
        }

        state = StartSupplementaryClassMethods(type, typeof(AfterClassAttribute));
        if (state != ClassState.Passed) return new ClassTestInfo(type.Name, state);

        return new ClassTestInfo(type.Name, methodsInfo);
    }

    /// <summary>
    ///     Prints information about tests in a class on the console.
    /// </summary>
    public void Print()
    {
        Console.WriteLine($"- Class {Name}");
        Console.WriteLine($"- State: {State}");

        if (MethodsInfo == null) return;

        foreach (var methodInfo in MethodsInfo)
        {
            methodInfo.Print();
        }
    }

    /// <summary>
    ///     Used to start methods with either <see cref="BeforeAttribute" /> of <see cref="AfterAttribute" /> attributes.
    /// </summary>
    /// <param name="type">Type where methods are contained.</param>
    /// <param name="instance">Instance on which methods are executed.</param>
    /// <param name="attributeType">Either <see cref="BeforeAttribute" /> of <see cref="AfterAttribute" />.</param>
    /// <returns>State of the class: Passed if everything's fine or other if an exception occurs within.</returns>
    private static ClassState StartSupplementaryMethods(Type type, object instance, Type attributeType)
    {
        var methods = GetMethods(type, attributeType);
        var methodsInfo = methods as MethodInfo[] ?? methods.ToArray();

        try
        {
            Parallel.ForEach(methodsInfo, method => method.Invoke(instance, null));
        }
        catch (AggregateException exception)
        {
            if (exception.InnerException is TargetInvocationException)
            {
                if (attributeType == typeof(BeforeAttribute)) return ClassState.BeforeMethodFailed;

                if (attributeType == typeof(AfterAttribute)) return ClassState.AfterMethodFailed;
            }
        }

        return ClassState.Passed;
    }

    /// <summary>
    ///     Used to start methods with either <see cref="BeforeClassAttribute" /> of <see cref="AfterClassAttribute" />
    ///     attributes.
    /// </summary>
    /// <param name="type">Type where methods are contained.</param>
    /// <param name="attributeType">Either <see cref="BeforeClassAttribute" /> of <see cref="AfterClassAttribute" />.</param>
    /// <returns>State of the class: Passed if everything's fine or other if an exception occurs within.</returns>
    private static ClassState StartSupplementaryClassMethods(Type type, Type attributeType)
    {
        var classMethods = GetMethods(type, attributeType);
        var classMethodsInfo = classMethods as MethodInfo[] ?? classMethods.ToArray();

        if (classMethodsInfo.Any(method => !method.IsStatic)) return ClassState.ClassMethodWasNotStatic;

        try
        {
            Parallel.ForEach(classMethodsInfo, method => method.Invoke(null, null));
        }
        catch (AggregateException exception)
        {
            if (exception.InnerException is TargetInvocationException)
            {
                if (attributeType == typeof(BeforeClassAttribute)) return ClassState.BeforeClassMethodFailed;

                if (attributeType == typeof(AfterClassAttribute)) return ClassState.AfterClassMethodFailed;
            }
        }

        return ClassState.Passed;
    }

    private static IEnumerable<MethodInfo> GetMethods(Type type, Type attributeType)
    {
        return from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == attributeType
            select method;
    }
}