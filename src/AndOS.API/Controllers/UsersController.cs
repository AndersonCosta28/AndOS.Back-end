using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndOS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase;