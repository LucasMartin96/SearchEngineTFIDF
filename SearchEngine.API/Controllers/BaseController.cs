using Microsoft.AspNetCore.Mvc;

namespace SearchEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase;