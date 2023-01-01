namespace MyNUnitWeb.Data;

using MyNUnit.Info;
using MyNUnit.State;

public class AssemblyTestInfoDb
{
    public int AssemblyTestInfoDbId { get; set; }
    
    public string? Name { get; set; }
    
    public List<ClassTestInfoDb> ClassesInfo { get; set; }
    
    public int GetSuccessfulTestsCount()
    {
        return ClassesInfo
            .SelectMany(classInfo => classInfo.MethodsInfo)
            .Sum(methodInfo => methodInfo.State == TestState.Passed ? 1 : 0);
    }

    public int GetUnsuccessfulTestsCount()
    {
        return ClassesInfo
            .SelectMany(classInfo => classInfo.MethodsInfo)
            .Sum(methodInfo => methodInfo.State != TestState.Passed ? 1 : 0);
    }
}