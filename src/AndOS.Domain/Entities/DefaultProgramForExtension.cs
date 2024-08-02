namespace AndOS.Domain.Entities;
public class DefaultProgramForExtension
{
    public DefaultProgramForExtension() { }

    public DefaultProgramForExtension(string extension, string program)
    {
        this.Extension = extension;
        this.Program = program;
    }

    public Guid UserPreferenceId { get; private set; }
    public UserPreference UserPreference { get; private set; }
    public string Extension { get; private set; }
    public string Program { get; private set; }
}
