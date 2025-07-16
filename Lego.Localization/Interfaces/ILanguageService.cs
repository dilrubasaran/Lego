using Lego.Contexts.Models;

namespace Lego.Localization.Interfaces;

public interface ILanguageService
{
    Language? DedectLanguage();
    string DedectVisitorCulture();
    string DedectVisitorUICulture();
    void SetCulture(string culture);
}