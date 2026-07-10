// <copyright file="ValidationExtensionsTests.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>

using Atya.Errors.Exceptions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Foundation.Results;

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

    [Fact]
    public void ToError_Should_Convert_ValidationFailure()
    {
        var failure = ValidationFailureTestData.Create(
            "Email",
            "Email is required.",
            "atya.errors.validation.email_required");

        var error = failure.ToError();

        error.Code.Should().Be("atya.errors.validation.email_required");
        error.Message.Should().Be("Email is required.");
        error.Kind.Should().Be(ErrorKind.Validation);
        error.Target.Should().Be("Email");
        error.Details.Should().BeEmpty();
    }

    [Fact]
    public void ToError_Should_Use_Default_Code_When_Failure_Has_No_Code()
    {
        var failure = ValidationFailureTestData.Create("Email", "Email is required.");

        var error = failure.ToError();

        error.Code.Should().Be("atya.errors.validation.failed");
    }

    [Fact]
    public void ToError_Should_Throw_When_Failure_Is_Null()
    {
        var act = () => ValidationExtensions.ToError(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToResult_Should_Return_Success_When_Validation_Is_Valid()
    {
        var result = ValidationResult.Success.ToResult();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ToResult_Should_Return_Failure_When_Validation_Is_Invalid()
    {
        var validationResult = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Email", "Email is required."));

        var result = validationResult.ToResult();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("atya.errors.validation.failed");
        result.Error.Message.Should().Be("Validation failed.");
        result.Error.Kind.Should().Be(ErrorKind.Validation);
        result.Error.Details.Should().ContainSingle();
        result.Error.Details[0].Code.Should().Be("atya.errors.validation.failed");
        result.Error.Details[0].Message.Should().Be("Email is required.");
        result.Error.Details[0].Target.Should().Be("Email");
        result.Error.Details[0].Kind.Should().Be(ErrorKind.Validation);
    }

    [Fact]
    public void ToResult_Should_Use_Custom_Error_When_Validation_Is_Invalid()
    {
        var validationResult = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Name", "Name is required."));

        var result = validationResult.ToResult(
            "atya.errors.validation.command_invalid",
            "The command is invalid.");

        result.Error.Code.Should().Be("atya.errors.validation.command_invalid");
        result.Error.Message.Should().Be("The command is invalid.");
        result.Error.Kind.Should().Be(ErrorKind.Validation);
        result.Error.Details.Should().ContainSingle();
        result.Error.Details[0].Target.Should().Be("Name");
    }

    [Fact]
    public void ToResult_Should_Use_Failure_Code_For_Detail_When_Present()
    {
        var validationResult = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Email", "Email is invalid.", "validation.email"));

        var result = validationResult.ToResult();

        result.Error.Details.Should().ContainSingle();
        result.Error.Details[0].Code.Should().Be("validation.email");
    }

    [Fact]
    public void ToResult_Should_Throw_When_Result_Is_Null()
    {
        var act = () => ValidationExtensions.ToResult(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null, "Validation failed.")]
    [InlineData("", "Validation failed.")]
    [InlineData(" ", "Validation failed.")]
    [InlineData("atya.errors.validation.failed", null)]
    [InlineData("atya.errors.validation.failed", "")]
    [InlineData("atya.errors.validation.failed", " ")]
    public void ToResult_Should_Throw_When_Error_Arguments_Are_Invalid(string? errorCode, string? message)
    {
        var validationResult = ValidationResult.Success;

        var act = () => validationResult.ToResult(errorCode!, message!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToResultOfT_Should_Return_Success_When_Validation_Is_Valid()
    {
        var result = ValidationResult.Success.ToResultWithValue("value");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("value");
    }

    [Fact]
    public void ToResultOfT_Should_Return_Failure_When_Validation_Is_Invalid()
    {
        var validationResult = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Email", "Email is required."));

        var result = validationResult.ToResultWithValue("value");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("atya.errors.validation.failed");
        result.Error.Kind.Should().Be(ErrorKind.Validation);
        result.Error.Details.Should().ContainSingle();
        result.Error.Details[0].Message.Should().Be("Email is required.");
        result.Error.Details[0].Target.Should().Be("Email");
        result.Error.Details[0].Kind.Should().Be(ErrorKind.Validation);
    }

    [Fact]
    public void ToResultOfT_Should_Use_Custom_Error_When_Validation_Is_Invalid()
    {
        var validationResult = ValidationResult.FromFailure(
            ValidationFailureTestData.Create("Name", "Name is required."));

        var result = validationResult.ToResultWithValue(
            "value",
            "atya.errors.validation.command_invalid",
            "The command is invalid.");

        result.Error.Code.Should().Be("atya.errors.validation.command_invalid");
        result.Error.Message.Should().Be("The command is invalid.");
        result.Error.Kind.Should().Be(ErrorKind.Validation);
        result.Error.Details.Should().ContainSingle();
        result.Error.Details[0].Target.Should().Be("Name");
    }

    [Fact]
    public void ToResultOfT_Should_Throw_When_Result_Is_Null()
    {
        var act = () => ValidationExtensions.ToResultWithValue(null!, "value");

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null, "Validation failed.")]
    [InlineData("", "Validation failed.")]
    [InlineData(" ", "Validation failed.")]
    [InlineData("atya.errors.validation.failed", null)]
    [InlineData("atya.errors.validation.failed", "")]
    [InlineData("atya.errors.validation.failed", " ")]
    public void ToResultOfT_Should_Throw_When_Error_Arguments_Are_Invalid(string? errorCode, string? message)
    {
        var validationResult = ValidationResult.Success;

        var act = () => validationResult.ToResultWithValue("value", errorCode!, message!);

        act.Should().Throw<ArgumentException>();
    }
}
