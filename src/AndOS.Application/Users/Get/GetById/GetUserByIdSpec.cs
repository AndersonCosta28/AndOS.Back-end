using Ardalis.Specification;

namespace AndOS.Application.Users.Get.GetById;

public class GetUserByIdSpec : Specification<IUser>
{
    public GetUserByIdSpec(Guid id)
    {
        Query
            .Include(x => x.Folder)
            .Where(x => x.Id == id);
    }
}
