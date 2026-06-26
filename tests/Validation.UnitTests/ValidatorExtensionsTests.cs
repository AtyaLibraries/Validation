// <copyright file="ValidatorExtensionsTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Errors.Exceptions;

namespace Validation.UnitTests;

public sealed class ValidatorExtensionsTests
{
    [Fact]
    public async Task ValidateAllAsync_Should_Combine_All_Validator_Results()
    {
        var validators = new IValidator<string>[]
        {
            new EmailRequiredValidator(),
            new EmailMustContainAtValidator()
        };

        var result = await validators.ValidateAllAsync(string.Empty, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task ValidateAllAsync_Should_Return_Success_When_All_Are_Valid()
    {
        var validators = new IValidator<string>[]
        {
            new EmailRequiredValidator(),
            new EmailMustContainAtValidator()
        };

        var result = await validators.ValidateAllAsync("user@example.com", TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAllAsync_Should_Pass_CancellationToken_To_Each_Validator()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var validators = new IValidator<string>[]
        {
            new CapturingValidator(),
            new CapturingValidator()
        };

        var result = await validators.ValidateAllAsync("user@example.com", cancellationTokenSource.Token);

        result.IsValid.Should().BeTrue();
        validators.Cast<CapturingValidator>().Should().OnlyContain(
            validator => validator.CancellationToken == cancellationTokenSource.Token);
    }

    [Fact]
    public async Task ValidateAllAsync_Should_Throw_When_Validators_Is_Null()
    {
        var act = async () => await ValidatorExtensions.ValidateAllAsync<string>(
            null!,
            "abc",
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ValidateAllAsync_Should_Throw_When_Validator_Is_Null()
    {
        var validators = new IValidator<string>[]
        {
            new EmailRequiredValidator(),
            null!
        };

        var act = async () => await validators.ValidateAllAsync("abc", TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ValidateAllAsync_Should_Throw_When_Validator_Returns_Null()
    {
        var validators = new IValidator<string>[]
        {
            new NullResultValidator()
        };

        var act = async () => await validators.ValidateAllAsync("abc", TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ValidateAllAsync_Should_Throw_When_Cancellation_Is_Requested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var validator = new CountingValidator();
        var validators = new IValidator<string>[] { validator };

        var act = async () => await validators.ValidateAllAsync("abc", cancellationTokenSource.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        validator.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Return_Result_When_Valid()
    {
        var validator = new EmailRequiredValidator();

        var result = await validator.ValidateAndThrowAsync(
            "user@example.com",
            cancellationToken: TestContext.Current.CancellationToken);

        result.Should().BeSameAs(ValidationResult.Success);
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Throw_When_Invalid()
    {
        var validator = new EmailRequiredValidator();

        var act = async () => await validator.ValidateAndThrowAsync(
            string.Empty,
            cancellationToken: TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Throw_When_Validator_Is_Null()
    {
        var act = async () => await ValidatorExtensions.ValidateAndThrowAsync<string>(
            null!,
            "abc",
            cancellationToken: TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Throw_When_Result_Is_Null()
    {
        var validator = new NullResultValidator();

        var act = async () => await validator.ValidateAndThrowAsync(
            "abc",
            cancellationToken: TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ValidateAndThrowAsync_Should_Throw_When_Cancellation_Is_Requested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var validator = new CountingValidator();

        var act = async () => await validator.ValidateAndThrowAsync("abc", cancellationToken: cancellationTokenSource.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        validator.CallCount.Should().Be(0);
    }

    private sealed class EmailRequiredValidator : IValidator<string>
    {
        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            return string.IsNullOrWhiteSpace(instance)
                ? ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailureTestData.Create("Email", "Email is required.")))
                : ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class EmailMustContainAtValidator : IValidator<string>
    {
        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            return string.IsNullOrWhiteSpace(instance) || !instance.Contains('@')
                ? ValueTask.FromResult(ValidationResult.FromFailure(
                    ValidationFailureTestData.Create("Email", "Email must contain @.")))
                : ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class CapturingValidator : IValidator<string>
    {
        public CancellationToken CancellationToken { get; private set; }

        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            _ = instance;
            CancellationToken = cancellationToken;

            return ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class CountingValidator : IValidator<string>
    {
        public int CallCount { get; private set; }

        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            _ = instance;
            _ = cancellationToken;
            CallCount++;

            return ValueTask.FromResult(ValidationResult.Success);
        }
    }

    private sealed class NullResultValidator : IValidator<string>
    {
        public ValueTask<ValidationResult> ValidateAsync(string instance, CancellationToken cancellationToken = default)
        {
            _ = instance;
            _ = cancellationToken;

            return ValueTask.FromResult<ValidationResult>(null!);
        }
    }
}
