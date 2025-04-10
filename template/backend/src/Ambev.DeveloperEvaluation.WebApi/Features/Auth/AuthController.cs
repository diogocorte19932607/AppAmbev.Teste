using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of AuthController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The logger instance</param>
    public AuthController(IMediator mediator, IMapper mapper, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user with their credentials
    /// </summary>
    /// <param name="request">The authentication request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication token if successful</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<AuthenticateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received authentication request for user: {Email}", request.Email);

        var validator = new AuthenticateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for authentication request: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var command = _mapper.Map<AuthenticateUserCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("User authenticated successfully: {Email}", request.Email);

            return Ok(new ApiResponseWithData<AuthenticateUserResponse>
            {
                Success = true,
                Message = "User authenticated successfully",
                Data = _mapper.Map<AuthenticateUserResponse>(response)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while authenticating user: {Email}", request.Email);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred during authentication"
            });
        }
    }
}
