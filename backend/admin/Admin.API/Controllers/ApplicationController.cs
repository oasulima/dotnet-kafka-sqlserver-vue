using Microsoft.AspNetCore.Mvc;

namespace Admin.API.Controllers;

[ApiController]
public class ApplicationController : ControllerBase
{
    [HttpGet("/api/Application/running-env")]
    public string GetRunningEnv([FromServices] IWebHostEnvironment env)
    {
        return env.EnvironmentName.ToString();
    }
}