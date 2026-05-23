// <copyright file="ValidationExtensionsTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Exceptions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;

namespace Validation.UnitTests;

public sealed class ValidationExtensionsTests
{
    [Fact]
    public void ToValidationException_Should_Convert_Invalid_Result()
    {
        var result = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Email", "Email is required.", "validation.required"));

        var exception = result.ToValidationException(
            message: "Validation failed.",
            errorCode: "validation.failed",
            metadata: new Dictionary<string, object?> { ["source"] = "unit-test" });

        exception.Message.Should().Be("Validation failed.");
        exception.ErrorCode.Should().Be("validation.failed");
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].PropertyName.Should().Be("Email");
        exception.Metadata["source"].Should().Be("unit-test");
    }

    [Fact]
    public void ToValidationException_Should_Throw_For_Valid_Result()
    {
        var act = () => ValidationResult.Success.ToValidationException();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToValidationException_Should_Throw_When_Result_Is_Null()
    {
        var act = () => ValidationExtensions.ToValidationException(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToValidationException_Should_Throw_When_Message_Is_Empty()
    {
        var result = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Email", "Email is required.", "validation.required"));

        var act = () => result.ToValidationException(" ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ThrowIfInvalid_Should_Not_Throw_For_Valid_Result()
    {
        var act = () => ValidationResult.Success.ThrowIfInvalid();

        act.Should().NotThrow();
    }

    [Fact]
    public void ThrowIfInvalid_Should_Throw_For_Invalid_Result()
    {
        var result = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Age", "Age is invalid.", "validation.invalid", 0));

        var act = () => result.ThrowIfInvalid();

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void ThrowIfInvalid_Should_Throw_When_Result_Is_Null()
    {
        var act = () => ValidationExtensions.ThrowIfInvalid(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Merge_Should_Combine_Two_Results()
    {
        var left = ValidationResult.FromFailure(ValidationFailureTestData.Create("Email", "Email is required."));
        var right = ValidationResult.FromFailure(ValidationFailureTestData.Create("Age", "Age is invalid."));

        var result = left.Merge(right);

        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Merge_Should_Throw_When_Left_Is_Null()
    {
        var right = ValidationResult.Success;

        var act = () => ValidationExtensions.Merge(null!, right);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Merge_Should_Throw_When_Right_Is_Null()
    {
        var left = ValidationResult.Success;

        var act = () => left.Merge(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
