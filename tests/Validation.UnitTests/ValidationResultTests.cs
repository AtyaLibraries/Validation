// <copyright file="ValidationResultTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Validation.Models;

namespace Validation.UnitTests;

public sealed class ValidationResultTests
{
    [Fact]
    public void Success_Should_Be_Valid()
    {
        ValidationResult.Success.IsValid.Should().BeTrue();
        ValidationResult.Success.Errors.Should().BeEmpty();
    }

    [Fact]
    public void FromFailure_Should_Create_Invalid_Result()
    {
        var failure = ValidationFailureTestData.Create("Email", "Email is required.");
        var result = ValidationResult.FromFailure(failure);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().Be("Email");
    }

    [Fact]
    public void FromFailure_Should_Throw_When_Failure_Is_Null()
    {
        var act = () => ValidationResult.FromFailure(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromFailures_Should_Return_Success_When_Collection_Is_Empty()
    {
        var result = ValidationResult.FromFailures(Array.Empty<ValidationFailure>());

        result.Should().BeSameAs(ValidationResult.Success);
    }

    [Fact]
    public void FromFailures_Should_Throw_When_Collection_Is_Null()
    {
        var act = () => ValidationResult.FromFailures(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromFailures_Should_Throw_When_Collection_Contains_Null()
    {
        var failures = new ValidationFailure?[]
        {
            ValidationFailureTestData.Create("Email", "Email is required."),
            null
        };

        var act = () => ValidationResult.FromFailures(failures!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot contain null items*");
    }

    [Fact]
    public void FromFailures_Should_Copy_Input_Collection()
    {
        var first = ValidationFailureTestData.Create("Email", "Email is required.");
        var replacement = ValidationFailureTestData.Create("Age", "Age is invalid.");
        var failures = new[] { first };

        var result = ValidationResult.FromFailures(failures);
        failures[0] = replacement;

        result.Errors.Should().ContainSingle();
        result.Errors[0].Should().Be(first);
    }

    [Fact]
    public void Combine_Should_Return_Success_When_Collection_Is_Empty()
    {
        var result = ValidationResult.Combine(Array.Empty<ValidationResult>());

        result.Should().BeSameAs(ValidationResult.Success);
    }

    [Fact]
    public void Combine_Should_Throw_When_Collection_Is_Null()
    {
        var act = () => ValidationResult.Combine((IEnumerable<ValidationResult>)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Combine_Should_Merge_All_Failures()
    {
        var left = ValidationResult.FromFailure(ValidationFailureTestData.Create("Email", "Email is required."));
        var right = ValidationResult.FromFailure(ValidationFailureTestData.Create("Age", "Age is invalid."));

        var result = ValidationResult.Combine(left, right);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Combine_Should_Throw_When_Any_Result_Is_Null()
    {
        var valid = ValidationResult.Success;

        var act = () => ValidationResult.Combine(valid, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Combine_Params_Should_Throw_When_Array_Is_Null()
    {
        var act = () => ValidationResult.Combine((ValidationResult[])null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddFailure_Should_Return_New_Result()
    {
        var result = ValidationResult.Success
            .AddFailure(ValidationFailureTestData.Create("Email", "Email is required."));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void AddFailure_Should_Append_To_Invalid_Result()
    {
        var result = ValidationResult.FromFailure(ValidationFailureTestData.Create("Email", "Email is required."))
            .AddFailure(ValidationFailureTestData.Create("Age", "Age is invalid."));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void AddFailure_Should_Throw_When_Failure_Is_Null()
    {
        var act = () => ValidationResult.Success.AddFailure(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddFailures_Should_Append_Failures()
    {
        var result = ValidationResult.FromFailure(ValidationFailureTestData.Create("Email", "Email is required."))
            .AddFailures(new[]
            {
                ValidationFailureTestData.Create("Age", "Age is invalid.")
            });

        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void AddFailures_Should_Return_Same_Result_When_Empty()
    {
        var original = ValidationResult.Success;

        var result = original.AddFailures(Array.Empty<ValidationFailure>());

        result.Should().BeSameAs(original);
    }

    [Fact]
    public void AddFailures_Should_Throw_When_Collection_Is_Null()
    {
        var act = () => ValidationResult.Success.AddFailures(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddFailures_Should_Throw_When_Collection_Contains_Null()
    {
        var failures = new ValidationFailure?[]
        {
            ValidationFailureTestData.Create("Email", "Email is required."),
            null
        };

        var act = () => ValidationResult.Success.AddFailures(failures!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot contain null items*");
    }
}
