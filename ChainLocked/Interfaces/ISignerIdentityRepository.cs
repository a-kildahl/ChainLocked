using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChainLocked.Interfaces
{
    public interface ISignerIdentityRepository
    {
        /// <summary>
        /// Submits a new signer identity to the underlying storage.
        /// </summary>
        /// <param name="signerId">The ID of the signer.</param>
        /// <param name="publicKey">The signers public key.</param>
        /// <returns></returns>
        Task Save(string signerId, string publicKey);
        /// <summary>
        /// Retrieves the public key from the underlying storage.
        /// </summary>
        /// <param name="signerId">The signers ID.</param>
        /// <returns>The public key.</returns>
        Task<string> GetPublicKey(string signerId);
    }
}
