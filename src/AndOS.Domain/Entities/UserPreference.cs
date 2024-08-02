using AndOS.Common.Interfaces;

namespace AndOS.Domain.Entities;

public class UserPreference : IEntity, IAggregateRoot
{
    public UserPreference() { }

    public UserPreference(IUser user) => this.User = user;

    public UserPreference(IUser user, string language, List<DefaultProgramForExtension> defaultProgramsToExtensions)
    {
        this.User = user;
        this.Language = language;
        this.DefaultProgramsToExtensions = defaultProgramsToExtensions;
    }

    public Guid Id { get; set; }
    public IUser User { get; private set; }
    public Guid UserId { get; private set; }
    public string Language { get; private set; }
    public List<DefaultProgramForExtension> DefaultProgramsToExtensions { get; private set; } = [];

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

    public void UpdateUser(IUser user)
    {
        if (user == this.User)
            return;
        this.User = user;
    }

    public void UpdateUser(Guid userId)
    {
        if (userId == this.UserId)
            return;
        this.UserId = userId;
    }
}
