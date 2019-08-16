using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChainLocked.Cryptography
{
    public static class Signer
    {
        /// <summary>
        /// Generates a new set of keys for a new signer.
        /// </summary>
        /// <returns></returns>
        public static KeyPair GenerateNewKeyPair()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                byte[] publicKey = rsa.ExportCspBlob(false);
                byte[] privateKey = rsa.ExportCspBlob(true);

                return new KeyPair
                {
                    PublicKey = Convert.ToBase64String(publicKey),
                    PrivateKey = Convert.ToBase64String(privateKey)
                };
            }
        }

        /// <summary>
        /// Generates a signature using the provided private key for the hash to sign.
        /// </summary>
        /// <param name="privateKey">The private key to sign with.</param>
        /// <param name="hashToSign">The hash to sign.</param>
        /// <returns></returns>
        public static string Sign(string privateKey, string hashToSign)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportCspBlob(Convert.FromBase64String(privateKey));

                byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(hashToSign), "SHA256");
                return Convert.ToBase64String(signature);
            }
        }

        /// <summary>
        /// Verify the signature with the public key.
        /// </summary>
        /// <param name="publicKey">The public key to use for verification.</param>
        /// <param name="signedHash">The hash that was signed.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <returns></returns>
        public static bool VerifySignature(string publicKey, string signedHash, string signature)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportCspBlob(Convert.FromBase64String(publicKey));

                byte[] hashBuffer = Encoding.UTF8.GetBytes(signedHash);
                byte[] signatureBuffer = Convert.FromBase64String(signature);

                return rsa.VerifyData(hashBuffer, "SHA256", signatureBuffer);
            }
        }
    }
}
