using System;
using AspNetCore.Yandex.ObjectStorage.Enums;

namespace AspNetCore.Yandex.ObjectStorage.Helpers
{
    internal static class EnumToStringHelpers
    {
        internal static string GetACLHeaderValue(this ACLType type)
        {
            return type switch
            {
                ACLType.Private => "private",
                ACLType.PublicRead => "public-read",
                ACLType.PublicReadWrite => "public-read-write",
                ACLType.AuthRead => "authenticated-read",
                ACLType.OwnerFullControl => "bucket-owner-full-control",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}