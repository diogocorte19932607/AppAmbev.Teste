using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateUserHandler"/> class.
/// </summary>
public class CreateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly CreateUserHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new CreateUserHandler(_userRepository, _mapper, _passwordHasher);
    }

    /// <summary>
    /// Tests that a valid user creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid user data When creating user Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();

        // Dados adicionais com Faker
        var faker = new Faker("pt_BR");
        command = new Faker<CreateUserCommand>("pt_BR")
            .RuleFor(x => x.Username, f => f.Name.FullName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.Password, f => f.Internet.Password(8))
            .Generate();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Password = command.Password,
            Email = command.Email,
            Phone = command.Phone,
            Status = command.Status,
            Role = command.Role
        };

        var result = new CreateUserResult
        {
            Id = user.Id,
        };

        _mapper.Map<User>(command).Returns(user);
        _passwordHasher.HashPassword(command.Password).Returns("hashedPassword");
        _userRepository.CreateAsync(user).Returns(Task.CompletedTask);
        _mapper.Map<CreateUserResult>(user).Returns(result);

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(user.Id);
        await _userRepository.Received(1).CreateAsync(user);
    }

    /// <summary>
    /// Tests that an invalid user creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid user data When creating user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateUserCommand(); // Empty command with full validation

        // When & Then
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that the password is hashed before saving the user.
    /// </summary>
    [Fact(DisplayName = "Given valid user creation request When handling Then password is hashed")]
    public async Task Handle_ValidRequest_HashesPassword()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        command.Password = "MyPlainPassword";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Password = command.Password,
            Email = command.Email,
            Phone = command.Phone,
            Status = command.Status,
            Role = command.Role
        };

        _mapper.Map<User>(command).Returns(user);
        _passwordHasher.HashPassword(command.Password).Returns("HashedPassword");
        _userRepository.CreateAsync(user).Returns(Task.CompletedTask);
        _mapper.Map<CreateUserResult>(user).Returns(new CreateUserResult { Id = user.Id });

        // When
        var response = await _handler.Handle(command, CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        user.Password.Should().Be("HashedPassword");
    }
}
