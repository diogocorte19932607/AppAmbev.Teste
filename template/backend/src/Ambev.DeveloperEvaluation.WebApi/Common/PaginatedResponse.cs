using Ambev.DeveloperEvaluation.WebApi.Common.Responses;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Response model for paginated results
/// </summary>
public class PaginatedResponse<T> : ApiResponse<List<T>>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
