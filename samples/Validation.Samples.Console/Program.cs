// <copyright file="Program.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Errors.Validation.Validators;

namespace Validation.Samples.Console;

public static class Program
{
    public static async Task Main()
    {
        var validator = new CompositeValidator<CreateCustomerCommand>(
        [
            new NameRequiredValidator(),
            new EmailFormatValidator(),
        ]);

        var command = new CreateCustomerCommand(string.Empty, "not-an-email");
        var result = await validator.ValidateAsync(command);

        System.Console.WriteLine($"IsValid: {result.IsValid}");
        System.Console.WriteLine($"Error count: {result.Errors.Count}");

        foreach (var error in result.Errors)
        {
            System.Console.WriteLine($"{error.PropertyName}: {error.Message} ({error.ErrorCode ?? "n/a"})");
        }
    }

    private sealed record CreateCustomerCommand(string Name, string Email);

    private sealed class NameRequiredValidator : IValidator<CreateCustomerCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CreateCustomerCommand instance,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            return string.IsNullOrWhiteSpace(instance.Name)
                ? ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailureFactory.Required(nameof(instance.Name), instance.Name)))
                : ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class EmailFormatValidator : IValidator<CreateCustomerCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CreateCustomerCommand instance,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            return instance.Email.Contains('@', StringComparison.Ordinal)
                ? ValueTask.FromResult(ValidationResult.Success)
                : ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailureFactory.Invalid(
                        nameof(instance.Email),
                        "Email must contain '@'.",
                        instance.Email)));
        }
    }
}
