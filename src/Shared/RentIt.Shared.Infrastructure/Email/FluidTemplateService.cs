using Fluid;
using RentIt.Shared.Abstractions.Email;
using System.Reflection;

namespace RentIt.Shared.Infrastructure.Email;

internal sealed class FluidTemplateService : ITemplateService
{
    private readonly FluidParser _parser;
    private readonly TemplateOptions _options;

    public FluidTemplateService()
    {
        _parser = new FluidParser();
        _options = new TemplateOptions();
    }

    public async Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model)
    {
        // Add the model type to Fluid options so its properties can be accessed
        try
        {
            _options.MemberAccessStrategy.Register<TModel>();
        }
        catch { }

        // We load templates from Embedded Resources in this assembly.
        // The expected path is RentIt.Shared.Infrastructure.Email.Templates.{templateName}.liquid
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"RentIt.Shared.Infrastructure.Email.Templates.{templateName}.liquid";

        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"Template {resourceName} not found as embedded resource.");
        using var reader = new StreamReader(stream);
        var source = await reader.ReadToEndAsync();

        if (_parser.TryParse(source, out var template, out var error))
        {
            var context = new TemplateContext(model, _options);
            return await template.RenderAsync(context);
        }

        throw new InvalidOperationException($"Failed to parse template {templateName}: {error}");
    }
}

