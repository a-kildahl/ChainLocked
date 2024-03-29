﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChainLocked.Cryptography
{
    /// <summary>
    /// POCO for a public / private key pair.
    /// </summary>
    public class KeyPair
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
