namespace MyNUnit.SDK.Attributes;

/// <summary>
/// Attribute for tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets type of an expected exception.
    /// </summary>
    public Type? Expected { get; set; }

    /// <summary>
    /// Gets or sets reason for ignoring a test.
    /// </summary>
    public string? Ignored { get; set; }
}