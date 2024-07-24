using AndOS.Common.Classes;

namespace AndOS.Domain.Entities;

public abstract class SystemItem : BaseAuditableEntity
{
    public SystemItem() { }

    public SystemItem(string name, IUser owner, Folder parentFolder = null, string icon = "") : this()
    {
        UpdateName(name);
        UpdateIcon(icon);
        UpdateOwner(owner);
    }

    public string NormalizedName { get; private set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            NormalizedName = value?.ToLower().Trim();
        }
    }
    public string Icon { get; protected set; }
    public IUser Owner { get; set; }

    public abstract void UpdateName(string name);

    public void UpdateIcon(string icon)
    {
        if (icon == Icon)
            return;

        Icon = icon;
    }

    public void UpdateOwner(IUser owner)
    {
        if (owner.Id == Owner?.Id)
            return;

        Owner = owner;
    }
}