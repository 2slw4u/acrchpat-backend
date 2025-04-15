using System.ComponentModel.DataAnnotations;
using PreferenceService.Models;

namespace PreferenceService.Database.TableModels;

public class Preference
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public ThemeType Theme { get; set; }

    [Required]
    public List<Guid> HiddenAccounts { get; set; }
}