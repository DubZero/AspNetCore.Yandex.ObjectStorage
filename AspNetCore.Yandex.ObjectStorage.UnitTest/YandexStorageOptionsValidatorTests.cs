using System;

using AspNetCore.Yandex.ObjectStorage.Configuration;

using Microsoft.Extensions.Options;

using Xunit;
// ReSharper disable ConvertToLocalFunction

namespace AspNetCore.Yandex.ObjectStorage.UnitTest;

public class YandexStorageOptionsValidatorTests
{
    [Fact(DisplayName = "[001] Validate - not empty values of bucket, access key, secret key. ")]
    public void Validate_NotEmptyValues_WithDefault_ReturnsValid()
    {
        // Arrange
        var options = new YandexStorageOptions
        {
            BucketName = "test",
            AccessKey = "test",
            SecretKey = "test"
        };

        var validator = new YandexStorageOptionsValidator();

        // Act
        var result = validator.ValidateOrThrow(options);

        // Assert
        Assert.True(result);
    }

    [Fact(DisplayName = "[002] Validate - empty value of bucket.")]
    public void Validate_BucketNotSet_WithDefault_ThrowsValidationException()
    {
        // Arrange
        var options = new YandexStorageOptions
        {
            AccessKey = "test",
            SecretKey = "test"
        };

        var validator = new YandexStorageOptionsValidator();

        // Act
        Action result = () => validator.ValidateOrThrow(options);

        // Assert
        Assert.Throws<OptionsValidationException>(result);
    }

    [Fact(DisplayName = "[003] Validate - default constructor.")]
    public void Validate_DefaultConstructor_ThrowsValidationException()
    {
        // Arrange
        var options = new YandexStorageOptions();

        var validator = new YandexStorageOptionsValidator();

        // Act
        Action result = () => validator.ValidateOrThrow(options);

        // Assert
        Assert.Throws<OptionsValidationException>(result);
    }

    [Fact(DisplayName = "[004] Validate - options is null.")]
    public void Validate_OptionsIsNull_ThrowsValidationException()
    {
        // Arrange
        YandexStorageOptions options = null;

        var validator = new YandexStorageOptionsValidator();

        // Act
        Action result = () => validator.ValidateOrThrow(options);

        // Assert
        Assert.Throws<OptionsValidationException>(result);
    }
}