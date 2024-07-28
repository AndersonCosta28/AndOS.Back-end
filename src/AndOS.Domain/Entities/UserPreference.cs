using AndOS.Common.Interfaces;

namespace AndOS.Domain.Entities;

public class UserPreference : IEntity, IAggregateRoot
{
    public Guid Id { get; set; }
    public IUser User { get; set; }
    public Guid UserId { get; set; }
    public string Language { get; set; } = "en-US";
    public List<DefaultProgramForExtension> DefaultProgramsToExtensions { get; set; } = [];

    public void UpdateDefaultProgramToExtension(List<DefaultProgramForExtension> defaultProgramForExtensions)
    {
        foreach (var defaultProgramForExtension in defaultProgramForExtensions)
        {
            var currentStardandProgramExtension = this.DefaultProgramsToExtensions.Find(x => x.Extension.Equals(defaultProgramForExtension.Extension));
            if (currentStardandProgramExtension != null)
            {
                if (currentStardandProgramExtension.Program == defaultProgramForExtension.Program)
                    continue;

                DefaultProgramsToExtensions.Remove(currentStardandProgramExtension);
            }
            this.DefaultProgramsToExtensions.Add(defaultProgramForExtension);
        }
    }

    public void RemoveDefaultProgramToExtension(string extension)
    {
        var defaultProgramToExtension = this.DefaultProgramsToExtensions.Find(x => x.Extension == extension);
        this.DefaultProgramsToExtensions.Remove(defaultProgramToExtension);
    }

    public void UpdateLanguage(string language)
    {
        if (language == this.Language)
            return;

        this.Language = language;
    }
}
