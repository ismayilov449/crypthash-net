﻿using CryptHash.Net.Encryption.AES.AE;
using CryptHash.Net.Encryption.AES.EncryptionResults;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace CryptHash.Net.Tests.Encryption.AES.AE
{
    [TestClass]
    public class AE_AES_192_CBC_HMAC_SHA_384_Tests
    {
        AE_AES_192_CBC_HMAC_SHA_384 _aes192cbc = new AE_AES_192_CBC_HMAC_SHA_384();
        string _testString = "This is a test string!";
        string _password = "P4$$w0rd#123";

        [TestMethod]
        public void Test_EncryptString_with_append_encryption_data()
        {
            var appendEncryptionData = true;

            var aes192cbc_EncryptionResult = _aes192cbc.EncryptString(_testString, _password, appendEncryptionDataToOutputString: appendEncryptionData);

            Assert.IsTrue(aes192cbc_EncryptionResult.Success, aes192cbc_EncryptionResult.Message);
        }

        [TestMethod]
        public void Test_EncryptString_without_append_encryption_data()
        {
            var appendEncryptionData = false;

            var aes192cbc_EncryptionResult = _aes192cbc.EncryptString(_testString, _password, appendEncryptionDataToOutputString: appendEncryptionData);

            Assert.IsTrue(aes192cbc_EncryptionResult.Success, aes192cbc_EncryptionResult.Message);
        }

        [TestMethod]
        public void Test_DecryptString_with_encryption_data_appended()
        {
            string errorMessage = "";
            var appendEncryptionData = true;
            var aes192cbc_DecryptionResult = new AesEncryptionResult();

            var aes192cbc_EncryptionResult = _aes192cbc.EncryptString(_testString, _password, appendEncryptionDataToOutputString: appendEncryptionData);

            if (aes192cbc_EncryptionResult.Success)
            {
                aes192cbc_DecryptionResult = _aes192cbc.DecryptString(aes192cbc_EncryptionResult.EncryptedDataBase64String, _password, hasEncryptionDataAppendedInIntputString: appendEncryptionData);

                if (!aes192cbc_DecryptionResult.Success)
                    errorMessage = aes192cbc_DecryptionResult.Message;
            }
            else
                errorMessage = aes192cbc_EncryptionResult.Message;

            Assert.IsTrue((aes192cbc_EncryptionResult.Success && aes192cbc_DecryptionResult.Success && aes192cbc_DecryptionResult.DecryptedDataString.Equals(_testString)), errorMessage);
        }

        [TestMethod]
        public void Test_DecryptString_without_encryption_data_appended()
        {
            string errorMessage = "";
            var appendEncryptionData = false;
            var aes192cbc_DecryptionResult = new AesEncryptionResult();

            var aes192cbc_EncryptionResult = _aes192cbc.EncryptString(_testString, _password, appendEncryptionDataToOutputString: appendEncryptionData);

            if (aes192cbc_EncryptionResult.Success)
            {
                aes192cbc_DecryptionResult = _aes192cbc.DecryptString(aes192cbc_EncryptionResult.EncryptedDataBytes, Encoding.UTF8.GetBytes(_password),
                    hasEncryptionDataAppendedInIntputString: appendEncryptionData, aes192cbc_EncryptionResult.Tag, aes192cbc_EncryptionResult.Salt, aes192cbc_EncryptionResult.IV);

                if (!aes192cbc_DecryptionResult.Success)
                    errorMessage = aes192cbc_DecryptionResult.Message;
            }
            else
                errorMessage = aes192cbc_EncryptionResult.Message;

            Assert.IsTrue((aes192cbc_EncryptionResult.Success && aes192cbc_DecryptionResult.Success && aes192cbc_DecryptionResult.DecryptedDataString.Equals(_testString)), errorMessage);
        }
    }
}