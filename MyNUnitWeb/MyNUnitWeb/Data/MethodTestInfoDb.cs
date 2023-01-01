namespace MyNUnitWeb.Data;

using MyNUnit.State;

public class MethodTestInfoDb
{
    public int MethodTestInfoDbId { get; set; }
    
    /// <summary>
    ///     Gets name of the test.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets state of the test.
    /// </summary>
    public TestState State { get; set; }

    /// <summary>
    ///     Gets time which was required to complete the test. 0 if it is ignored.
    /// </summary>
    public TimeSpan CompletionTime { get ; set; }

    /// <summary>
    ///     Gets the reason for ignoring a test if it has <see cref="TestAttribute" /> with Ignored property.
    /// </summary>
    public string? Ignored { get; set; }

    /// <summary>
    ///     Gets a value indicating whether an exception was caught or not.
    /// </summary>
    public bool HasCaughtException { get; set; }

    /// <summary>
    ///     Gets type of the expected exception.
    /// </summary>
    public string? ExpectedExceptionType { get; set; }

    /// <summary>
    ///     Gets exception that is thrown in the test.
    /// </summary>
    public string? ActualExceptionType { get; set; }
}