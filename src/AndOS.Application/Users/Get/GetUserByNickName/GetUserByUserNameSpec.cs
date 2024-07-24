using Ardalis.Specification;

namespace AndOS.Application.Users.Get.GetUserByUserName;

public class GetUserByUserNameSpec : Specification<IUser>
{
    public GetUserByUserNameSpec(string userName)
    {
        Query.Where(user => user.UserName.ToLower().Trim() == userName.ToLower().Trim());
    }
}