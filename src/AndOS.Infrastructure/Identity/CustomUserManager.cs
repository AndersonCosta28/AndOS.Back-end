using AndOS.Application.Users.Get.GetUserByUserName;
using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Exceptions;
using AndOS.Infrastructure.Identity.Entities;
using AndOS.Resources.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AndOS.Infrastructure.Identity;

public class CustomUserManager(IUserStore<ApplicationUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<ApplicationUser> passwordHasher,
    IEnumerable<IUserValidator<ApplicationUser>> userValidators,
    IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<ApplicationUser>> logger,
    IRepository<IUser> userRepository,
    IStringLocalizer<ValidationResource> validationLocalizer) : UserManager<ApplicationUser>(store,
    optionsAccessor,
    passwordHasher,
    userValidators,
    passwordValidators,
    keyNormalizer,
    errors,
    services,
    logger)
{
    private readonly IRepository<IUser> _userRepository = userRepository;
    private readonly IStringLocalizer<ValidationResource> _validationLocalizer = validationLocalizer;

    public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
    {
        var userNameAlreadyExists = await _userRepository.AnyAsync(new GetUserByUserNameSpec(user.UserName));
        if (userNameAlreadyExists)
            throw new InfrastructureLayerException(_validationLocalizer["UserNameAlreadyExists"]);

        var emailAlreadyExists = await Users.AnyAsync(x => x.Email.ToLower().Trim() == user.Email.ToLower().Trim());
        if (emailAlreadyExists)
            throw new InfrastructureLayerException(_validationLocalizer["EmailAlreadyExists"]);
        return await base.CreateAsync(user);
    }
}