using ChainLocked.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainLocked.DefaultImplementations
{
    public class InMemoryBlockRepository<T> : IBlockRepository<T>
    {
        private List<Block<T>> blocks = new List<Block<T>>();

        public Task<Block<T>> GetLatestBlock()
        {
            return Task.FromResult(blocks.LastOrDefault());
        }

        public Task<Block<T>> GetBlockByNumber(long no)
        {
            return Task.FromResult(blocks.FirstOrDefault(o => o.BlockNumber == no));
        }

        public Task SaveBlock(Block<T> block)
        {
            blocks.Add(block);
            return Task.CompletedTask;
        }
    }
}
