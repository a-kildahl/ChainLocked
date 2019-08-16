using System;
using System.Collections.Generic;
using System.Text;

namespace ChainLocked.Exceptions
{
    public abstract class BlockChainException<T> : Exception
    {
        public Block<T> Block { get; protected set; }

        public BlockChainException(Block<T> block)
            : this(block, null)
        {

        }

        public BlockChainException(Block<T> block, string message)
            : this(block, message, null)
        {

        }

        public BlockChainException(Block<T> block, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Block = block;
        }
    }
}
