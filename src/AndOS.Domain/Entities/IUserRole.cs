namespace AndOS.Domain.Entities;

public interface IUserRole
{
    public IUser User { get; set; }
    public IRole Role { get; set; }
}
