using Ardalis.Specification;

namespace AndOS.Application.Users.Get.GetUserByEmail;

public class GetUserByEmailSpec : Specification<IUser>
{
    public GetUserByEmailSpec(string email)
    {
        Query
            .Where(x => x.Email.ToLower() == email.ToLower().Trim());
    }
}