using System;
using System.Linq;

namespace AspNetCore.Yandex.ObjectStorage.Configuration
{
    internal class YandexStorageOptionsValidator
    {
        private readonly string[] _protocolCorrectValues = { "http", "https" };


        internal bool Validate(YandexStorageOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException($"Input value `{nameof(options)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.HostName))
            {
                throw new ArgumentNullException($"Input value `{nameof(options.HostName)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                throw new ArgumentNullException($"Input value `{nameof(options.Endpoint)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.Location))
            {
                throw new ArgumentNullException($"Input value `{nameof(options.Location)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.SecretKey))
            {
                throw new ArgumentNullException($"Input value `{nameof(options.SecretKey)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.AccessKey))
            {
                throw new ArgumentNullException($"Input value `{nameof(options.AccessKey)}` can't be `null`");
            }

            if (string.IsNullOrWhiteSpace(options.Protocol))
            {
                throw new ArgumentNullException($"Input value `{nameof(options.Protocol)}` can't be `null`");
            }

            if (!_protocolCorrectValues.Contains(options.Protocol))
            {
                throw new ArgumentNullException($"`{nameof(options.Protocol)}` must be on of `{string.Join(", ", "_protocolCorrectValues)}`")}");
            }

            return true;
        }
    }
}