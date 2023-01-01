namespace MyNUnit.Info;

/// <summary>
/// Contains information about an exception that may be thrown within a test.
/// </summary>
public class ExceptionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
    /// </summary>
    /// <param name="expectedExceptionType">Type of expected exception.</param>
    /// <param name="actualException">Exception that is thrown in the test.</param>
    public ExceptionInfo(Type? expectedExceptionType, Exception? actualException)
    {
        this.ExpectedExceptionType = expectedExceptionType;
        this.ActualException = actualException;
    }

    /// <summary>
    /// Gets type of the expected exception.
    /// </summary>
    public Type? ExpectedExceptionType { get; }

    /// <summary>
    /// Gets exception that is thrown in the test.
    /// </summary>
    public Exception? ActualException { get; }

    /// <summary>
    /// Checks if <see cref="ExpectedExceptionType"/> and type of <see cref="ActualException"/> are same.
    /// </summary>
    /// <returns>True if types are the same and false otherwise.
    /// Also returns false if an exception was expected but was not caught.</returns>
    public bool AreExceptionsSame()
    {
        if (this.ActualException == null)
        {
            return false;
        }

        return this.ExpectedExceptionType == this.ActualException.GetType();
    }
}