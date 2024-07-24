using AndOS.Domain.Entities;
using File = AndOS.Domain.Entities.File;

namespace AndOS.Domain.Interfaces;

public interface ISystemItemService
{
    Task<string> GetCurrentPath(Folder folder);
    Task<string> GetCurrentPath(File file);
}
