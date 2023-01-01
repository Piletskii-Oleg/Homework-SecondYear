namespace MyNUnit.State;

using SDK.Attributes;

/// <summary>
/// Possible states of the class.
/// </summary>
public enum ClassState
{
    /// <summary>
    /// Tests can be passed without issues on the side of the class.
    /// </summary>
    Passed,

    /// <summary>
    /// Exception occured in the method with <see cref="BeforeAttribute"/>.
    /// </summary>
    BeforeMethodFailed,

    /// <summary>
    /// Exception occured in the method with <see cref="AfterAttribute"/>.
    /// </summary>
    AfterMethodFailed,

    /// <summary>
    /// Method with <see cref="BeforeClassAttribute"/> or <see cref="AfterClassAttribute"/> is not static.
    /// </summary>
    ClassMethodWasNotStatic,

    /// <summary>
    /// Exception occured in the method with <see cref="BeforeClassAttribute"/>.
    /// </summary>
    BeforeClassMethodFailed,

    /// <summary>
    /// Exception occured in the method with <see cref="AfterClassAttribute"/>.
    /// </summary>
    AfterClassMethodFailed,

    /// <summary>
    /// The class is abstract.
    /// </summary>
    ClassIsAbstract,
}