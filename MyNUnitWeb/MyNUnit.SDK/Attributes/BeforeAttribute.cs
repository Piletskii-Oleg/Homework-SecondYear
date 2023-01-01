namespace MyNUnit.SDK.Attributes;

/// <summary>
/// Attribute for methods that should be executed before each test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
}