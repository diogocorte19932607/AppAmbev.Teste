using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Enums; // necessário para KeyNotFoundException

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetUserHandler"/> class.
/// </summary>
public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUserHandler(_userRepository, _mapper);
    }

    /// <summary>
    /// Tests that an existing user is correctly retrieved by ID.
    /// </summary>
    [Fact(DisplayName = "Given valid user ID When user exists Then returns user data")]
    public async Task Handle_ValidId_UserExists_ReturnsUser()
    {
        // Arrange
        var faker = new Faker("pt_BR");
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = faker.Internet.UserName(),
            Email = faker.Internet.Email(),
            Phone = faker.Phone.PhoneNumber(),
            Password = "hashedPass",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        var command = new GetUserCommand(userId);
        var expectedResult = new GetUserResult
        {
            Id = userId,
            Name = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status
        };

        _userRepository.GetByIdAsync(userId).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the user does not exist.
    /// </summary>
    [Fact(DisplayName = "Given invalid user ID When user not found Then throws KeyNotFoundException")]
    public async Task Handle_InvalidId_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var command = new GetUserCommand(nonExistentUserId);

        _userRepository.GetByIdAsync(nonExistentUserId).Returns((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {nonExistentUserId} not found");
    }
}
