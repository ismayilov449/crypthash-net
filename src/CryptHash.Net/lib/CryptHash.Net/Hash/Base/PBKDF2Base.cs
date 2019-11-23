﻿using CryptHash.Net.Util;
using CryptHash.Net.Hash.HashResults;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace CryptHash.Net.Hash
{
    public abstract class PBKDF2Base
    {
        private const int _hashBitSize = 256;
        private const int _hashBytesLength = (_hashBitSize / 8);

        private static readonly int _saltBitSize = 128;
        private static readonly int _saltBytesLength = (_saltBitSize / 8);

        private const int _iterations = 100000;

        private const KeyDerivationPrf _prf = KeyDerivationPrf.HMACSHA1;

        internal GenericHashResult ComputeHash(string stringToComputeHash, byte[] salt = null, KeyDerivationPrf prf = _prf, int iterationCount = _iterations,
            int numBytesRequested = _hashBytesLength)
        {
            if (string.IsNullOrWhiteSpace(stringToComputeHash))
            {
                return new GenericHashResult()
                {
                    Success = false,
                    Message = MessageDictionary.Instance["Hash.InputRequired"]
                };
            }

            //salt = salt ?? CommonMethods.GenerateSalt(_saltBytesLength);
            salt = salt ?? CommonMethods.GenerateSalt();
            byte[] hash;

            try
            {
                hash = KeyDerivation.Pbkdf2(
                    password: stringToComputeHash,
                    salt: salt,
                    prf: prf,
                    iterationCount: iterationCount,
                    numBytesRequested: numBytesRequested
                );

                var hashBytes = new byte[(_saltBytesLength + _hashBytesLength)];
                Array.Copy(salt, 0, hashBytes, 0, _saltBytesLength);
                Array.Copy(hash, 0, hashBytes, _saltBytesLength, _hashBytesLength);

                return new GenericHashResult()
                {
                    Success = true,
                    Message = MessageDictionary.Instance["Hash.Compute.Success"],
                    HashString = Convert.ToBase64String(hashBytes),
                    HashBytes = hashBytes
                };
            }
            catch (Exception ex)
            {
                return new GenericHashResult()
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }

        internal GenericHashResult VerifyHash(string stringToBeVerified, string hash, KeyDerivationPrf prf = _prf, int iterationCount = _iterations,
            int numBytesRequested = _hashBytesLength)
        {
            if (string.IsNullOrWhiteSpace(stringToBeVerified))
            {
                return new GenericHashResult()
                {
                    Success = false,
                    Message = MessageDictionary.Instance["Hash.InputRequired"]
                };
            }

            if (string.IsNullOrWhiteSpace(hash))
            {
                return new GenericHashResult()
                {
                    Success = false,
                    Message = MessageDictionary.Instance["Hash.VerificationHashRequired"]
                };
            }

            var hashWithSaltBytes = Convert.FromBase64String(hash);

            if (hashWithSaltBytes.Length != (_saltBytesLength + _hashBytesLength))
            {
                return new GenericHashResult()
                {
                    Success = false,
                    Message = MessageDictionary.Instance["Common.IncorrectInputLengthError"]
                };
            }

            var saltBytes = new byte[_saltBytesLength];
            Array.Copy(hashWithSaltBytes, 0, saltBytes, 0, _saltBytesLength);

            var hashBytes = new byte[_hashBytesLength];
            Array.Copy(hashWithSaltBytes, _saltBytesLength, hashBytes, 0, _hashBytesLength);

            var result = ComputeHash(stringToBeVerified, saltBytes, prf, iterationCount, numBytesRequested);

            if (string.Equals(result.HashString, hash))
            {
                return new GenericHashResult()
                {
                    Success = true,
                    Message = MessageDictionary.Instance["Hash.Match"],
                    HashString = hash,
                    HashBytes = result.HashBytes
                };
            }
            else
            {
                return new GenericHashResult()
                {
                    Success = false,
                    Message = MessageDictionary.Instance["Hash.DoesNotMatch"]
                };
            }
        }
    }
}
