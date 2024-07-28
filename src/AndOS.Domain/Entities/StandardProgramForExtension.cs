namespace AndOS.Domain.Entities;
public class DefaultProgramForExtension
{
    public Guid UserPreferenceId { get; set; }
    public UserPreference UserPreference { get; set; }
    public string Extension { get; set; }
    public string Program { get; set; }
}
