// <copyright file="CompositeValidatorTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Models;
using Atya.Errors.Validation.Validators;

namespace Validation.UnitTests;

public sealed class CompositeValidatorTests
{
    [Fact]
    public async Task ValidateAsync_Should_Combine_All_Validator_Results()
    {
        var validator = new CompositeValidator<string>(new IValidator<string>[]
        {
            new RequiredValidator(),
            new LengthValidator()
        });

        var result = await validator.ValidateAsync(string.Empty);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task ValidateAsync_Should_Return_Success_When_All_Validators_Pass()
    {
        var validator = new CompositeValidator<string>(new IValidator<string>[]
        {
            new RequiredValidator(),
            new LengthValidator()
        });

        var result = await validator.ValidateAsync("abcd");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Constructor_Should_Throw_When_Validators_Is_Null()
    {
        var act = () => new CompositeValidator<string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Should_Throw_When_Validators_Contain_Null()
    {
        var validators = new IValidator<string>[]
        {
            new RequiredValidator(),
            null!
        };

        var act = () => new CompositeValidator<string>(validators);

        act.Should().Throw<ArgumentException>();
    }

    private sealed class RequiredValidator : IValidator<string>
    {
        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            return string.IsNullOrWhiteSpace(instance)
                ? ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailureTestData.Create("Value", "Value is required.")))
                : ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class LengthValidator : IValidator<string>
    {
        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            return string.IsNullOrWhiteSpace(instance) || instance.Length < 3
                ? ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailureTestData.Create("Value", "Value length must be at least 3.")))
                : ValueTask.FromResult(ValidationResult.Success);
        }
    }
}
