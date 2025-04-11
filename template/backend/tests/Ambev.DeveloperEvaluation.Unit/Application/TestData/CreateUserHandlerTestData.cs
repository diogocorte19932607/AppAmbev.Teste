using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateUserHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid User entities.
    /// The generated users will have valid:
    /// - Username (using internet usernames)
    /// - Password (meeting complexity requirements)
    /// - Email (valid format)
    /// - Phone (Brazilian format)
    /// - Status (Active or Suspended)
    /// - Role (Customer or Admin)
    /// </summary>
    private static readonly Faker<CreateUserCommand> createUserHandlerFaker = new Faker<CreateUserCommand>()
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin));

    /// <summary>
    /// Generates a valid CreateUserCommand instance.
    /// </summary>
    public static CreateUserCommand GenerateValidCommand() => createUserHandlerFaker.Generate();

    /// <summary>
    /// Generates an invalid CreateUserCommand instance.
    /// Useful for negative test scenarios.
    /// </summary>
    public static CreateUserCommand GenerateInvalidCommand() =>
        new CreateUserCommand
        {
            Username = "",                // Empty username
            Password = "123",             // Weak password
            Email = "invalid-email",      // Invalid email format
            Phone = "999",                // Invalid phone
            Status = (UserStatus)999,     // Invalid enum
            Role = (UserRole)999          // Invalid enum
        };
}
