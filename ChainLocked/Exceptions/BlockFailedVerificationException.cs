using System;
using System.Collections.Generic;
using System.Text;

namespace ChainLocked.Exceptions
{
    public class BlockFailedVerificationException<T> : BlockChainException<T>
    {
        public BlockFailedVerificationException(Block<T> block, string message)
            : base(block, message)
        {

        }
    }
}
