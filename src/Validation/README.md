# Validation

Transport-agnostic validation abstractions, models, helpers, and composite validators for the Atya Errors group.

This package provides small validation building blocks for domain, application,
worker, and API layers without any transport-specific concepts. It intentionally
does not include a rules DSL, reflection-based validation, ASP.NET integration,
or localization.

## Install

```bash
dotnet add package Atya.Errors.Validation
```

## Supported framework

`Atya.Errors.Validation` targets `net10.0`.

## Quick start

```csharp
using Atya.Errors.Validation.Abstractions;
using Atya.Errors.Validation.Extensions;
using Atya.Errors.Validation.Models;
using Atya.Errors.Validation.Validators;
using Atya.Foundation.Results;

var validator = new CompositeValidator<CreateCustomerCommand>(
[
    new NameRequiredValidator(),
    new EmailFormatValidator(),
]);

var command = new CreateCustomerCommand("", "not-an-email");
var result = await validator.ValidateAsync(command);
Result commandResult = result.ToResult();

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.Message}");
    }
}

sealed record CreateCustomerCommand(string Name, string Email);

sealed class NameRequiredValidator : IValidator<CreateCustomerCommand>
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

sealed class EmailFormatValidator : IValidator<CreateCustomerCommand>
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
```

## Core APIs

- `IValidator<T>` defines the async validation contract.
- `ValidationFailure` is an immutable value object for one property or logical field failure.
- `ValidationResult` represents success or one or more failures.
- `ValidationFailureFactory` creates standard Atya validation failures.
- `ValidatorExtensions.ValidateAllAsync` executes validators in order and combines failures.
- `ValidatorExtensions.ValidateAndThrowAsync` validates with one validator and throws on failure.
- `CompositeValidator<T>` composes several validators behind one `IValidator<T>`.
- `ValidationExtensions.ThrowIfInvalid` and `ToValidationException` convert failures into `Atya.Errors.Exceptions.ValidationException`.
- `ValidationExtensions.ToError` and `ToResult` convert validation failures into `Atya.Foundation.Results` errors and results.

## Behavior

- `ValidationFailure` trims `PropertyName`, `Message`, and non-empty `ErrorCode` values.
- Empty or whitespace `PropertyName` and `Message` values throw `ArgumentException`.
- Empty failure collections produce `ValidationResult.Success`.
- Result and failure collections are copied before storage, so callers cannot mutate a result through the original input collection.
- Composite and extension-based validation runs validators sequentially in the order provided.
- Cancellation is checked before each validator and the same token is passed to validators.
- Validators must return a non-null `ValidationResult`; a null result throws `ArgumentNullException`.

## Exceptions

Use `ThrowIfInvalid` when validation errors should become an Atya validation exception:

```csharp
await validator.ValidateAndThrowAsync(command);
```

The default exception message is `Validation failed.` and the default error code
is `validation.failed`. Provide explicit values when those should be part of a
public API contract.

## Results

Use `ToResult` when validation errors are expected domain outcomes:

```csharp
ValidationResult validation = await validator.ValidateAsync(command);
Result result = validation.ToResult();
Result<CreateCustomerCommand> typed = validation.ToResultWithValue(command);
```

Invalid validation results become `ErrorKind.Validation` failures. The parent
error contains one child `Error` in `Details` for each validation failure. Each
child uses the failure error code when present, otherwise
`atya.errors.validation.failed`; the child `Message` is the failure message and
`Target` is the property name.

The default result error code is `atya.errors.validation.failed`; pass explicit
error code and message values when an API owns a more specific contract.

## Versioning and support

Stable releases use SemVer and are produced from `vMAJOR.MINOR.PATCH` tags.
Breaking API changes should wait for the next major version. Bug fixes and new
non-breaking helpers can be released in patch or minor versions as appropriate.
