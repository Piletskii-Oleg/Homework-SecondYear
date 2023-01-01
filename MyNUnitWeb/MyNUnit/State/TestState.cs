namespace MyNUnit.State;

/// <summary>
/// Possible states of a test.
/// </summary>
public enum TestState
{
    /// <summary>
    /// Test has passed without issues.
    /// </summary>
    Passed,

    /// <summary>
    /// Test was not successful.
    /// </summary>
    Failed,

    /// <summary>
    /// Test was ignored.
    /// </summary>
    Ignored,

    /// <summary>
    /// Incorrect number of parameters was given to the test method (should be 0).
    /// </summary>
    IncorrectNumberOfParameters,

    /// <summary>
    /// The return type of the test method is incorrect (should be void).
    /// </summary>
    IncorrectReturnType,
}