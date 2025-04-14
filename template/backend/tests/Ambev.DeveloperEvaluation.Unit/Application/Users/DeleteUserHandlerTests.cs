using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="DeleteUserHandler"/> class.
/// </summary>
public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new DeleteUserHandler(_userRepository);
    }

    /// <summary>
    /// Tests that an existing user is successfully deleted.
    /// </summary>
    [Fact(DisplayName = "Given valid user ID When user exists Then deletes user successfully")]
    public async Task Handle_ValidUserId_DeletesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            Password = "hashedPass",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        var command = new DeleteUserCommand(userId);

        _userRepository.GetByIdAsync(userId).Returns(user);
        _userRepository.DeleteAsync(user.Id).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        await _userRepository.Received(1).DeleteAsync(user.Id);
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when user does not exist.
    /// </summary>
    [Fact(DisplayName = "Given invalid user ID When user not found Then throws KeyNotFoundException")]
    public async Task Handle_InvalidUserId_ThrowsNotFoundException()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid();
        var command = new DeleteUserCommand(invalidUserId);

        _userRepository.GetByIdAsync(invalidUserId).Returns((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {invalidUserId} not found");
    }
}
