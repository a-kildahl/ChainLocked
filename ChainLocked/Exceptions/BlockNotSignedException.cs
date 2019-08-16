using System;
using System.Collections.Generic;
using System.Text;

namespace ChainLocked.Exceptions
{
    public class BlockNotSignedException<T> : BlockChainException<T>
    {
        public BlockNotSignedException(Block<T> block)
            : base(block)
        {

        }
    }
}
