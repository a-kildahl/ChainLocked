using System;
using System.Collections.Generic;
using System.Text;

namespace ChainLocked
{
    public class DigitalSignature
    {
        public string SignerId { get; internal set; }
        public string PublicKey { get; internal set; }
        public string Signature { get; internal set; }
    }
}
