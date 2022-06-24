using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Apiraiser.Helpers
{
    public static class Hash
    {

        /* Hashing algorithms guideline:
         *
         * Salt is needed for Pbkdf2 algorithm.
         *
         * No Salt is required in case of Bcrypt algorithm
         * because bcrypt generates random Salt which is much more secure
         * Work factor can be used to increase the security even further.
        */

        public static string Create(string value, int workFactor = 11)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(value, BCrypt.Net.HashType.SHA512, workFactor);

        }
        public static bool Validate(string value, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(value, hash, BCrypt.Net.HashType.SHA512);
        }

        public static string CreateUsingPbkdf2(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                 password: value,
                                 salt: Encoding.UTF8.GetBytes(salt),
                                 prf: KeyDerivationPrf.HMACSHA512,
                                 iterationCount: 10000,
                                 numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);

        }

        public static bool ValidateUsingPbkdf2(string value, string salt, string hash)
        {
            //return Create(value, salt) == hash;
            return BCrypt.Net.BCrypt.EnhancedVerify(value, hash);
        }



    }
}
