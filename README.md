# Validation

`Validation` is the repository for the `Atya.Errors.Validation` NuGet package.

| | |
| --- | --- |
| Repository | [https://github.com/AtyaLibraries/Validation](https://github.com/AtyaLibraries/Validation) |
| NuGet | `Atya.Errors.Validation` |
| License | MIT |

This package provides validation models, validator contracts, composition helpers,
conversion into `Atya.Errors.Exceptions.ValidationException`, and additive
conversion into `Atya.Foundation.Results`.

## Included APIs

- `IValidator<T>`
- `ValidationFailure`
- `ValidationResult`
- `ValidationFailureFactory`
- `ValidationExtensions`
- `ValidatorExtensions`
- `CompositeValidator<T>`

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

var result = await validator.ValidateAsync(new CreateCustomerCommand("", "bad"));
Result commandResult = result.ToResult();

if (!result.IsValid)
{
    result.ThrowIfInvalid();
}
```

See `samples/Validation.Samples.Console` for a runnable example.

## Results integration

`ValidationFailure.ToError()` creates an `ErrorKind.Validation` error with
`Target` set to the failure property name.
`ValidationResult.ToResult()` and `ValidationResult.ToResultWithValue(value)` convert
validation outcomes to untyped and typed Results. Invalid validation results return
a parent `ErrorKind.Validation` error whose `Details` contains one child `Error` per
validation failure. Each child uses the failure error code when present, otherwise
`atya.errors.validation.failed`; the child `Message` is the failure message and
`Target` is the property name.

Invalid Result conversions use `atya.errors.validation.failed` by default unless the
caller supplies a specific error code and message. `ToValidationException()` keeps
its legacy default exception code, `validation.failed`, for compatibility.

## Layout

```text
.
|-- src/Validation/
|-- tests/Validation.UnitTests/
|-- samples/Validation.Samples.Console/
|-- benchmarks/Validation.Benchmarks/
`-- .github/
```

## Build, test, pack

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test ./tests/Validation.UnitTests/Validation.UnitTests.csproj --configuration Release --no-build --collect "XPlat Code Coverage"
dotnet pack ./src/Validation/Validation.csproj --configuration Release --no-build --output artifacts/packages -p:EnablePackageValidation=true
```

Artifacts land in `artifacts/packages/`.

## Versioning

Versions are derived from git tags via [MinVer](https://github.com/adamralph/minver).
Releases are published by the central publisher, not by a workflow in this repository. After promoting `development` to `master` and tagging the `master` commit, a maintainer runs `gh workflow run publish.yml --repo AtyaLibraries/publisher -f repository=AtyaLibraries/Validation -f ref=<tag>`; see the publisher RUNBOOK (https://github.com/AtyaLibraries/publisher/blob/main/docs/RUNBOOK.md).

## Compatibility

The package intentionally targets `net10.0`.
