﻿namespace MyNUnitWeb.Pages;

using Data;
using Microsoft.EntityFrameworkCore;
using MyNUnit.Info;

public class IndexModel : PageModel
{ 
    private readonly TestInfoDbContext infoContext;

    private readonly IWebHostEnvironment environment;
    
    public IndexModel(IWebHostEnvironment environment, TestInfoDbContext infoContext)
    {
        this.environment = environment;
        this.infoContext = infoContext;
    }

    public IList<TestInfo> TestInfos { get; private set; } = new List<TestInfo>();

    public async Task OnGetAsync()
    {
        TestInfos = await infoContext.TestsInfo
            .Include(testsInfo => testsInfo.AssembliesTestInfo)
            .ThenInclude(assemblyInfo => assemblyInfo.ClassesInfo)
            .ThenInclude(classInfo => classInfo.MethodsInfo)
            .OrderByDescending(info => info.TestInfoId).ToListAsync();
    }
    
    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        string wwwPath = environment.WebRootPath;

        await CreateFiles(files, wwwPath);

        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostRunTests()
    {
        string wwwPath = environment.WebRootPath;

        var filesPath = Path.Combine(wwwPath, "Uploads");
        var assembliesList = MyNUnit.MyNUnit.StartAllTests(filesPath);

        var dbList = new List<AssemblyTestInfoDb>();

        foreach (var assemblyTestInfo in assembliesList)
        {
            dbList.Add(new AssemblyTestInfoDb
            {
                Name = assemblyTestInfo.Name.Name,
                ClassesInfo = GetClassesInfo(assemblyTestInfo)
            });
        }

        if (dbList.Count != 0)
        {
            var info = new TestInfo
            {
                AssembliesTestInfo = dbList,
                TestDate = DateTime.Now,
            };
            infoContext.TestsInfo.Add(info);
            await infoContext.SaveChangesAsync();
        }

        DeleteFiles(filesPath);

        return RedirectToPage("./Index");
    }

    private static async Task CreateFiles(List<IFormFile> files, string path)
    {
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(path, "Uploads");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                await using var stream = System.IO.File.Create(Path.Combine(filePath, Path.GetRandomFileName()));
                await file.CopyToAsync(stream);
            }
        }
    }
    
    private void DeleteFiles(string path)
    {
        var files = Directory.GetFiles(path);
        foreach (var fileName in files)
        {
            System.IO.File.Delete(fileName);
        }
    }
    
    private List<ClassTestInfoDb> GetClassesInfo(AssemblyTestInfo info)
    {
        var list = new List<ClassTestInfoDb>();
        foreach (var classInfo in info.ClassesInfo)
        {
            list.Add(new ClassTestInfoDb
            {
                Name = classInfo.Name,
                MethodsInfo = GetMethodsInfo(classInfo),
                State = classInfo.State,
            });
        }

        return list;
    }

    private List<MethodTestInfoDb>? GetMethodsInfo(ClassTestInfo classInfo)
    {
        var list = new List<MethodTestInfoDb>();

        if (classInfo.MethodsInfo == null)
        {
            return null;
        }
        
        foreach (var methodInfo in classInfo.MethodsInfo)
        {
            list.Add(new MethodTestInfoDb
            {
                Name = methodInfo.Name,
                State = methodInfo.State,
                CompletionTime = methodInfo.CompletionTime,
                Ignored = methodInfo.Ignored,
                HasCaughtException = methodInfo.HasCaughtException,
                ExpectedExceptionType = methodInfo.ExceptionInfo?.ExpectedExceptionType?.ToString(),
                ActualExceptionType = methodInfo.ExceptionInfo?.ActualException?.GetType().ToString(),
            });
        }

        return list;
    }
}