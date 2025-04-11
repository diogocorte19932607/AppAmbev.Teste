using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ambev.DeveloperEvaluation.WebApi.Common.Responses;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Ok<T>(T data) =>
        base.Ok(ApiResponse<T>.Ok(data));

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(data, "Created"));

    protected IActionResult BadRequest(string message) =>
        base.BadRequest(ApiResponse<string>.Fail(message, "Bad request"));

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(ApiResponse<string>.Fail(message, "Not found"));

    protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
        base.Ok(new PaginatedResponse<T>
        {
            Data = pagedList,
            CurrentPage = pagedList.CurrentPage,
            TotalPages = pagedList.TotalPages,
            TotalCount = pagedList.TotalCount,
            Success = true,
            Message = "Paged data returned"
        });
}
