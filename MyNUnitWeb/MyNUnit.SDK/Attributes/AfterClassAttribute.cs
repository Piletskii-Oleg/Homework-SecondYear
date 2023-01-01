namespace MyNUnit.SDK.Attributes;

/// <summary>
/// Attribute for methods that should be executed after all methods in a single class.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : Attribute
{
}