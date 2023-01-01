namespace MyNUnit.SDK.Attributes;

/// <summary>
/// Attribute for methods that should be executed after each test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterAttribute : Attribute
{
}