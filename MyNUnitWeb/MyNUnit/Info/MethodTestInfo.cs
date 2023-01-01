namespace MyNUnit.Info;

using System.Diagnostics;
using System.Reflection;
using SDK.Attributes;
using State;

/// <summary>
/// Contains information about a single test.
/// </summary>
public class MethodTestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTestInfo"/> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    private MethodTestInfo(string name)
        => this.Name = name;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTestInfo"/> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="ignored">Reason for ignoring a method.</param>
    private MethodTestInfo(string name, string ignored)
        : this(name)
    {
        this.Ignored = ignored;
        this.State = TestState.Ignored;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTestInfo"/> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="testState">State of the method.</param>
    private MethodTestInfo(string name, TestState testState)
        : this(name)
        => this.State = testState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTestInfo"/> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="testState">State of the method.</param>
    /// <param name="time">Time which was required for the test to pass.</param>
    private MethodTestInfo(string name, TestState testState, TimeSpan time)
        : this(name)
    {
        this.State = testState;
        this.CompletionTime = time;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTestInfo"/> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="expectedExceptionType">Type of an exception that was expected.</param>
    /// <param name="actualException">The exception that was caught.</param>
    /// <param name="time">Time which was required for the test to pass.</param>
    private MethodTestInfo(string name, Type? expectedExceptionType, Exception? actualException, TimeSpan time)
        : this(name)
    {
        this.HasCaughtException = true;
        this.ExceptionInfo = new ExceptionInfo(expectedExceptionType, actualException);

        this.State = this.ExceptionInfo.AreExceptionsSame() ? TestState.Passed : TestState.Failed;
        this.CompletionTime = time;
    }

    /// <summary>
    /// Gets name of the test.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets state of the test.
    /// </summary>
    public TestState State { get; }

    /// <summary>
    /// Gets time which was required to complete the test. 0 if it is ignored.
    /// </summary>
    public TimeSpan CompletionTime { get; }

    /// <summary>
    /// Gets the reason for ignoring a test if it has <see cref="TestAttribute"/> with Ignored property.
    /// </summary>
    public string? Ignored { get; }

    /// <summary>
    /// Gets a value indicating whether an exception was caught or not.
    /// </summary>
    public bool HasCaughtException { get; }

    /// <summary>
    /// Gets information about an exception if it was caught.
    /// </summary>
    public ExceptionInfo? ExceptionInfo { get; }

    /// <summary>
    /// Starts the test and returns information about it.
    /// </summary>
    /// <param name="instance">Instance on which the test is executed.</param>
    /// <param name="method">Method that should be executed.</param>
    /// <returns>Information about the test.</returns>
    public static MethodTestInfo StartTest(object? instance, MethodInfo method)
    {
        var testAttribute = method.GetCustomAttribute<TestAttribute>();

        if (testAttribute.Ignored != null)
        {
            return new MethodTestInfo(method.Name, testAttribute.Ignored);
        }

        if (method.GetParameters().Length != 0)
        {
            return new MethodTestInfo(method.Name, TestState.IncorrectNumberOfParameters);
        }

        if (method.ReturnType != typeof(void))
        {
            return new MethodTestInfo(method.Name, TestState.IncorrectReturnType);
        }

        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            method.Invoke(instance, null);
        }
        catch (TargetInvocationException exception)
        {
            stopwatch.Stop();
            return new MethodTestInfo(method.Name, testAttribute.Expected, exception.InnerException, stopwatch.Elapsed);
        }

        stopwatch.Stop();

        return testAttribute.Expected != null
            ? new MethodTestInfo(method.Name, testAttribute.Expected, null, stopwatch.Elapsed)
            : new MethodTestInfo(method.Name, TestState.Passed, stopwatch.Elapsed);
    }

    /// <summary>
    /// Prints information about the test on the console.
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if an exception has been caught but its info is null.</exception>
    public void Print()
    {
        Console.WriteLine($"--- Method name: {this.Name}");
        Console.WriteLine($"    Test State: {this.State}");

        if (this.Ignored != null)
        {
            Console.WriteLine($"    Ignore reason: {this.Ignored}");
            Console.WriteLine();
            return;
        }

        if (this.HasCaughtException)
        {
            if (this.ExceptionInfo == null)
            {
                throw new InvalidOperationException();
            }

            Console.WriteLine($"    Has caught exception {this.ExceptionInfo.ActualException}");

            Console.WriteLine(this.ExceptionInfo.ExpectedExceptionType == null
                ? "    Exception that was expected: none"
                : $"    Exception that was expected: {this.ExceptionInfo.ExpectedExceptionType}");
        }

        if (this.State == TestState.Passed)
        {
            Console.WriteLine($"    Time required: {this.CompletionTime.Milliseconds} ms.");
        }

        Console.WriteLine();
    }
}