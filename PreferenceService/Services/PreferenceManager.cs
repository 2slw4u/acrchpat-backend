using Microsoft.EntityFrameworkCore;
using PreferenceService.Database;
using PreferenceService.Database.TableModels;
using PreferenceService.Models;

namespace PreferenceService.Services;

public class PreferenceManager(AppDbContext dbContext) : IPreferenceManager
{
    private const ThemeType DefaultTheme = ThemeType.Light;
    
    public async Task<ThemeType> GetTheme(Guid userId)
    {
        var preference = await dbContext.Preferences
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        return preference?.Theme ?? DefaultTheme;
    }

    public async Task SetTheme(Guid userId, ThemeType theme)
    {
        var preference = await dbContext.Preferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (preference == null)
        {
            preference = new Preference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Theme = theme,
                HiddenAccounts = []
            };
            dbContext.Preferences.Add(preference);
        }
        else
        {
            preference.Theme = theme;
        }
        
        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<Guid>> GetHiddenAccountList(Guid userId)
    {
        var preference = await dbContext.Preferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return preference?.HiddenAccounts ?? [];
    }

    public async Task HideAccounts(Guid userId, List<Guid> accountsToHide)
    {
        var preference = await dbContext.Preferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (preference == null)
        {
            preference = new Preference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Theme = DefaultTheme,
                HiddenAccounts = accountsToHide
            };
            dbContext.Preferences.Add(preference);
        }
        else
        {
            foreach (var accountId in accountsToHide)
            {
                if (preference.HiddenAccounts.All(id => id != accountId))
                {
                    preference.HiddenAccounts.Add(accountId);
                }
            }
        }
        
        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task UnhideAccounts(Guid userId, List<Guid> accountsToUnhide)
    {
        var preference = await dbContext.Preferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (preference == null)
        {
            preference = new Preference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Theme = DefaultTheme,
                HiddenAccounts = []
            };
            dbContext.Preferences.Add(preference);
        }
        else
        {
            preference.HiddenAccounts.RemoveAll(accountsToUnhide.Contains);
        }
        
        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task ClearPreference(Guid userId)
    {
        var preference = await dbContext.Preferences
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (preference != null)
        {
            dbContext.Remove(preference);
        }
        
        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }
    }
}