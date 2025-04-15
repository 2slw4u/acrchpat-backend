using PreferenceService.Models;

namespace PreferenceService.Services;

public interface IPreferenceManager
{
    Task<ThemeType> GetTheme(Guid userId);

    Task SetTheme(Guid userId, ThemeType theme);

    Task<List<Guid>> GetHiddenAccountList(Guid userId);

    Task HideAccounts(Guid userId, List<Guid> accountsToHide);

    Task UnhideAccounts(Guid userId, List<Guid> accountsToUnhide);

    Task ClearPreference(Guid userId);
}