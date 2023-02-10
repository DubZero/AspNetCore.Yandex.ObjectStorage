using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage.Configuration
{
    internal class YandexStorageOptionsValidator
    {
        private readonly string[] _protocolCorrectValues = { "http", "https" };

        internal bool ValidateOrThrow(YandexStorageOptions options)
        {
            var errors = new List<string>();

            if (options is null)
            {
                errors.Add($"Input value `{nameof(options)}` can't be `null`");
                throw new OptionsValidationException("YandexStorageOptions", typeof(YandexStorageOptions),
                    errors);
            }

            if (string.IsNullOrWhiteSpace(options.HostName))
            {
                errors.Add($"Input value `{nameof(options.HostName)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                errors.Add($"Input value `{nameof(options.Endpoint)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.Location))
            {
                errors.Add($"Input value `{nameof(options.Location)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.SecretKey))
            {
                errors.Add($"Input value `{nameof(options.SecretKey)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.AccessKey))
            {
                errors.Add($"Input value `{nameof(options.AccessKey)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.Protocol))
            {
                errors.Add($"Input value `{nameof(options.Protocol)}` can't be `null`");
            }

            if (!_protocolCorrectValues.Contains(options.Protocol))
            {
                errors.Add($"`{nameof(options.Protocol)}` must be one of `{string.Join(", ", _protocolCorrectValues)}`");
            }

            return errors.Any()
                ? throw new OptionsValidationException("YandexStorageOptions", typeof(YandexStorageOptions), errors)
                : true;
        }
    }
}