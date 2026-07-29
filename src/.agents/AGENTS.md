# RentIt Project Rules

## Logging
- **Always use Serilog**: Inject and use `Serilog.ILogger` instead of `Microsoft.Extensions.Logging.ILogger<T>` across all application classes. Use `_logger.Information`, `_logger.Warning`, `_logger.Error`, etc.

## Dependency Management
- **Check CPM First**: Before downloading or installing any new NuGet packages, check `Directory.Packages.props` first to see if the package is already centrally managed. If it is, simply add a `<PackageReference Include="Package.Name" />` without specifying the version in the `.csproj` file.
