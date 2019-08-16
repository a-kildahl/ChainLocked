using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChainLocked.Interfaces
{
    public interface IBlockRepository<T>
    {
        /// <summary>
        /// Gets the last block in the chain.
        /// </summary>
        /// <returns>The last block.</returns>
        Task<Block<T>> GetLatestBlock();
        /// <summary>
        /// Gets the block with the indicated block number.
        /// </summary>
        /// <param name="no">BlockNumber to retrieve.</param>
        /// <returns>The block. Null if none where found.</returns>
        Task<Block<T>> GetBlockByNumber(long no);
        /// <summary>
        /// Submits a new block to the underlying storage.
        /// </summary>
        /// <param name="block">The block to submit.</param>
        /// <returns>Awaitable task.</returns>
        Task SaveBlock(Block<T> block);
    }
}
