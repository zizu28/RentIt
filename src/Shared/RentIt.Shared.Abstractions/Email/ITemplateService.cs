namespace RentIt.Shared.Abstractions.Email;

public interface ITemplateService
{
    Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model);
}

