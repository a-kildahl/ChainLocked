using ChainLocked.Cryptography;
using ChainLocked.Exceptions;
using ChainLocked.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ChainLocked
{
    public class BlockChain<T>
    {

        private readonly IBlockRepository<T> _blockRepository;
        private readonly ISignerIdentityRepository _signerRepository;

        public BlockChain(IBlockRepository<T> blockRepository, ISignerIdentityRepository signerRepository)
        {
            _blockRepository = blockRepository;
            _signerRepository = signerRepository;
        }

        /// <summary>
        /// Factory method for creating a new block. 
        /// This will not insert the block into the chain!
        /// </summary>
        /// <param name="data">The message payload of the block.</param>
        /// <param name="signerId">The ID of the person that is going to digitally sign the block.</param>
        /// <param name="signersPublicKey">The public key of the person that is going to digitally sign the block.</param>
        /// <returns>A new block</returns>
        public async Task<Block<T>> ConstructBlock(T data, string signerId, string signersPublicKey)
        {
            Block<T> previousBlock = await _blockRepository.GetLatestBlock();
            long newBlockNo = previousBlock == null ? 0 : previousBlock.BlockNumber + 1;

            return new Block<T>(data, newBlockNo, previousBlock?.BlockHash, signerId, signersPublicKey);
        }

        /// <summary>
        /// Accepts a new block into the chain.
        /// Requires that the block have been signed. 
        /// </summary>
        /// <param name="block">The block to accept into the chain.</param>
        /// <exception cref="ChainLocked.Exceptions.BlockFailedVerificationException">If the block fails initial verification.</exception>
        /// <returns>Awaitable task.</returns>
        public async Task AcceptNewBlock(Block<T> block)
        {
            if (block.Signature == default || 
                string.IsNullOrWhiteSpace(block.Signature.Signature) || 
                string.IsNullOrWhiteSpace(block.Signature.SignerId) ||
                string.IsNullOrWhiteSpace(block.Signature.PublicKey))
            {
                throw new BlockNotSignedException<T>(block);
            }

            Block<T> previousBlock = await _blockRepository.GetLatestBlock();
            ThrowVerificationExceptionsIfNotValid(VerifyBlockHash(block, previousBlock?.BlockHash), await VerifyBlockSignature(block), block);

            await _blockRepository.SaveBlock(block);
        }

        /// <summary>
        /// Verify the entire chain.
        /// </summary>
        /// <exception cref="ChainLocked.Exceptions.BlockFailedVerificationException">If a block fails verification.</exception>
        public Task VerifyChain()
        {
            return VerifyChain(0);
        }

        /// <summary>
        /// Verifies the blockchain starting from the indicated block.
        /// </summary>
        /// <exception cref="ChainLocked.Exceptions.BlockFailedVerificationException">If a block fails verification.</exception>
        /// <param name="startBlockNumber">The blocknumber to start from.</param>
        public async Task VerifyChain(int startBlockNumber)
        {
            string previousHash = null;

            for (Block<T> currentBlock = await _blockRepository.GetBlockByNumber(startBlockNumber); 
                currentBlock != null; 
                currentBlock = await _blockRepository.GetBlockByNumber(currentBlock.BlockNumber + 1))
            {
                bool isHashValid = VerifyBlockHash(currentBlock, previousHash);
                bool isSignatureValid = await VerifyBlockSignature(currentBlock);

                ThrowVerificationExceptionsIfNotValid(isHashValid, isSignatureValid, currentBlock);
                previousHash = currentBlock.BlockHash;
            }
        }

        /// <summary>
        /// Verifies that the block hash is valid for the given block.
        /// </summary>
        /// <param name="block">The block to verify.</param>
        /// <param name="previousHash">The hash of the previous block in the chain.</param>
        /// <returns>If the block hash is valid.</returns>
        public bool VerifyBlockHash(Block<T> block, string previousHash)
        {
            string computedHash = block.CalculateBlockHash(previousHash);
            return computedHash == block.BlockHash;
        }

        /// <summary>
        /// Verifies that the digital signature of the block is valid for the indicated signer.
        /// </summary>
        /// <param name="block">The block to verify.</param>
        /// <returns>If the digital signature is valid for the block.</returns>
        public async Task<bool> VerifyBlockSignature(Block<T> block)
        {
            DigitalSignature signature = block.Signature;
            string signersPublicKey = await _signerRepository.GetPublicKey(signature.SignerId);

            return signersPublicKey == signature.PublicKey &&
                Signer.VerifySignature(signersPublicKey, block.BlockHash, signature.Signature);
        }

        private void ThrowVerificationExceptionsIfNotValid(bool isHashValid, bool isSignatureValid, Block<T> block)
        {
            if (!isHashValid)
                throw new BlockFailedVerificationException<T>(block, "Block failed hash verification");
            else if (!isSignatureValid)
                throw new BlockFailedVerificationException<T>(block, "Block failed signature verification");
        }
    }
}
