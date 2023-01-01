namespace MyNUnit.SDK.Attributes;

/// <summary>
/// Attribute for methods that should be executed before all methods in a single class.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
}