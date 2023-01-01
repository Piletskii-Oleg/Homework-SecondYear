namespace MyNUnitWeb.Data;

using MyNUnit.Info;
using MyNUnit.State;

public class ClassTestInfoDb
{
    public int ClassTestInfoDbId { get; set; }
    
    /// <summary>
    ///     Gets name of the class.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets list of <see cref="MethodTestInfo" /> that contain information about all tests.
    /// </summary>
    public List<MethodTestInfoDb>? MethodsInfo { get; set; }

    /// <summary>
    ///     Gets state of the class.
    /// </summary>
    public ClassState State { get; set; }
}