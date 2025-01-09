using Admin.API.Models.Cache;
using Microsoft.AspNetCore.Mvc;
using Shared;
using static Admin.API.Constants;

namespace Admin.API.Controllers;

[ApiController]
public class SSEHelpController : ControllerBase
{
    [HttpGet("/api/help/sse/" + SSEMethods.LocateRequest)]
    public LocateRequestModel? LocateRequest() => null;

    [HttpGet("/api/help/sse/" + SSEMethods.LocateRequestHistory)]
    public LocateRequestModel[]? LocateRequestHistory() => null;

    [HttpGet("/api/help/sse/" + SSEMethods.Locate)]
    public LocateModel? Locate() => null;

    [HttpGet("/api/help/sse/" + SSEMethods.LocateHistory)]
    public LocateModel[]? LocateHistory() => null;

    [HttpGet("/api/help/sse/" + SSEMethods.Notification)]
    public GroupedNotification[]? Notification() => null;

    [HttpGet("/api/help/sse/" + SSEMethods.InternalInventory)]
    public InternalInventoryItem? InternalInventory() => null;
}
