//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using System.IO;

//namespace RentIt.Shared.Infrastructure.Data;

//public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//{
//    public ApplicationDbContext CreateDbContext(string[] args)
//    {
//        Console.WriteLine("Creating ApplicationDbContext for design-time operations...");

//        var basePath = AppContext.BaseDirectory;

//        var apiProjectPath = Path.GetFullPath(Path.Combine(basePath,
//            "..", "..", "..", "..", "RentIt.ApiGateway"));

//        Console.WriteLine($"Looking for API project at: {apiProjectPath}");

//        if(!Directory.Exists(apiProjectPath))
//        {
//            throw new InvalidOperationException(
//                        $"Could not find API project. Checked paths:\n" +
//                        $"- {Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "Core.APIGateway"))}\n" +
//                        $"- {apiProjectPath}\n" +
//                        "Ensure the API project exists and the relative path is correct.");
//        }

//        Console.WriteLine($"Using API project path: {apiProjectPath}");

//        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
//        Console.WriteLine($"Environment: {environment}");

//        var conectionStringName = "DefaultConnection";
//        ConfigurationBuilder configuration = new();
//}
